using MarketSphere.Domain.Common;

namespace MarketSphere.Domain.Entities.OrganizationSecurity;

public class Territory : SoftDeletableEntity
{
    public int TerritoryID { get; set; }
    public int AreaID { get; set; }
    public string TerritoryCode { get; set; } = string.Empty;
    public string TerritoryName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Area Area { get; set; } = null!;
    public ICollection<Route> Routes { get; set; } = new List<Route>();
}
