using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.CRM;

public sealed class CRMTask : AuditableEntity
{
    public int CRMTaskID { get; set; }
    public int? LeadID { get; set; }
    public int? ClientID { get; set; }
    public int? OpportunityID { get; set; }
    public int? ComplaintID { get; set; }
    public int? ReactivationCaseID { get; set; }
    public int AssignedEmployeeID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Normal;
    public DateTime DueAt { get; set; }
    public DateTime? ReminderAt { get; set; }
    public string? RecurrenceRule { get; set; }
    public CrmTaskStatus Status { get; set; } = CrmTaskStatus.Open;
    public DateTime? CompletedAt { get; set; }
    public DateTime? EscalatedAt { get; set; }

    public Lead? Lead { get; set; }
    public Client? Client { get; set; }
    public Opportunity? Opportunity { get; set; }
    public Complaint? Complaint { get; set; }
    public ReactivationCase? ReactivationCase { get; set; }
    public Employee AssignedEmployee { get; set; } = null!;
}
