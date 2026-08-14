using MarketSphere.Domain.Common;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.ProductPricing;

public sealed class ProductCategory : SoftDeletableEntity
{
    public int ProductCategoryID { get; set; }
    public int? ParentProductCategoryID { get; set; }
    public string CategoryCode { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public ProductCategoryType CategoryType { get; set; } = ProductCategoryType.Standard;
    public bool IsActive { get; set; } = true;

    public ProductCategory? ParentProductCategory { get; set; }
    public ICollection<ProductCategory> Children { get; set; } = new List<ProductCategory>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
