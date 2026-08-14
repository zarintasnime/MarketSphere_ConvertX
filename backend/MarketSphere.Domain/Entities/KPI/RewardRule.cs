using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.KPI;

public sealed class RewardRule : AuditableEntity
{
    public int RewardRuleID { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public int? ApplicableDesignationID { get; set; }
    public RewardType RewardType { get; set; }
    public TargetType TargetType { get; set; }
    public decimal MinimumAchievementPercent { get; set; }
    public decimal? MaximumAchievementPercent { get; set; }
    public RewardCalculationType CalculationType { get; set; }
    public decimal? FixedAmount { get; set; }
    public decimal? RatePercent { get; set; }
    public decimal? MaximumCap { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;

    public Designation? ApplicableDesignation { get; set; }
    public ICollection<RewardCalculation> RewardCalculations { get; set; } = new List<RewardCalculation>();
}
