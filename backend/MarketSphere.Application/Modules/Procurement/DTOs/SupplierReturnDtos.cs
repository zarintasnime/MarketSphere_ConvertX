using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.Procurement.DTOs;

public sealed record SupplierReturnItemInputDto(
    int SKUID,
    int? BatchID,
    decimal Quantity,
    decimal UnitCost,
    string Reason);

public sealed record SaveSupplierReturnRequestDto(
    string SupplierReturnNo,
    int SupplierID,
    int? GoodsReceiptID,
    int WarehouseID,
    DateTime ReturnDate,
    string Reason,
    IReadOnlyCollection<SupplierReturnItemInputDto> Items);

public sealed record SupplierReturnItemDto(
    int SupplierReturnItemID,
    int SKUID,
    string SKUCode,
    int? BatchID,
    decimal Quantity,
    decimal UnitCost,
    string Reason,
    int? StockMovementID);

public sealed record SupplierReturnListDto(
    int SupplierReturnID,
    string SupplierReturnNo,
    string SupplierName,
    string WarehouseName,
    DateTime ReturnDate,
    SupplierReturnStatus Status);

public sealed record SupplierReturnDetailsDto(
    int SupplierReturnID,
    string SupplierReturnNo,
    int SupplierID,
    int? GoodsReceiptID,
    int WarehouseID,
    DateTime ReturnDate,
    string Reason,
    SupplierReturnStatus Status,
    IReadOnlyCollection<SupplierReturnItemDto> Items);

public sealed record ChangeSupplierReturnStatusRequestDto(
    SupplierReturnStatus Status);

public sealed record PostSupplierReturnRequestDto(string? Note);
