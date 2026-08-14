using MarketSphere.Domain.Common;

namespace MarketSphere.Domain.Entities.ProductPricing;

public sealed class SKU : SoftDeletableEntity
{
    public int SKUID { get; set; }
    public int ProductID { get; set; }
    public string SKUCode { get; set; } = string.Empty;
    public string SKUName { get; set; } = string.Empty;
    public string? Size { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public decimal MRP { get; set; }
    public decimal StandardTradePrice { get; set; }
    public bool IsActive { get; set; } = true;

    public Product Product { get; set; } = null!;
    public ICollection<PriceListItem> PriceListItems { get; set; } = new List<PriceListItem>();
}
