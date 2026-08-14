using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.ProductPricing;

namespace MarketSphere.Domain.Entities.MarketingField;

public sealed class BPSellOutItem : AuditableEntity
{
    public int BPSellOutItemID { get; set; }
    public int BPSellOutID { get; set; }
    public int SKUID { get; set; }
    public decimal QuantitySold { get; set; }
    public decimal? UnitSellingPrice { get; set; }
    public decimal? LineValue { get; set; }

    public BPSellOut BPSellOut { get; set; } = null!;
    public SKU SKU { get; set; } = null!;
}
