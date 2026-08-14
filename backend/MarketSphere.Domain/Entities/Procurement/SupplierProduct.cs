using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.ProductPricing;

namespace MarketSphere.Domain.Entities.Procurement;

public sealed class SupplierProduct : AuditableEntity
{
    public int SupplierProductID { get; set; }
    public int SupplierID { get; set; }
    public int SKUID { get; set; }
    public string? SupplierSKUCode { get; set; }
    public decimal? LastPurchasePrice { get; set; }
    public decimal? MinimumOrderQuantity { get; set; }
    public int? LeadTimeDays { get; set; }
    public bool IsPreferredSupplier { get; set; }
    public bool IsActive { get; set; } = true;

    public Supplier Supplier { get; set; } = null!;
    public SKU SKU { get; set; } = null!;
}
