using MarketSphere.Domain.Common;

namespace MarketSphere.Domain.Entities.ProductPricing;

public sealed class PriceListItem : AuditableEntity
{
    public int PriceListItemID { get; set; }
    public int PriceListID { get; set; }
    public int SKUID { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal MaximumDiscountPercent { get; set; }
    public decimal? MinimumOrderQuantity { get; set; }

    public PriceList PriceList { get; set; } = null!;
    public SKU SKU { get; set; } = null!;
}
