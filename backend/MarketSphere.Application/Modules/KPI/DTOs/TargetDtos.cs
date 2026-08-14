using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.KPI.DTOs;

public sealed record EmployeeTargetListDto(int EmployeeTargetID, int EmployeeID, string EmployeeCode, string EmployeeName, DateTime TargetPeriodStart, DateTime TargetPeriodEnd, TargetType TargetType, decimal TargetValue, EmployeeTargetStatus Status);
public sealed record EmployeeTargetDetailsDto(int EmployeeTargetID, int EmployeeID, DateTime TargetPeriodStart, DateTime TargetPeriodEnd, TargetType TargetType, decimal TargetValue, decimal? TargetAmount, int? CampaignID, int? SKUID, int? ClientID, EmployeeTargetStatus Status);
public sealed class SaveEmployeeTargetRequestDto { public int EmployeeID { get; init; } public DateTime TargetPeriodStart { get; init; } public DateTime TargetPeriodEnd { get; init; } public TargetType TargetType { get; init; } public decimal TargetValue { get; init; } public decimal? TargetAmount { get; init; } public int? CampaignID { get; init; } public int? SKUID { get; init; } public int? ClientID { get; init; } }
public sealed class ChangeEmployeeTargetStatusRequestDto { public EmployeeTargetStatus Status { get; init; } }
public sealed record TargetProgressDto(int EmployeeTargetID, decimal TargetValue, decimal ActualValue, decimal AchievementPercent, EmployeeTargetStatus Status);
