using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.OrderFulfilment.DTOs;

public sealed record PaymentAllocationDto(
    int PaymentAllocationID,
    int PaymentID,
    int InvoiceID,
    PaymentAllocationType AllocationType,
    decimal AllocatedAmount,
    int? ReversalOfPaymentAllocationID,
    DateTime AllocatedAt,
    int AllocatedByUserID);

public sealed record PaymentListDto(
    int PaymentID,
    string PaymentNo,
    int ClientID,
    DateTime PaymentDate,
    PaymentMethod PaymentMethod,
    decimal Amount,
    CustomerPaymentStatus Status,
    decimal AllocatedAmount,
    decimal AvailableAmount);

public sealed record PaymentDetailsDto(
    int PaymentID,
    string PaymentNo,
    int ClientID,
    DateTime PaymentDate,
    PaymentMethod PaymentMethod,
    decimal Amount,
    string? ReferenceNo,
    int? ProofFileAttachmentID,
    CustomerPaymentStatus Status,
    int ReceivedByUserID,
    IReadOnlyCollection<PaymentAllocationDto> Allocations);

public sealed class CreatePaymentRequestDto
{
    public string PaymentNo { get; init; } = string.Empty;
    public int ClientID { get; init; }
    public DateTime PaymentDate { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public decimal Amount { get; init; }
    public string? ReferenceNo { get; init; }
    public int? ProofFileAttachmentID { get; init; }
}

public sealed class ConfirmPaymentRequestDto
{
    public IReadOnlyCollection<AllocatePaymentRequestDto> Allocations { get; init; }
        = Array.Empty<AllocatePaymentRequestDto>();
}

public sealed class AllocatePaymentRequestDto
{
    public int InvoiceID { get; init; }
    public decimal Amount { get; init; }
}

public sealed class ReversePaymentAllocationRequestDto
{
    public int PaymentAllocationID { get; init; }
}
