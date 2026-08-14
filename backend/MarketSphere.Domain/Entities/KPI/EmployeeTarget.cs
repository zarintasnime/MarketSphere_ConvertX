using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Entities.MarketingField;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Entities.ProductPricing;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.KPI;

public sealed class EmployeeTarget : AuditableEntity
{
    public int EmployeeTargetID { get; set; }
    public int EmployeeID { get; set; }
    public DateTime TargetPeriodStart { get; set; }
    public DateTime TargetPeriodEnd { get; set; }
    public TargetType TargetType { get; set; }
    public decimal TargetValue { get; set; }
    public decimal? TargetAmount { get; set; }
    public int? CampaignID { get; set; }
    public int? SKUID { get; set; }
    public int? ClientID { get; set; }
    public EmployeeTargetStatus Status { get; set; } = EmployeeTargetStatus.Draft;

    public Employee Employee { get; set; } = null!;
    public Campaign? Campaign { get; set; }
    public SKU? SKU { get; set; }
    public Client? Client { get; set; }
    public ICollection<RewardCalculation> RewardCalculations { get; set; } = new List<RewardCalculation>();
}
