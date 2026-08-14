using MarketSphere.Domain.Common;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrganizationSecurity;

public class EmployeeTerritoryAssignment : AuditableEntity
{
    public int EmployeeTerritoryAssignmentID { get; set; }
    public int EmployeeID { get; set; }
    public GeographyScopeType ScopeType { get; set; }
    public int? RegionID { get; set; }
    public int? AreaID { get; set; }
    public int? TerritoryID { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsPrimary { get; set; }

    public Employee Employee { get; set; } = null!;
    public Region? Region { get; set; }
    public Area? Area { get; set; }
    public Territory? Territory { get; set; }
}
