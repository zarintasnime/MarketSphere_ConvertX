using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.OrderFulfilment.DTOs;

public sealed record DeliveryItemDto(int DeliveryItemID, int? PickListItemID, int OrderItemID, int? InvoiceItemID, int SKUID, string SKUCode, int? BatchID, decimal QuantityDispatched, decimal QuantityDelivered, decimal QuantityRejectedAtDelivery);
public sealed record DeliveryListDto(int DeliveryID, string DeliveryNo, int OrderID, int? InvoiceID, int? PickListID, int WarehouseID, DateTime? PlannedDeliveryDate, DateTime? DispatchDate, DateTime? DeliveredAt, DeliveryStatus Status);
public sealed record DeliveryDetailsDto(int DeliveryID, string DeliveryNo, int OrderID, int? InvoiceID, int? PickListID, int WarehouseID, DateTime? PlannedDeliveryDate, DateTime? DispatchDate, DateTime? DeliveredAt, DeliveryStatus Status, int? DeliveredByEmployeeID, string? ReceiverName, string? ReceiverPhone, string? FailureReason, DateTime? RescheduledDate, IReadOnlyCollection<DeliveryItemDto> Items);
public sealed class CreateDeliveryRequestDto { public string DeliveryNo { get; init; } = string.Empty; public int OrderID { get; init; } public int? InvoiceID { get; init; } public int PickListID { get; init; } public int WarehouseID { get; init; } public DateTime? PlannedDeliveryDate { get; init; } }
public sealed class DispatchDeliveryRequestDto { public int DeliveredByEmployeeID { get; init; } }
public sealed class CompleteDeliveryRequestDto { public DeliveryStatus Status { get; init; } public string? ReceiverName { get; init; } public string? ReceiverPhone { get; init; } public string? FailureReason { get; init; } public DateTime? RescheduledDate { get; init; } public IReadOnlyCollection<CompleteDeliveryItemRequestDto> Items { get; init; } = Array.Empty<CompleteDeliveryItemRequestDto>(); }
public sealed class CompleteDeliveryItemRequestDto { public int DeliveryItemID { get; init; } public decimal QuantityDelivered { get; init; } public decimal QuantityRejectedAtDelivery { get; init; } }
