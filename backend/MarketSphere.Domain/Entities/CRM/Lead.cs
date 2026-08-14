using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.MarketingField;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.CRM;

public sealed class Lead : AuditableEntity
{
    public int LeadID { get; set; }
    public string LeadCode { get; set; } = string.Empty;
    public string LeadName { get; set; } = string.Empty;
    public string? BusinessName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public LeadSource Source { get; set; }
    public int? SourceCampaignID { get; set; }
    public int? AssignedEmployeeID { get; set; }
    public int? RegionID { get; set; }
    public string? ProductInterest { get; set; }
    public decimal? EstimatedValue { get; set; }
    public int CurrentScore { get; set; }
    public LeadTemperature Temperature { get; set; } = LeadTemperature.Cold;
    public LeadStatus Status { get; set; } = LeadStatus.New;
    public DateTime? NextFollowUpAt { get; set; }
    public string? LostReason { get; set; }
    public DateTime? ReactivationAt { get; set; }
    public int? ConvertedClientID { get; set; }

    public Employee? AssignedEmployee { get; set; }
    public Region? Region { get; set; }
    public Client? ConvertedClient { get; set; }
    public Campaign? SourceCampaign { get; set; }
}
