using MarketSphere.Domain.Common;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.CRM;

public sealed class LeadScoreRule : SoftDeletableEntity
{
    public int LeadScoreRuleID { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public LeadScoreConditionType ConditionType { get; set; }
    public ComparisonOperator Operator { get; set; }
    public string? ComparisonValue { get; set; }
    public int ScoreValue { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
}
