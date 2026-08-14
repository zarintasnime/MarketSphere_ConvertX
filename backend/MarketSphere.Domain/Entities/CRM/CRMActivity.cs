using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.CRM;

public sealed class CRMActivity : AuditableEntity
{
    public int CRMActivityID { get; set; }
    public int? LeadID { get; set; }
    public int? ClientID { get; set; }
    public int? OpportunityID { get; set; }
    public CrmActivityType ActivityType { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime ActivityAt { get; set; }
    public DateTime? ScheduledStartAt { get; set; }
    public DateTime? ScheduledEndAt { get; set; }
    public string? LocationOrMeetingLink { get; set; }
    public string? Agenda { get; set; }
    public CrmActivityStatus ActivityStatus { get; set; } = CrmActivityStatus.Planned;
    public string? Outcome { get; set; }
    public DateTime? NextActionAt { get; set; }
    public int? PerformedByEmployeeID { get; set; }

    public Lead? Lead { get; set; }
    public Client? Client { get; set; }
    public Opportunity? Opportunity { get; set; }
    public Employee? PerformedByEmployee { get; set; }
    public ICollection<CRMActivityParticipant> Participants { get; set; } = new List<CRMActivityParticipant>();
}
