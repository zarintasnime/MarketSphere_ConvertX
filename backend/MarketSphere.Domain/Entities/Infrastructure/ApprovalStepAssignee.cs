using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Infrastructure;

public sealed class ApprovalStepAssignee : AuditableEntity
{
    public int ApprovalStepAssigneeID { get; set; }
    public int ApprovalStepDefinitionID { get; set; }
    public ApprovalAssigneeType AssigneeType { get; set; }
    public int? RoleID { get; set; }
    public int? DesignationID { get; set; }
    public int? UserID { get; set; }
    public int? EmployeeID { get; set; }
    public bool IsFallback { get; set; }
    public int Priority { get; set; }

    public ApprovalStepDefinition ApprovalStepDefinition { get; set; } = null!;
    public Role? Role { get; set; }
    public Designation? Designation { get; set; }
    public User? User { get; set; }
    public Employee? Employee { get; set; }
}
