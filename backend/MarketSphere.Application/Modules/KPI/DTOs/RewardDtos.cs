using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.KPI.DTOs;

public sealed record RewardRuleDto(int RewardRuleID, string RuleName, int? ApplicableDesignationID, RewardType RewardType, TargetType TargetType, decimal MinimumAchievementPercent, decimal? MaximumAchievementPercent, RewardCalculationType CalculationType, decimal? FixedAmount, decimal? RatePercent, decimal? MaximumCap, DateTime EffectiveFrom, DateTime? EffectiveTo, bool IsActive);
public sealed class SaveRewardRuleRequestDto { public string RuleName { get; init; } = string.Empty; public int? ApplicableDesignationID { get; init; } public RewardType RewardType { get; init; } public TargetType TargetType { get; init; } public decimal MinimumAchievementPercent { get; init; } public decimal? MaximumAchievementPercent { get; init; } public RewardCalculationType CalculationType { get; init; } public decimal? FixedAmount { get; init; } public decimal? RatePercent { get; init; } public decimal? MaximumCap { get; init; } public DateTime EffectiveFrom { get; init; } public DateTime? EffectiveTo { get; init; } public bool IsActive { get; init; } = true; }
public sealed record RewardCalculationDto(int RewardCalculationID, int? EmployeeTargetID, int EmployeeID, int RewardRuleID, DateTime PeriodStart, DateTime PeriodEnd, decimal ActualValue, decimal AchievementPercent, decimal EligibleBaseAmount, decimal RewardAmount, decimal AdjustmentAmount, decimal FinalAmount, RewardCalculationStatus Status, DateTime? ApprovedAt);
public sealed class CalculateRewardRequestDto { public int EmployeeTargetID { get; init; } public decimal EligibleBaseAmount { get; init; } }
public sealed class AdjustRewardRequestDto { public decimal AdjustmentAmount { get; init; } public string Reason { get; init; } = string.Empty; }
public sealed class ChangeRewardStatusRequestDto { public RewardCalculationStatus Status { get; init; } public string? Note { get; init; } }
