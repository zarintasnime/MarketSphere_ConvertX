using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.MarketingField;

public sealed class Feedback : AuditableEntity
{
    public int FeedbackID { get; set; }
    public int? ClientID { get; set; }
    public int? LeadID { get; set; }
    public int? CampaignID { get; set; }
    public int? VisitID { get; set; }
    public int? SubmittedByEmployeeID { get; set; }
    public FeedbackType FeedbackType { get; set; }
    public int? Rating { get; set; }
    public string? Comments { get; set; }
    public DateTime SubmittedAt { get; set; }
    public bool IsFollowUpRequired { get; set; }

    public Client? Client { get; set; }
    public Lead? Lead { get; set; }
    public Campaign? Campaign { get; set; }
    public Visit? Visit { get; set; }
    public Employee? SubmittedByEmployee { get; set; }
}
