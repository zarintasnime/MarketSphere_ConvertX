using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.OrderFulfilment.DTOs;

public sealed record AppliedOfferDto(
    int AppliedOfferID,
    int? QuotationID,
    int? QuotationItemID,
    int? OrderID,
    int? OrderItemID,
    int CampaignOfferID,
    AppliedBenefitType BenefitType,
    decimal? BenefitAmount,
    int? FreeSKUID,
    decimal? FreeQuantity,
    string RuleSnapshotJson,
    int UsageCount,
    DateTime AppliedAt,
    int? AppliedByUserID);

public sealed class ApplyOfferRequestDto
{
    public int? QuotationID { get; init; }
    public int? QuotationItemID { get; init; }
    public int? OrderID { get; init; }
    public int? OrderItemID { get; init; }
    public int CampaignOfferID { get; init; }
    public AppliedBenefitType BenefitType { get; init; }
    public decimal? BenefitAmount { get; init; }
    public int? FreeSKUID { get; init; }
    public decimal? FreeQuantity { get; init; }
    public string RuleSnapshotJson { get; init; } = "{}";
    public int UsageCount { get; init; } = 1;
}
