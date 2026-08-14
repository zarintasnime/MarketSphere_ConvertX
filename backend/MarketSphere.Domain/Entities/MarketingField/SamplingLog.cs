using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Entities.ProductPricing;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.MarketingField;

public sealed class SamplingLog : AuditableEntity
{
    public int SamplingLogID { get; set; }
    public int? VisitID { get; set; }
    public int? CampaignID { get; set; }
    public int? ClientID { get; set; }
    public int? LeadID { get; set; }
    public int EmployeeID { get; set; }
    public int SKUID { get; set; }
    public decimal IssuedQuantity { get; set; }
    public decimal ConsumedQuantity { get; set; }
    public decimal ReturnedQuantity { get; set; }
    public decimal DamagedQuantity { get; set; }
    public DateOnly SampleDate { get; set; }
    public string? FeedbackSummary { get; set; }
    public SamplingOutcome Outcome { get; set; }
    public bool FollowUpRequired { get; set; }
    public int? IssueStockMovementID { get; set; }
    public int? ReturnStockMovementID { get; set; }
    public int? DamageStockMovementID { get; set; }

    public Visit? Visit { get; set; }
    public Campaign? Campaign { get; set; }
    public Client? Client { get; set; }
    public Lead? Lead { get; set; }
    public Employee Employee { get; set; } = null!;
    public SKU SKU { get; set; } = null!;
}
