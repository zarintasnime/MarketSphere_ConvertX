using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.MarketingField;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.CRM;

public sealed class Opportunity : AuditableEntity
{
    public int OpportunityID { get; set; }
    public string OpportunityCode { get; set; } = string.Empty;
    public int? LeadID { get; set; }
    public int? ClientID { get; set; }
    public int? CampaignID { get; set; }
    public int OwnerEmployeeID { get; set; }
    public string OpportunityName { get; set; } = string.Empty;
    public OpportunityStage Stage { get; set; } = OpportunityStage.Qualified;
    public decimal ExpectedValue { get; set; }
    public int ProbabilityPercent { get; set; }
    public DateOnly? ExpectedCloseDate { get; set; }
    public string? Competitor { get; set; }
    public string? LostReason { get; set; }
    public DateTime? WonAt { get; set; }

    public Lead? Lead { get; set; }
    public Client? Client { get; set; }
    public Employee OwnerEmployee { get; set; } = null!;
    public ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
    public Campaign? Campaign { get; set; }
}
