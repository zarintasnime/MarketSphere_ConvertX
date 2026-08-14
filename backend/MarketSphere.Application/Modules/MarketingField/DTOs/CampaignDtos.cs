using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.MarketingField.DTOs;

public sealed record CampaignListDto(int CampaignID, string CampaignCode, string CampaignTitle, decimal Budget, decimal ActualExpense, DateOnly StartDate, DateOnly EndDate, SalesChannel Channel, CampaignStatus Status, int CreatedByEmployeeID);
public sealed record CampaignDetailsDto(int CampaignID, string CampaignCode, string CampaignTitle, string Objective, decimal Budget, decimal ActualExpense, DateOnly StartDate, DateOnly EndDate, SalesChannel Channel, CampaignStatus Status, int CreatedByEmployeeID, IReadOnlyCollection<CampaignTargetDto> Targets, IReadOnlyCollection<CampaignOfferDto> Offers, IReadOnlyCollection<CampaignExpenseDto> Expenses, IReadOnlyCollection<CampaignAttributionDto> Attributions);
public sealed class SaveCampaignRequestDto { public string CampaignCode { get; init; } = string.Empty; public string CampaignTitle { get; init; } = string.Empty; public string Objective { get; init; } = string.Empty; public decimal Budget { get; init; } public DateOnly StartDate { get; init; } public DateOnly EndDate { get; init; } public SalesChannel Channel { get; init; } public int CreatedByEmployeeID { get; init; } }
public sealed class ChangeCampaignStatusRequestDto { public CampaignStatus Status { get; init; } public string? Reason { get; init; } }

public sealed record CampaignTargetDto(int CampaignTargetID, int CampaignID, CampaignTargetType TargetType, int? RegionID, int? AreaID, int? ClientSegmentID, int? ClientID, int? ProductCategoryID, int? SKUID, decimal? TargetValue);
public sealed class SaveCampaignTargetRequestDto { public CampaignTargetType TargetType { get; init; } public int? RegionID { get; init; } public int? AreaID { get; init; } public int? ClientSegmentID { get; init; } public int? ClientID { get; init; } public int? ProductCategoryID { get; init; } public int? SKUID { get; init; } public decimal? TargetValue { get; init; } }

public sealed record CampaignOfferDto(int CampaignOfferID, int CampaignID, string OfferCode, CampaignOfferType OfferType, string RuleJson, decimal? DiscountValue, int? FreeSKUID, int Priority, int? UsageLimit, int? PerClientLimit, bool IsStackable, bool IsActive);
public sealed class SaveCampaignOfferRequestDto { public string OfferCode { get; init; } = string.Empty; public CampaignOfferType OfferType { get; init; } public string RuleJson { get; init; } = "{}"; public decimal? DiscountValue { get; init; } public int? FreeSKUID { get; init; } public int Priority { get; init; } public int? UsageLimit { get; init; } public int? PerClientLimit { get; init; } public bool IsStackable { get; init; } public bool IsActive { get; init; } = true; }

public sealed record CampaignExpenseDto(int CampaignExpenseID, int CampaignID, DateOnly ExpenseDate, string ExpenseCategory, decimal Amount, string? VendorName, string? Description, CampaignExpenseStatus Status);
public sealed class SaveCampaignExpenseRequestDto { public DateOnly ExpenseDate { get; init; } public string ExpenseCategory { get; init; } = string.Empty; public decimal Amount { get; init; } public string? VendorName { get; init; } public string? Description { get; init; } }
public sealed class ChangeCampaignExpenseStatusRequestDto { public CampaignExpenseStatus Status { get; init; } }

public sealed record CampaignAttributionDto(int CampaignAttributionID, int CampaignID, int? LeadID, int? OpportunityID, int? QuotationID, int? OrderID, CampaignAttributionType AttributionType, decimal WeightPercent, decimal? AttributedAmount);
public sealed class SaveCampaignAttributionRequestDto { public int? LeadID { get; init; } public int? OpportunityID { get; init; } public int? QuotationID { get; init; } public int? OrderID { get; init; } public CampaignAttributionType AttributionType { get; init; } public decimal WeightPercent { get; init; } public decimal? AttributedAmount { get; init; } }
public sealed record CampaignRoiDto(int CampaignID, decimal Budget, decimal ActualExpense, decimal AttributedAmount, decimal RoiAmount, decimal? RoiPercent);
