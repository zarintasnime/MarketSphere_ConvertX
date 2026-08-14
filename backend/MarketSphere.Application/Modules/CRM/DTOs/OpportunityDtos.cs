using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.CRM.DTOs;

public sealed record OpportunityListDto(int OpportunityID, string OpportunityCode, string OpportunityName, int? LeadID, int? ClientID, int OwnerEmployeeID, OpportunityStage Stage, decimal ExpectedValue, int ProbabilityPercent, DateOnly? ExpectedCloseDate);
public sealed record OpportunityDetailsDto(int OpportunityID, string OpportunityCode, string OpportunityName, int? LeadID, int? ClientID, int? CampaignID, int OwnerEmployeeID, OpportunityStage Stage, decimal ExpectedValue, int ProbabilityPercent, DateOnly? ExpectedCloseDate, string? Competitor, string? LostReason, DateTime? WonAt);
public class SaveOpportunityRequestDto { public string OpportunityCode { get; init; } = string.Empty; public int? LeadID { get; init; } public int? ClientID { get; init; } public int? CampaignID { get; init; } public int OwnerEmployeeID { get; init; } public string OpportunityName { get; init; } = string.Empty; public decimal ExpectedValue { get; init; } public int ProbabilityPercent { get; init; } public DateOnly? ExpectedCloseDate { get; init; } public string? Competitor { get; init; } }
public sealed class ChangeOpportunityStageRequestDto { public OpportunityStage Stage { get; init; } public string? LostReason { get; init; } }
