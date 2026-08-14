using MarketSphere.Domain.Common;

namespace MarketSphere.Domain.Entities.OrganizationSecurity;

public class Area : SoftDeletableEntity
{
    public int AreaID { get; set; }
    public int RegionID { get; set; }
    public string AreaCode { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Region Region { get; set; } = null!;
    public ICollection<Territory> Territories { get; set; } = new List<Territory>();
}
