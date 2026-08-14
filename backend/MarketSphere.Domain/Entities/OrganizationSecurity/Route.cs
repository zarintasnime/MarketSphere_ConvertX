using MarketSphere.Domain.Common;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrganizationSecurity;

public class Route : SoftDeletableEntity
{
    public int RouteID { get; set; }
    public int TerritoryID { get; set; }
    public string RouteCode { get; set; } = string.Empty;
    public string RouteName { get; set; } = string.Empty;
    public DayOfWeek? DayOfWeek { get; set; }
    public VisitFrequency VisitFrequency { get; set; }
    public bool IsActive { get; set; } = true;

    public Territory Territory { get; set; } = null!;
    public ICollection<RouteOutlet> RouteOutlets { get; set; } = new List<RouteOutlet>();
    public ICollection<EmployeeRouteAssignment> EmployeeAssignments { get; set; } = new List<EmployeeRouteAssignment>();
}
