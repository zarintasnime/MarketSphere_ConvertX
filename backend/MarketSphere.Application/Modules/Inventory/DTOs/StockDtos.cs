using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.Inventory.DTOs;

public sealed record BatchDto(int BatchID, int SKUID, string SKUCode, string BatchNo, DateTime? ManufacturingDate, DateTime? ExpiryDate, DateTime? BestBeforeDate, decimal CostPrice, BatchStatus Status);
public sealed record StockBalanceDto(int StockBalanceID, int WarehouseID, string WarehouseName, int SKUID, string SKUCode, string SKUName, int? BatchID, string? BatchNo, DateTime? ExpiryDate, decimal OnHandQuantity, decimal ReservedQuantity, decimal QuarantineQuantity, decimal DamagedQuantity, decimal AvailableQuantity, byte[] RowVersion);
public sealed record StockMovementDto(int StockMovementID, int WarehouseID, string WarehouseName, int SKUID, string SKUCode, string SKUName, int? BatchID, string? BatchNo, StockMovementType MovementType, decimal QuantityIn, decimal QuantityOut, decimal BalanceAfter, string ReferenceType, int ReferenceID, DateTime MovementAt, string? Note);
public sealed record StockReservationDto(int StockReservationID, int OrderItemID, int WarehouseID, string WarehouseName, int SKUID, string SKUCode, string SKUName, int? BatchID, string? BatchNo, decimal ReservedQuantity, StockReservationStatus ReservationStatus, DateTime ReservedAt, DateTime? ExpiresAt, DateTime? ReleasedAt);
public sealed record StockSearchRequestDto(int? WarehouseID, int? SKUID, int? BatchID, bool IncludeZero, bool IncludeExpired);
