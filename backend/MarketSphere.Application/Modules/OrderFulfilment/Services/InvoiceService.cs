using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.OrderFulfilment.DTOs;
using MarketSphere.Application.Modules.OrderFulfilment.Interfaces;
using MarketSphere.Domain.Entities.OrderFulfilment;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.OrderFulfilment.Services;

public sealed class InvoiceService : IInvoiceService
{
    private readonly IApplicationDbContext _db;

    public InvoiceService(IApplicationDbContext db)
    {
        _db = db;
    }

    public Task<PagedResult<InvoiceListDto>> GetAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Invoices.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x =>
                x.InvoiceNo.Contains(search) ||
                x.Client.ClientName.Contains(search));
        }

        var projected = query
            .OrderByDescending(x => x.InvoiceDate)
            .Select(x => new InvoiceListDto(
                x.InvoiceID,
                x.InvoiceNo,
                x.OrderID,
                x.ClientID,
                x.InvoiceDate,
                x.DueDate,
                x.TotalAmount,
                x.PaidAmount,
                x.DueAmount,
                x.Status));

        return OrderFulfilmentServiceHelper.ToPagedAsync(
            projected,
            request,
            cancellationToken);
    }

    public async Task<InvoiceDetailsDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var invoice = await OrderFulfilmentServiceHelper.RequireAsync(
            _db.Invoices.AsNoTracking().Where(x => x.InvoiceID == id),
            "Invoice",
            cancellationToken);

        var items = await _db.InvoiceItems
            .AsNoTracking()
            .Where(x => x.InvoiceID == id)
            .OrderBy(x => x.InvoiceItemID)
            .Select(x => new InvoiceItemDto(
                x.InvoiceItemID,
                x.OrderItemID,
                x.SKUID,
                x.SKU.SKUCode,
                x.Quantity,
                x.UnitPrice,
                x.DiscountAmount,
                x.TaxAmount,
                x.LineTotal))
            .ToListAsync(cancellationToken);

        return new InvoiceDetailsDto(
            invoice.InvoiceID,
            invoice.InvoiceNo,
            invoice.OrderID,
            invoice.ClientID,
            invoice.InvoiceDate,
            invoice.DueDate,
            invoice.GrossAmount,
            invoice.DiscountAmount,
            invoice.TaxAmount,
            invoice.TotalAmount,
            invoice.PaidAmount,
            invoice.DueAmount,
            invoice.Status,
            items);
    }

    public async Task<int> CreateFromOrderAsync(
        CreateInvoiceRequestDto request,
        CancellationToken cancellationToken = default)
        => await _db.ExecuteInTransactionAsync(async ct =>
        {
            if (string.IsNullOrWhiteSpace(request.InvoiceNo) ||
                request.Items.Count == 0)
            {
                throw new BusinessRuleException(
                    "Invoice number and items are required.");
            }

            var number = request.InvoiceNo.Trim().ToUpperInvariant();

            if (await _db.Invoices.AnyAsync(x => x.InvoiceNo == number, ct))
                throw new ConflictException("Invoice number already exists.");

            var order = await OrderFulfilmentServiceHelper.RequireAsync(
                _db.Orders.Where(x => x.OrderID == request.OrderID),
                "Order",
                ct);

            if (order.Status is not (
                OrderStatus.StockAllocated or OrderStatus.Invoiced))
            {
                throw new BusinessRuleException(
                    "A stock-allocated order is required.");
            }

            var requestedIDs = request.Items
                .Select(x => x.OrderItemID)
                .Distinct()
                .ToArray();

            if (requestedIDs.Length != request.Items.Count ||
                request.Items.Any(x => x.Quantity <= 0))
            {
                throw new BusinessRuleException(
                    "Invoice item quantities are invalid.");
            }

            var orderItems = await _db.OrderItems
                .Where(x => x.OrderID == order.OrderID &&
                            requestedIDs.Contains(x.OrderItemID))
                .ToListAsync(ct);

            if (orderItems.Count != requestedIDs.Length)
                throw new NotFoundException(
                    "One or more order items were not found.");

            var invoice = new Invoice
            {
                InvoiceNo = number,
                OrderID = order.OrderID,
                ClientID = order.ClientID,
                InvoiceDate = request.InvoiceDate,
                DueDate = request.DueDate,
                Status = InvoiceStatus.Issued
            };

            await _db.AddAsync(invoice, ct);

            foreach (var input in request.Items)
            {
                var item = orderItems.Single(
                    x => x.OrderItemID == input.OrderItemID);

                var invoiced = await _db.InvoiceItems
                    .Where(x => x.OrderItemID == item.OrderItemID &&
                                x.Invoice.Status != InvoiceStatus.Cancelled)
                    .SumAsync(x => (decimal?)x.Quantity, ct) ?? 0;

                var remaining = item.ApprovedQuantity - invoiced;

                if (input.Quantity > remaining)
                    throw new BusinessRuleException(
                        BusinessRuleMessages.InvoiceQuantityExceeded);

                var ratio = item.ApprovedQuantity == 0
                    ? 0
                    : input.Quantity / item.ApprovedQuantity;

                var discount = Math.Round(item.DiscountAmount * ratio, 2);
                var tax = Math.Round(item.TaxAmount * ratio, 2);
                var gross = Math.Round(item.UnitPrice * input.Quantity, 2);
                var total = gross - discount + tax;

                await _db.AddAsync(
                    new InvoiceItem
                    {
                        Invoice = invoice,
                        OrderItemID = item.OrderItemID,
                        SKUID = item.SKUID,
                        Quantity = input.Quantity,
                        UnitPrice = item.UnitPrice,
                        DiscountAmount = discount,
                        TaxAmount = tax,
                        LineTotal = total
                    },
                    ct);

                invoice.GrossAmount += gross;
                invoice.DiscountAmount += discount;
                invoice.TaxAmount += tax;
                invoice.TotalAmount += total;
            }

            invoice.DueAmount = invoice.TotalAmount;
            order.Status = OrderStatus.Invoiced;

            await _db.SaveChangesAsync(ct);
            await OrderFulfilmentServiceHelper.UpdateClientDueAsync(
                _db,
                order.ClientID,
                invoice.TotalAmount,
                ct);

            return invoice.InvoiceID;
        }, cancellationToken);

    public async Task ChangeStatusAsync(
        int id,
        ChangeInvoiceStatusRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (request.Status != InvoiceStatus.Cancelled)
        {
            throw new BusinessRuleException(
                "Invoice payment and credit statuses are controlled by payment and credit-note workflows. Only cancellation is allowed here.");
        }

        await _db.ExecuteInTransactionAsync(async ct =>
        {
            var invoice = await OrderFulfilmentServiceHelper.RequireAsync(
                _db.Invoices.Where(x => x.InvoiceID == id),
                "Invoice",
                ct);

            if (invoice.Status == InvoiceStatus.Cancelled)
                throw new BusinessRuleException("The invoice is already cancelled.");

            if (invoice.PaidAmount > 0 ||
                invoice.Status is InvoiceStatus.PartiallyPaid or InvoiceStatus.Paid)
            {
                throw new BusinessRuleException(
                    "An invoice with payment activity cannot be cancelled.");
            }

            var hasPostedCreditNote = await _db.CreditNotes.AnyAsync(
                x => x.InvoiceID == invoice.InvoiceID &&
                     x.Status == CreditNoteStatus.Posted,
                ct);

            if (hasPostedCreditNote ||
                invoice.Status is InvoiceStatus.PartiallyCredited or InvoiceStatus.Credited)
            {
                throw new BusinessRuleException(
                    "An invoice with posted credit activity cannot be cancelled.");
            }

            var dueToReverse = invoice.DueAmount;
            invoice.Status = InvoiceStatus.Cancelled;

            if (dueToReverse > 0)
            {
                await OrderFulfilmentServiceHelper.UpdateClientDueAsync(
                    _db,
                    invoice.ClientID,
                    -dueToReverse,
                    ct);
            }

            var hasOtherActiveInvoice = await _db.Invoices.AnyAsync(
                x => x.OrderID == invoice.OrderID &&
                     x.InvoiceID != invoice.InvoiceID &&
                     x.Status != InvoiceStatus.Cancelled,
                ct);

            if (!hasOtherActiveInvoice)
            {
                var order = await _db.Orders.SingleAsync(
                    x => x.OrderID == invoice.OrderID,
                    ct);

                if (order.Status == OrderStatus.Invoiced)
                    order.Status = OrderStatus.StockAllocated;
            }

            await _db.SaveChangesAsync(ct);
            return true;
        }, cancellationToken);
    }
}
