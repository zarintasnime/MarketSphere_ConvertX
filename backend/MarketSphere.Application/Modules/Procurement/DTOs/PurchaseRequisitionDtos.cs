using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.Procurement.DTOs;

public sealed record PurchaseRequisitionItemInputDto(int SKUID, decimal RequestedQuantity, decimal? EstimatedUnitCost, string? Note);
public sealed record SavePurchaseRequisitionRequestDto(string PurchaseRequisitionNo, int BranchID, int RequestedByEmployeeID, DateTime RequiredDate, string? Reason, IReadOnlyCollection<PurchaseRequisitionItemInputDto> Items);
public sealed record PurchaseRequisitionItemDto(int PurchaseRequisitionItemID, int SKUID, string SKUCode, string SKUName, decimal RequestedQuantity, decimal? EstimatedUnitCost, string? Note);
public sealed record PurchaseRequisitionListDto(int PurchaseRequisitionID, string PurchaseRequisitionNo, string BranchName, string RequestedBy, DateTime RequiredDate, PurchaseRequisitionStatus Status, decimal EstimatedAmount);
public sealed record PurchaseRequisitionDetailsDto(int PurchaseRequisitionID, string PurchaseRequisitionNo, int BranchID, int RequestedByEmployeeID, DateTime RequiredDate, string? Reason, PurchaseRequisitionStatus Status, IReadOnlyCollection<PurchaseRequisitionItemDto> Items);
public sealed record ChangePurchaseRequisitionStatusRequestDto(PurchaseRequisitionStatus Status, string? Note);
