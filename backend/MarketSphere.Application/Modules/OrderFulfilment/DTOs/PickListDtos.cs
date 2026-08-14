using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.OrderFulfilment.DTOs;

public sealed record PickListItemDto(int PickListItemID, int OrderItemID, int? StockReservationID, int SKUID, string SKUCode, int? BatchID, decimal RequestedQuantity, decimal PickedQuantity, decimal ShortQuantity, int? PickedByEmployeeID, DateTime? PickedAt, string? VerificationNote);
public sealed record PickListListDto(int PickListID, string PickListNo, int OrderID, int? InvoiceID, int WarehouseID, string? WaveNo, PickListStatus Status, DateTime? ReleasedAt, DateTime? VerifiedAt);
public sealed record PickListDetailsDto(int PickListID, string PickListNo, int OrderID, int? InvoiceID, int WarehouseID, string? WaveNo, PickListStatus Status, DateTime? ReleasedAt, int? ReleasedByEmployeeID, int? VerifiedByEmployeeID, DateTime? VerifiedAt, string? Note, IReadOnlyCollection<PickListItemDto> Items);
public sealed class CreatePickListRequestDto { public string PickListNo { get; init; } = string.Empty; public int OrderID { get; init; } public int? InvoiceID { get; init; } public int WarehouseID { get; init; } public string? WaveNo { get; init; } public string? Note { get; init; } }
public sealed class ReleasePickListRequestDto { public int ReleasedByEmployeeID { get; init; } }
public sealed class RecordPickRequestDto { public int PickListItemID { get; init; } public decimal PickedQuantity { get; init; } public decimal ShortQuantity { get; init; } public int PickedByEmployeeID { get; init; } public string? VerificationNote { get; init; } }
public sealed class VerifyPickListRequestDto { public int VerifiedByEmployeeID { get; init; } public string? Note { get; init; } }
