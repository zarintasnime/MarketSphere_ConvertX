using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.CRM.DTOs;

public sealed record ReactivationCaseDto(int ReactivationCaseID, int ClientID, DateTime InactiveAt, string? ChurnReason, int? CampaignID, int AssignedEmployeeID, DateTime OpenedAt, ReactivationCaseStatus Status, ReactivationResult? ReactivationResult, DateTime? ReactivatedAt, int? RepeatOrderID);
public sealed class CreateReactivationCaseRequestDto { public int ClientID { get; init; } public DateTime InactiveAt { get; init; } public string? ChurnReason { get; init; } public int? CampaignID { get; init; } public int AssignedEmployeeID { get; init; } }
public sealed class ResolveReactivationCaseRequestDto { public ReactivationCaseStatus Status { get; init; } public ReactivationResult? ReactivationResult { get; init; } public int? RepeatOrderID { get; init; } }
