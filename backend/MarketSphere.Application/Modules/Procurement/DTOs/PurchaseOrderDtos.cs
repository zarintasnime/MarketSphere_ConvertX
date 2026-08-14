using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.Procurement.DTOs;

public sealed record PurchaseOrderItemInputDto(int SKUID, decimal OrderedQuantity, decimal UnitCost, decimal DiscountAmount, decimal TaxAmount);
public sealed record SavePurchaseOrderRequestDto(string PurchaseOrderNo, int SupplierID, int? PurchaseRequisitionID, int BranchID, DateTime OrderDate, DateTime? ExpectedDeliveryDate, IReadOnlyCollection<PurchaseOrderItemInputDto> Items);
public sealed record PurchaseOrderItemDto(int PurchaseOrderItemID, int SKUID, string SKUCode, string SKUName, decimal OrderedQuantity, decimal ReceivedQuantity, decimal UnitCost, decimal DiscountAmount, decimal TaxAmount, decimal LineTotal);
public sealed record PurchaseOrderListDto(int PurchaseOrderID, string PurchaseOrderNo, string SupplierName, DateTime OrderDate, DateTime? ExpectedDeliveryDate, PurchaseOrderStatus Status, decimal NetAmount);
public sealed record PurchaseOrderDetailsDto(int PurchaseOrderID, string PurchaseOrderNo, int SupplierID, int? PurchaseRequisitionID, int BranchID, DateTime OrderDate, DateTime? ExpectedDeliveryDate, PurchaseOrderStatus Status, decimal GrossAmount, decimal DiscountAmount, decimal TaxAmount, decimal NetAmount, IReadOnlyCollection<PurchaseOrderItemDto> Items);
public sealed record ChangePurchaseOrderStatusRequestDto(PurchaseOrderStatus Status, string? Note);
