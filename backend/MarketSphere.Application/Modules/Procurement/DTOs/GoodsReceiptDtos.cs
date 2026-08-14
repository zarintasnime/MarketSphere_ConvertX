using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.Procurement.DTOs;

public sealed record GoodsReceiptItemInputDto(
    int PurchaseOrderItemID,
    int SKUID,
    decimal AcceptedQuantity,
    decimal RejectedQuantity,
    string? BatchNo,
    DateTime? ManufacturingDate,
    DateTime? ExpiryDate,
    decimal UnitCost,
    string? RejectionReason);

public sealed record SaveGoodsReceiptRequestDto(
    string GoodsReceiptNo,
    int PurchaseOrderID,
    int WarehouseID,
    DateTime ReceivedDate,
    int ReceivedByEmployeeID,
    string? SupplierChallanNo,
    IReadOnlyCollection<GoodsReceiptItemInputDto> Items);

public sealed record GoodsReceiptItemDto(
    int GoodsReceiptItemID,
    int PurchaseOrderItemID,
    int SKUID,
    string SKUCode,
    decimal AcceptedQuantity,
    decimal RejectedQuantity,
    string? BatchNo,
    DateTime? ExpiryDate,
    decimal UnitCost,
    int? BatchID,
    string? RejectionReason);

public sealed record GoodsReceiptListDto(
    int GoodsReceiptID,
    string GoodsReceiptNo,
    string PurchaseOrderNo,
    string WarehouseName,
    DateTime ReceivedDate,
    GoodsReceiptStatus Status,
    QualityCheckStatus QualityCheckStatus);

public sealed record GoodsReceiptDetailsDto(
    int GoodsReceiptID,
    string GoodsReceiptNo,
    int PurchaseOrderID,
    int WarehouseID,
    DateTime ReceivedDate,
    int ReceivedByEmployeeID,
    string? SupplierChallanNo,
    GoodsReceiptStatus Status,
    QualityCheckStatus QualityCheckStatus,
    IReadOnlyCollection<GoodsReceiptItemDto> Items);

public sealed record CompleteQualityCheckRequestDto(
    QualityCheckStatus QualityCheckStatus);

public sealed record PostGoodsReceiptRequestDto(string? Note);
