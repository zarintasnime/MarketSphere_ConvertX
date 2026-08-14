using MarketSphere.Domain.Common;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrganizationSecurity;

public class Employee : AuditableEntity, IHasStatus<EmployeeStatus>
{
    public int EmployeeID { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public int? UserID { get; set; }
    public int DesignationID { get; set; }
    public int? ManagerEmployeeID { get; set; }
    public int BranchID { get; set; }
    public int? RegionID { get; set; }
    public int? AreaID { get; set; }
    public int? TerritoryID { get; set; }
    public DateOnly JoiningDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public User? User { get; set; }
    public Designation Designation { get; set; } = null!;
    public Employee? Manager { get; set; }
    public ICollection<Employee> DirectReports { get; set; } = new List<Employee>();
    public Branch Branch { get; set; } = null!;
    public Region? Region { get; set; }
    public Area? Area { get; set; }
    public Territory? Territory { get; set; }
    public ICollection<EmployeeRouteAssignment> RouteAssignments { get; set; } = new List<EmployeeRouteAssignment>();
    public ICollection<EmployeeTerritoryAssignment> TerritoryAssignments { get; set; } = new List<EmployeeTerritoryAssignment>();
}
