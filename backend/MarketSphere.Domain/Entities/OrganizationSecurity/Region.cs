using MarketSphere.Domain.Common;

namespace MarketSphere.Domain.Entities.OrganizationSecurity;

public class Region : SoftDeletableEntity
{
    public int RegionID { get; set; }
    public int CompanyID { get; set; }
    public string RegionCode { get; set; } = string.Empty;
    public string RegionName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;
    public ICollection<Area> Areas { get; set; } = new List<Area>();
}
