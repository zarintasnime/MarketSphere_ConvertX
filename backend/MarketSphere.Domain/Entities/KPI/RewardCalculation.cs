using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.KPI;

public sealed class RewardCalculation : AuditableEntity
{
    public int RewardCalculationID { get; set; }
    public int? EmployeeTargetID { get; set; }
    public int EmployeeID { get; set; }
    public int RewardRuleID { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal ActualValue { get; set; }
    public decimal AchievementPercent { get; set; }
    public decimal EligibleBaseAmount { get; set; }
    public decimal RewardAmount { get; set; }
    public decimal AdjustmentAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public RewardCalculationStatus Status { get; set; } = RewardCalculationStatus.Draft;
    public DateTime? ApprovedAt { get; set; }

    public EmployeeTarget? EmployeeTarget { get; set; }
    public Employee Employee { get; set; } = null!;
    public RewardRule RewardRule { get; set; } = null!;
}
