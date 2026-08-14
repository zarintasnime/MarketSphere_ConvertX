using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.ProductPricing;

public sealed class StandardDiscountRule : AuditableEntity
{
    public int StandardDiscountRuleID { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public SalesChannel Channel { get; set; }
    public int? ClientSegmentID { get; set; }
    public int? SKUID { get; set; }
    public int? ProductCategoryID { get; set; }
    public decimal? MinQuantity { get; set; }
    public decimal MaxDiscountPercent { get; set; }
    public bool RequiresApproval { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;

    public ClientSegment? ClientSegment { get; set; }
    public SKU? SKU { get; set; }
    public ProductCategory? ProductCategory { get; set; }
}
