using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.Inventory.DTOs;

public sealed record StockAdjustmentItemInputDto(
    int SKUID,
    int? BatchID,
    decimal AdjustmentQuantity,
    decimal? UnitCost,
    string? Note);

public sealed record SaveStockAdjustmentRequestDto(
    string StockAdjustmentNo,
    int WarehouseID,
    DateTime AdjustmentDate,
    string Reason,
    int PerformedByEmployeeID,
    IReadOnlyCollection<StockAdjustmentItemInputDto> Items);

public sealed record StockAdjustmentItemDto(
    int StockAdjustmentItemID,
    int SKUID,
    string SKUCode,
    int? BatchID,
    decimal AdjustmentQuantity,
    decimal? UnitCost,
    string? Note,
    int? StockMovementID);

public sealed record StockAdjustmentListDto(
    int StockAdjustmentID,
    string StockAdjustmentNo,
    string WarehouseName,
    DateTime AdjustmentDate,
    string Reason,
    StockAdjustmentStatus Status);

public sealed record StockAdjustmentDetailsDto(
    int StockAdjustmentID,
    string StockAdjustmentNo,
    int WarehouseID,
    DateTime AdjustmentDate,
    string Reason,
    StockAdjustmentStatus Status,
    int PerformedByEmployeeID,
    IReadOnlyCollection<StockAdjustmentItemDto> Items);

public sealed record ChangeStockAdjustmentStatusRequestDto(
    StockAdjustmentStatus Status);

public sealed record PostStockAdjustmentRequestDto(string? Note);
