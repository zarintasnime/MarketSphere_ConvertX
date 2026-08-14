namespace MarketSphere.Application.Modules.CRM.DTOs;

public sealed record CrmDashboardSummaryDto(int ActiveClients, int OpenLeads, int HotLeads, int OpenTasks, int OverdueTasks, int OpenOpportunities, decimal OpportunityPipelineValue, int ActiveQuotations, decimal ActiveQuotationValue, int OpenComplaints, int SlaBreachedComplaints, int OpenReactivationCases);
public sealed record FunnelStageDto(string Stage, int Count, decimal Value);
public sealed record CrmDashboardDto(CrmDashboardSummaryDto Summary, IReadOnlyCollection<FunnelStageDto> LeadFunnel, IReadOnlyCollection<FunnelStageDto> OpportunityFunnel);
