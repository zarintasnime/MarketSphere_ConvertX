using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Entities.MarketingField;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Entities.ProductPricing;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrderFulfilment;

public sealed class AppliedOffer : AuditableEntity
{
    public int AppliedOfferID { get; set; }
    public int? QuotationID { get; set; }
    public int? QuotationItemID { get; set; }
    public int? OrderID { get; set; }
    public int? OrderItemID { get; set; }
    public int CampaignOfferID { get; set; }
    public AppliedBenefitType BenefitType { get; set; }
    public decimal? BenefitAmount { get; set; }
    public int? FreeSKUID { get; set; }
    public decimal? FreeQuantity { get; set; }
    public string RuleSnapshotJson { get; set; } = "{}";
    public int UsageCount { get; set; } = 1;
    public DateTime AppliedAt { get; set; }
    public int? AppliedByUserID { get; set; }

    public Quotation? Quotation { get; set; }
    public QuotationItem? QuotationItem { get; set; }
    public Order? Order { get; set; }
    public OrderItem? OrderItem { get; set; }
    public CampaignOffer CampaignOffer { get; set; } = null!;
    public SKU? FreeSKU { get; set; }
    public User? AppliedByUser { get; set; }
}
