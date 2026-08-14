using MarketSphere.Domain.Common;

namespace MarketSphere.Domain.Entities.OrganizationSecurity;

public class Company : SoftDeletableEntity
{
    public int CompanyID { get; set; }
    public string CompanyCode { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? TradeLicenseNo { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Branch> Branches { get; set; } = new List<Branch>();
    public ICollection<Region> Regions { get; set; } = new List<Region>();
}
