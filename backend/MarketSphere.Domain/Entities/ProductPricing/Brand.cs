using MarketSphere.Domain.Common;

namespace MarketSphere.Domain.Entities.ProductPricing;

public sealed class Brand : SoftDeletableEntity
{
    public int BrandID { get; set; }
    public string BrandCode { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string? OwnerCompanyName { get; set; }
    public bool IsCustomerFacing { get; set; } = true;
    public bool IsActive { get; set; } = true;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
