using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Entities.ProductPricing;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.MarketingField;

public sealed class CampaignTarget : AuditableEntity
{
    public int CampaignTargetID { get; set; }
    public int CampaignID { get; set; }
    public CampaignTargetType TargetType { get; set; }
    public int? RegionID { get; set; }
    public int? AreaID { get; set; }
    public int? ClientSegmentID { get; set; }
    public int? ClientID { get; set; }
    public int? ProductCategoryID { get; set; }
    public int? SKUID { get; set; }
    public decimal? TargetValue { get; set; }

    public Campaign Campaign { get; set; } = null!;
    public Region? Region { get; set; }
    public Area? Area { get; set; }
    public ClientSegment? ClientSegment { get; set; }
    public Client? Client { get; set; }
    public ProductCategory? ProductCategory { get; set; }
    public SKU? SKU { get; set; }
}
