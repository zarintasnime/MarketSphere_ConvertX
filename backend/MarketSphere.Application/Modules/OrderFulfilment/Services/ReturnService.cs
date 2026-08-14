using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.Inventory.Services;
using MarketSphere.Application.Modules.OrderFulfilment.DTOs;
using MarketSphere.Application.Modules.OrderFulfilment.Interfaces;
using MarketSphere.Domain.Constants;
using MarketSphere.Domain.Entities.OrderFulfilment;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.OrderFulfilment.Services;

public sealed class ReturnService : IReturnService
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;
    private readonly ICurrentUserService _currentUser;

    public ReturnService(
        IApplicationDbContext db,
        IDateTimeProvider clock,
        ICurrentUserService currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    public Task<PagedResult<ReturnListDto>> GetAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.ReturnRequests.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                x.ReturnNo.Contains(search) ||
                x.Client.ClientName.Contains(search));
        }

        return OrderFulfilmentServiceHelper.ToPagedAsync(
            query
                .OrderByDescending(x => x.RequestDate)
                .Select(x => new ReturnListDto(
                    x.ReturnRequestID,
                    x.ReturnNo,
                    x.ClientID,
                    x.OrderID,
                    x.InvoiceID,
                    x.DeliveryID,
                    x.RequestDate,
                    x.ReturnReason,
                    x.Status,
                    x.ResolutionType)),
            request,
            cancellationToken);
    }

    public async Task<ReturnDetailsDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity =
            await OrderFulfilmentServiceHelper.RequireAsync(
                _db.ReturnRequests
                    .AsNoTracking()
                    .Where(x => x.ReturnRequestID == id),
                "Return request",
                cancellationToken);

        var items = await _db.ReturnItems
            .AsNoTracking()
            .Where(x => x.ReturnRequestID == id)
            .Select(x => new ReturnItemDto(
                x.ReturnItemID,
                x.DeliveryItemID,
                x.SKUID,
                x.SKU.SKUCode,
                x.BatchID,
                x.RequestedQuantity,
                x.ApprovedQuantity,
                x.ReceivedQuantity,
                x.ConditionStatus,
                x.InspectionResult,
                x.Disposition,
                x.RestockQuantity,
                x.QuarantineQuantity,
                x.DamageQuantity,
                x.ReplacementQuantity,
                x.CreditAmount))
            .ToListAsync(cancellationToken);

        return new ReturnDetailsDto(
            entity.ReturnRequestID,
            entity.ReturnNo,
            entity.ClientID,
            entity.OrderID,
            entity.InvoiceID,
            entity.DeliveryID,
            entity.ComplaintID,
            entity.RequestDate,
            entity.ReturnReason,
            entity.Description,
            entity.Status,
            entity.ReceivedAtWarehouseAt,
            entity.ResolutionType,
            entity.ReplacementOrderID,
            entity.ReplacementDeliveryID,
            entity.SupplierReturnID,
            entity.ResolvedByEmployeeID,
            entity.ResolvedAt,
            entity.ResolutionNote,
            items);
    }

    public async Task<int> CreateAsync(
        CreateReturnRequestDto request,
        CancellationToken cancellationToken = default)
    {
        return await _db.ExecuteInTransactionAsync(
            async token =>
            {
                if (string.IsNullOrWhiteSpace(request.ReturnNo) ||
                    string.IsNullOrWhiteSpace(request.ReturnReason) ||
                    request.Items.Count == 0)
                {
                    throw new BusinessRuleException(
                        "Return number, reason and items are required.");
                }

                var returnNo =
                    request.ReturnNo.Trim().ToUpperInvariant();

                var returnNumberExists =
                    await _db.ReturnRequests.AnyAsync(
                        x => x.ReturnNo == returnNo,
                        token);

                if (returnNumberExists)
                {
                    throw new ConflictException(
                        "Return number already exists.");
                }

                await OrderFulfilmentServiceHelper.RequireAsync(
                    _db.Orders
                        .AsNoTracking()
                        .Where(x =>
                            x.OrderID == request.OrderID &&
                            x.ClientID == request.ClientID),
                    "Order",
                    token);

                var requestedDeliveryItemIDs =
                    request.Items
                        .Select(x => x.DeliveryItemID)
                        .ToArray();

                if (requestedDeliveryItemIDs
                        .Distinct()
                        .Count() != requestedDeliveryItemIDs.Length)
                {
                    throw new BusinessRuleException(
                        "A delivery item cannot be included more than once.");
                }

                if (request.InvoiceID.HasValue)
                {
                    var invoiceExists =
                        await _db.Invoices
                            .AsNoTracking()
                            .AnyAsync(
                                x =>
                                    x.InvoiceID == request.InvoiceID.Value &&
                                    x.OrderID == request.OrderID &&
                                    x.ClientID == request.ClientID,
                                token);

                    if (!invoiceExists)
                    {
                        throw new BusinessRuleException(
                            "The selected invoice does not belong to the return order and client.");
                    }
                }

                if (request.DeliveryID.HasValue)
                {
                    var deliveryExists =
                        await _db.Deliveries
                            .AsNoTracking()
                            .AnyAsync(
                                x =>
                                    x.DeliveryID == request.DeliveryID.Value &&
                                    x.OrderID == request.OrderID,
                                token);

                    if (!deliveryExists)
                    {
                        throw new BusinessRuleException(
                            "The selected delivery does not belong to the return order.");
                    }
                }

                var deliveryItems =
                    await _db.DeliveryItems
                        .Where(x =>
                            requestedDeliveryItemIDs.Contains(
                                x.DeliveryItemID) &&
                            x.Delivery.OrderID == request.OrderID &&
                            (!request.DeliveryID.HasValue ||
                             x.DeliveryID == request.DeliveryID.Value))
                        .ToListAsync(token);

                if (deliveryItems.Count != request.Items.Count)
                {
                    throw new NotFoundException(
                        "One or more delivery items were not found.");
                }

                var header = new ReturnRequest
                {
                    ReturnNo = returnNo,
                    ClientID = request.ClientID,
                    OrderID = request.OrderID,
                    InvoiceID = request.InvoiceID,
                    DeliveryID = request.DeliveryID,
                    ComplaintID = request.ComplaintID,
                    RequestDate = request.RequestDate,
                    ReturnReason = request.ReturnReason.Trim(),
                    Description =
                        string.IsNullOrWhiteSpace(request.Description)
                            ? null
                            : request.Description.Trim(),
                    Status = ReturnRequestStatus.Requested,
                };

                await _db.AddAsync(header, token);

                foreach (var input in request.Items)
                {
                    var delivered = deliveryItems.Single(
                        x =>
                            x.DeliveryItemID ==
                            input.DeliveryItemID);

                    var previouslyRequested =
                        await _db.ReturnItems
                            .Where(x =>
                                x.DeliveryItemID ==
                                    input.DeliveryItemID &&
                                x.ReturnRequest.Status !=
                                    ReturnRequestStatus.Rejected)
                            .SumAsync(
                                x => (decimal?)x.RequestedQuantity,
                                token) ?? 0;

                    var availableReturnQuantity =
                        delivered.QuantityDelivered -
                        previouslyRequested;

                    if (input.RequestedQuantity <= 0 ||
                        input.RequestedQuantity >
                            availableReturnQuantity)
                    {
                        throw new BusinessRuleException(
                            BusinessRuleMessages.ReturnQuantityInvalid);
                    }

                    await _db.AddAsync(
                        new ReturnItem
                        {
                            ReturnRequest = header,
                            DeliveryItemID =
                                delivered.DeliveryItemID,
                            SKUID = delivered.SKUID,
                            BatchID = delivered.BatchID,
                            RequestedQuantity =
                                input.RequestedQuantity,
                        },
                        token);
                }

                await _db.SaveChangesAsync(token);

                return header.ReturnRequestID;
            },
            cancellationToken);
    }

    public async Task ApproveAsync(
        int id,
        ApproveReturnRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var header =
            await OrderFulfilmentServiceHelper.RequireAsync(
                _db.ReturnRequests.Where(
                    x => x.ReturnRequestID == id),
                "Return request",
                cancellationToken);

        if (header.Status is not (
            ReturnRequestStatus.Requested or
            ReturnRequestStatus.UnderReview))
        {
            throw new BusinessRuleException(
                "The return request is not open for approval.");
        }

        var items = await _db.ReturnItems
            .Where(x => x.ReturnRequestID == id)
            .ToListAsync(cancellationToken);

        var inputIDs = request.Items
            .Select(x => x.ReturnItemID)
            .ToArray();

        var entityIDs = items
            .Select(x => x.ReturnItemID)
            .ToArray();

        if (inputIDs.Length != entityIDs.Length ||
            inputIDs.Distinct().Count() != inputIDs.Length ||
            !inputIDs
                .OrderBy(x => x)
                .SequenceEqual(entityIDs.OrderBy(x => x)))
        {
            throw new BusinessRuleException(
                "Every return item must be supplied exactly once.");
        }

        foreach (var input in request.Items)
        {
            var item = items.SingleOrDefault(
                x => x.ReturnItemID == input.ReturnItemID)
                ?? throw new NotFoundException(
                    "Return item was not found.");

            if (input.ApprovedQuantity < 0 ||
                input.ApprovedQuantity >
                    item.RequestedQuantity)
            {
                throw new BusinessRuleException(
                    BusinessRuleMessages.ReturnQuantityInvalid);
            }

            item.ApprovedQuantity =
                input.ApprovedQuantity;
        }

        header.Status =
            ReturnRequestStatus.Approved;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ResolveAsync(
        int id,
        ResolveReturnRequestDto request,
        CancellationToken cancellationToken = default)
    {
        await _db.ExecuteInTransactionAsync(
            async token =>
            {
                var header =
                    await OrderFulfilmentServiceHelper.RequireAsync(
                        _db.ReturnRequests.Where(
                            x => x.ReturnRequestID == id),
                        "Return request",
                        token);

                if (header.Status !=
                    ReturnRequestStatus.Approved)
                {
                    throw new BusinessRuleException(
                        "An approved return request is required.");
                }

                if (string.IsNullOrWhiteSpace(
                    request.ResolutionNote))
                {
                    throw new BusinessRuleException(
                        "Resolution note is required.");
                }

                var items = await _db.ReturnItems
                    .Where(x => x.ReturnRequestID == id)
                    .ToListAsync(token);

                var inputIDs = request.Items
                    .Select(x => x.ReturnItemID)
                    .ToArray();

                var entityIDs = items
                    .Select(x => x.ReturnItemID)
                    .ToArray();

                if (inputIDs.Length != entityIDs.Length ||
                    inputIDs.Distinct().Count() !=
                        inputIDs.Length ||
                    !inputIDs
                        .OrderBy(x => x)
                        .SequenceEqual(
                            entityIDs.OrderBy(x => x)))
                {
                    throw new BusinessRuleException(
                        "Every return item must be supplied exactly once.");
                }

                decimal totalCredit = 0;

                foreach (var input in request.Items)
                {
                    var item = items.SingleOrDefault(
                        x =>
                            x.ReturnItemID ==
                            input.ReturnItemID)
                        ?? throw new NotFoundException(
                            "Return item was not found.");

                    if (input.RestockQuantity < 0 ||
                        input.QuarantineQuantity < 0 ||
                        input.DamageQuantity < 0 ||
                        input.ReplacementQuantity < 0 ||
                        input.CreditAmount < 0)
                    {
                        throw new BusinessRuleException(
                            "Return resolution values cannot be negative.");
                    }

                    var dispositionTotal =
                        input.RestockQuantity +
                        input.QuarantineQuantity +
                        input.DamageQuantity +
                        input.ReplacementQuantity;

                    if (input.ReceivedQuantity < 0 ||
                        input.ReceivedQuantity >
                            item.ApprovedQuantity ||
                        dispositionTotal !=
                            input.ReceivedQuantity)
                    {
                        throw new BusinessRuleException(
                            BusinessRuleMessages.ReturnDispositionInvalid);
                    }

                    item.ReceivedQuantity =
                        input.ReceivedQuantity;

                    item.ConditionStatus =
                        input.ConditionStatus;

                    item.InspectionResult =
                        string.IsNullOrWhiteSpace(
                            input.InspectionResult)
                            ? null
                            : input.InspectionResult.Trim();

                    item.Disposition =
                        input.Disposition;

                    item.RestockQuantity =
                        input.RestockQuantity;

                    item.QuarantineQuantity =
                        input.QuarantineQuantity;

                    item.DamageQuantity =
                        input.DamageQuantity;

                    item.ReplacementQuantity =
                        input.ReplacementQuantity;

                    item.CreditAmount =
                        input.CreditAmount;

                    totalCredit +=
                        input.CreditAmount;

                    if (input.ReceivedQuantity > 0)
                    {
                        await InventoryServiceHelper
                            .PostMovementAsync(
                                _db,
                                request.WarehouseID,
                                item.SKUID,
                                item.BatchID,
                                StockMovementType.CustomerReturn,
                                input.ReceivedQuantity,
                                0,
                                ReferenceTypeCodes.ReturnRequest,
                                header.ReturnRequestID,
                                _currentUser.UserID,
                                "Customer return received",
                                token);

                        var balance =
                            await _db.StockBalances
                                .SingleAsync(
                                    x =>
                                        x.WarehouseID ==
                                            request.WarehouseID &&
                                        x.SKUID == item.SKUID &&
                                        x.BatchID ==
                                            item.BatchID,
                                    token);

                        balance.QuarantineQuantity +=
                            input.QuarantineQuantity;

                        balance.DamagedQuantity +=
                            input.DamageQuantity;
                    }

                    if (item.DeliveryItemID.HasValue)
                    {
                        var orderItemID =
                            await _db.DeliveryItems
                                .Where(x =>
                                    x.DeliveryItemID ==
                                    item.DeliveryItemID.Value)
                                .Select(x => x.OrderItemID)
                                .SingleAsync(token);

                        var orderItem =
                            await _db.OrderItems
                                .SingleAsync(
                                    x =>
                                        x.OrderItemID ==
                                        orderItemID,
                                    token);

                        orderItem.ReturnedQuantity +=
                            input.ReceivedQuantity;
                    }
                }

                totalCredit = decimal.Round(
                    totalCredit,
                    2,
                    MidpointRounding.AwayFromZero);

                if (totalCredit > 0)
                {
                    if (!header.InvoiceID.HasValue ||
                        string.IsNullOrWhiteSpace(
                            request.CreditNoteNo))
                    {
                        throw new BusinessRuleException(
                            "Invoice and credit-note number are required for a credited return.");
                    }

                    var invoice =
                        await OrderFulfilmentServiceHelper
                            .RequireAsync(
                                _db.Invoices.Where(
                                    x =>
                                        x.InvoiceID ==
                                            header.InvoiceID.Value &&
                                        x.OrderID ==
                                            header.OrderID &&
                                        x.ClientID ==
                                            header.ClientID),
                                "Invoice",
                                token);

                    if (totalCredit >
                        invoice.DueAmount)
                    {
                        throw new BusinessRuleException(
                            "Credit amount exceeds invoice due amount.");
                    }

                    var creditNoteNo =
                        request.CreditNoteNo
                            .Trim()
                            .ToUpperInvariant();

                    var creditNoteExists =
                        await _db.CreditNotes.AnyAsync(
                            x =>
                                x.CreditNoteNo ==
                                creditNoteNo,
                            token);

                    if (creditNoteExists)
                    {
                        throw new ConflictException(
                            "Credit-note number already exists.");
                    }

                    await _db.AddAsync(
                        new CreditNote
                        {
                            CreditNoteNo =
                                creditNoteNo,

                            ClientID =
                                header.ClientID,

                            InvoiceID =
                                invoice.InvoiceID,

                            ReturnRequestID =
                                header.ReturnRequestID,

                            CreditDate =
                                _clock.UtcNow,

                            Amount =
                                totalCredit,

                            Status =
                                CreditNoteStatus.Posted,

                            PostedAt =
                                _clock.UtcNow,

                            Reason =
                                request.ResolutionNote.Trim(),
                        },
                        token);

                    /*
                     * The invoice check constraint requires:
                     * PaidAmount + DueAmount = TotalAmount.
                     *
                     * A credit reduces the receivable amount,
                     * so both DueAmount and the net TotalAmount
                     * must be reduced by the same value.
                     */
                    invoice.DueAmount =
                        decimal.Round(
                            invoice.DueAmount -
                            totalCredit,
                            2,
                            MidpointRounding.AwayFromZero);

                    invoice.TotalAmount =
                        decimal.Round(
                            invoice.TotalAmount -
                            totalCredit,
                            2,
                            MidpointRounding.AwayFromZero);

                    invoice.Status =
                        invoice.DueAmount == 0
                            ? InvoiceStatus.Credited
                            : InvoiceStatus.PartiallyCredited;

                    await OrderFulfilmentServiceHelper
                        .UpdateClientDueAsync(
                            _db,
                            header.ClientID,
                            -totalCredit,
                            token);
                }

                header.Status =
                    ReturnRequestStatus.Resolved;

                header.ReceivedAtWarehouseAt =
                    _clock.UtcNow;

                header.ResolutionType =
                    request.ResolutionType;

                header.ResolvedByEmployeeID =
                    request.ResolvedByEmployeeID;

                header.ResolvedAt =
                    _clock.UtcNow;

                header.ResolutionNote =
                    request.ResolutionNote.Trim();

                var order = await _db.Orders
                    .SingleAsync(
                        x =>
                            x.OrderID ==
                            header.OrderID,
                        token);

                order.Status =
                    OrderStatus.Returned;

                await _db.SaveChangesAsync(token);

                return true;
            },
            cancellationToken);
    }
}