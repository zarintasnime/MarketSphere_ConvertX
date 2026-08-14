using MarketSphere.Domain.Common;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrganizationSecurity;

public class EmployeeRouteAssignment : AuditableEntity, IHasStatus<AssignmentStatus>
{
    public int EmployeeRouteAssignmentID { get; set; }
    public int EmployeeID { get; set; }
    public int RouteID { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
    public bool IsPrimary { get; set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Active;

    public Employee Employee { get; set; } = null!;
    public Route Route { get; set; } = null!;
}
