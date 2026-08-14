using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.Inventory.DTOs;

public sealed record StockTransferItemInputDto(
    int SKUID,
    int? BatchID,
    decimal RequestedQuantity);

public sealed record SaveStockTransferRequestDto(
    string StockTransferNo,
    int FromWarehouseID,
    int ToWarehouseID,
    DateTime RequestedAt,
    IReadOnlyCollection<StockTransferItemInputDto> Items);

public sealed record StockTransferItemDto(
    int StockTransferItemID,
    int SKUID,
    string SKUCode,
    int? BatchID,
    string? BatchNo,
    decimal RequestedQuantity,
    decimal DispatchedQuantity,
    decimal ReceivedQuantity);

public sealed record StockTransferListDto(
    int StockTransferID,
    string StockTransferNo,
    string FromWarehouse,
    string ToWarehouse,
    DateTime RequestedAt,
    StockTransferStatus Status);

public sealed record StockTransferDetailsDto(
    int StockTransferID,
    string StockTransferNo,
    int FromWarehouseID,
    int ToWarehouseID,
    DateTime RequestedAt,
    DateTime? DispatchedAt,
    DateTime? ReceivedAt,
    StockTransferStatus Status,
    int? ApprovalRequestID,
    IReadOnlyCollection<StockTransferItemDto> Items);

public sealed record DispatchStockTransferItemDto(
    int StockTransferItemID,
    decimal DispatchedQuantity);

public sealed record DispatchStockTransferRequestDto(
    IReadOnlyCollection<DispatchStockTransferItemDto> Items,
    string? Note);

public sealed record ReceiveStockTransferItemDto(
    int StockTransferItemID,
    decimal ReceivedQuantity);

public sealed record ReceiveStockTransferRequestDto(
    IReadOnlyCollection<ReceiveStockTransferItemDto> Items,
    string? Note);
