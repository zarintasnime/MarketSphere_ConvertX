using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.ProductPricing;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.MarketingField;

public sealed class CampaignOffer : AuditableEntity
{
    public int CampaignOfferID { get; set; }
    public int CampaignID { get; set; }
    public string OfferCode { get; set; } = string.Empty;
    public CampaignOfferType OfferType { get; set; }
    public string RuleJson { get; set; } = "{}";
    public decimal? DiscountValue { get; set; }
    public int? FreeSKUID { get; set; }
    public int Priority { get; set; }
    public int? UsageLimit { get; set; }
    public int? PerClientLimit { get; set; }
    public bool IsStackable { get; set; }
    public bool IsActive { get; set; } = true;

    public Campaign Campaign { get; set; } = null!;
    public SKU? FreeSKU { get; set; }
}
