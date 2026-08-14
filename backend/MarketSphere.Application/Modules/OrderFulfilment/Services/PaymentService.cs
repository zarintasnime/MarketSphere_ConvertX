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

public sealed class PaymentService : IPaymentService
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;
    private readonly ICurrentUserService _currentUser;

    public PaymentService(
        IApplicationDbContext db,
        IDateTimeProvider clock,
        ICurrentUserService currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    public Task<PagedResult<PaymentListDto>> GetAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Payments.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x =>
                x.PaymentNo.Contains(search) ||
                x.Client.ClientName.Contains(search));
        }

        var projected = query
            .OrderByDescending(x => x.PaymentDate)
            .Select(x => new PaymentListDto(
                x.PaymentID,
                x.PaymentNo,
                x.ClientID,
                x.PaymentDate,
                x.PaymentMethod,
                x.Amount,
                x.Status,
                x.Allocations
                    .Where(a => a.AllocationType == PaymentAllocationType.Allocation)
                    .Sum(a => (decimal?)a.AllocatedAmount) ?? 0,
                x.Amount -
                (x.Allocations
                    .Where(a => a.AllocationType == PaymentAllocationType.Allocation)
                    .Sum(a => (decimal?)a.AllocatedAmount) ?? 0) +
                (x.Allocations
                    .Where(a => a.AllocationType == PaymentAllocationType.Reversal)
                    .Sum(a => (decimal?)a.AllocatedAmount) ?? 0)));

        return OrderFulfilmentServiceHelper.ToPagedAsync(
            projected,
            request,
            cancellationToken);
    }

    public async Task<PaymentDetailsDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var payment = await OrderFulfilmentServiceHelper.RequireAsync(
            _db.Payments.AsNoTracking().Where(x => x.PaymentID == id),
            "Payment",
            cancellationToken);

        var allocations = await _db.PaymentAllocations
            .AsNoTracking()
            .Where(x => x.PaymentID == id)
            .OrderBy(x => x.PaymentAllocationID)
            .Select(x => new PaymentAllocationDto(
                x.PaymentAllocationID,
                x.PaymentID,
                x.InvoiceID,
                x.AllocationType,
                x.AllocatedAmount,
                x.ReversalOfPaymentAllocationID,
                x.AllocatedAt,
                x.AllocatedByUserID))
            .ToListAsync(cancellationToken);

        return new PaymentDetailsDto(
            payment.PaymentID,
            payment.PaymentNo,
            payment.ClientID,
            payment.PaymentDate,
            payment.PaymentMethod,
            payment.Amount,
            payment.ReferenceNo,
            payment.ProofFileAttachmentID,
            payment.Status,
            payment.ReceivedByUserID,
            allocations);
    }

    public async Task<int> CreateAsync(
        CreatePaymentRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var userID = _currentUser.UserID
            ?? throw new ForbiddenBusinessActionException(
                "Authenticated user is required.");

        if (string.IsNullOrWhiteSpace(request.PaymentNo) || request.Amount <= 0)
            throw new BusinessRuleException(
                "Payment number and a positive amount are required.");

        var number = request.PaymentNo.Trim().ToUpperInvariant();

        if (await _db.Payments.AnyAsync(x => x.PaymentNo == number, cancellationToken))
            throw new ConflictException("Payment number already exists.");

        if (!await _db.Clients.AnyAsync(
                x => x.ClientID == request.ClientID,
                cancellationToken))
        {
            throw new NotFoundException("Client was not found.");
        }

        var payment = new Payment
        {
            PaymentNo = number,
            ClientID = request.ClientID,
            PaymentDate = request.PaymentDate,
            PaymentMethod = request.PaymentMethod,
            Amount = request.Amount,
            ReferenceNo = string.IsNullOrWhiteSpace(request.ReferenceNo)
                ? null
                : request.ReferenceNo.Trim(),
            ProofFileAttachmentID = request.ProofFileAttachmentID,
            Status = CustomerPaymentStatus.Pending,
            ReceivedByUserID = userID
        };

        await _db.AddAsync(payment, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return payment.PaymentID;
    }

    public async Task ConfirmAsync(
        int id,
        ConfirmPaymentRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var userID = _currentUser.UserID
            ?? throw new ForbiddenBusinessActionException(
                "Authenticated user is required.");

        await _db.ExecuteInTransactionAsync(async ct =>
        {
            var payment = await OrderFulfilmentServiceHelper.RequireAsync(
                _db.Payments.Where(x => x.PaymentID == id),
                "Payment",
                ct);

            if (payment.Status != CustomerPaymentStatus.Pending)
                throw new BusinessRuleException(
                    "Only a pending payment can be confirmed.");

            if (request.Allocations.Count == 0 ||
                request.Allocations.Any(x => x.Amount <= 0) ||
                request.Allocations.GroupBy(x => x.InvoiceID).Any(x => x.Count() > 1))
            {
                throw new BusinessRuleException("Payment allocations are invalid.");
            }

            if (request.Allocations.Sum(x => x.Amount) > payment.Amount)
                throw new BusinessRuleException(
                    BusinessRuleMessages.AllocationLimitExceeded);

            foreach (var input in request.Allocations)
            {
                var invoice = await OrderFulfilmentServiceHelper.RequireAsync(
                    _db.Invoices.Where(x =>
                        x.InvoiceID == input.InvoiceID &&
                        x.ClientID == payment.ClientID &&
                        x.Status != InvoiceStatus.Cancelled),
                    "Active invoice",
                    ct);

                if (input.Amount > invoice.DueAmount)
                    throw new BusinessRuleException(
                        BusinessRuleMessages.AllocationLimitExceeded);

                await _db.AddAsync(
                    new PaymentAllocation
                    {
                        PaymentID = payment.PaymentID,
                        InvoiceID = invoice.InvoiceID,
                        AllocationType = PaymentAllocationType.Allocation,
                        AllocatedAmount = input.Amount,
                        AllocatedAt = _clock.UtcNow,
                        AllocatedByUserID = userID
                    },
                    ct);

                invoice.PaidAmount += input.Amount;
                invoice.DueAmount -= input.Amount;
                invoice.Status = invoice.DueAmount == 0
                    ? InvoiceStatus.Paid
                    : InvoiceStatus.PartiallyPaid;

                await OrderFulfilmentServiceHelper.UpdateClientDueAsync(
                    _db,
                    payment.ClientID,
                    -input.Amount,
                    ct);
            }

            payment.Status = CustomerPaymentStatus.Confirmed;
            await _db.SaveChangesAsync(ct);
            return true;
        }, cancellationToken);
    }

    public async Task ReverseAllocationAsync(
        ReversePaymentAllocationRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var userID = _currentUser.UserID
            ?? throw new ForbiddenBusinessActionException(
                "Authenticated user is required.");

        await _db.ExecuteInTransactionAsync(async ct =>
        {
            var source = await OrderFulfilmentServiceHelper.RequireAsync(
                _db.PaymentAllocations.Where(x =>
                    x.PaymentAllocationID == request.PaymentAllocationID),
                "Payment allocation",
                ct);

            if (source.AllocationType != PaymentAllocationType.Allocation)
                throw new BusinessRuleException(
                    "Only an allocation can be reversed.");

            if (await _db.PaymentAllocations.AnyAsync(
                    x => x.ReversalOfPaymentAllocationID == source.PaymentAllocationID,
                    ct))
            {
                throw new BusinessRuleException(
                    BusinessRuleMessages.AllocationAlreadyReversed);
            }

            var invoice = await _db.Invoices.SingleAsync(
                x => x.InvoiceID == source.InvoiceID,
                ct);

            if (invoice.Status == InvoiceStatus.Cancelled)
                throw new BusinessRuleException(
                    "An allocation on a cancelled invoice cannot be reversed here.");

            await _db.AddAsync(
                new PaymentAllocation
                {
                    PaymentID = source.PaymentID,
                    InvoiceID = source.InvoiceID,
                    AllocationType = PaymentAllocationType.Reversal,
                    AllocatedAmount = source.AllocatedAmount,
                    ReversalOfPaymentAllocationID = source.PaymentAllocationID,
                    AllocatedAt = _clock.UtcNow,
                    AllocatedByUserID = userID
                },
                ct);

            invoice.PaidAmount -= source.AllocatedAmount;
            invoice.DueAmount += source.AllocatedAmount;
            invoice.Status = invoice.PaidAmount == 0
                ? InvoiceStatus.Issued
                : InvoiceStatus.PartiallyPaid;

            var payment = await _db.Payments.SingleAsync(
                x => x.PaymentID == source.PaymentID,
                ct);

            await OrderFulfilmentServiceHelper.UpdateClientDueAsync(
                _db,
                payment.ClientID,
                source.AllocatedAmount,
                ct);

            var existingNetAllocation = await _db.PaymentAllocations
    .Where(x => x.PaymentID == payment.PaymentID)
    .SumAsync(x => x.AllocationType == PaymentAllocationType.Allocation
        ? x.AllocatedAmount
        : -x.AllocatedAmount, ct);

            var netAllocationAfterReversal =
                existingNetAllocation - source.AllocatedAmount;

            if (netAllocationAfterReversal <= 0)
            {
                payment.Status = CustomerPaymentStatus.Reversed;
            }

            await _db.SaveChangesAsync(ct);
            return true;
        }, cancellationToken);
    }
}
