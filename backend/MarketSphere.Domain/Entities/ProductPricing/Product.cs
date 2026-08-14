using MarketSphere.Domain.Common;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.ProductPricing;

public sealed class Product : SoftDeletableEntity
{
    public int ProductID { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public int ProductCategoryID { get; set; }
    public int BrandID { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public ProductType ProductType { get; set; } = ProductType.FinishedGood;
    public string? Description { get; set; }
    public bool RequiresBatch { get; set; }
    public bool RequiresExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;

    public ProductCategory ProductCategory { get; set; } = null!;
    public Brand Brand { get; set; } = null!;
    public ICollection<SKU> SKUs { get; set; } = new List<SKU>();
}
