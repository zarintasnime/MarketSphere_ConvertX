using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Modules.CRM.DTOs;
using MarketSphere.Application.Modules.CRM.Interfaces;
using MarketSphere.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.CRM.Services;

public sealed class CrmDashboardService : ICrmDashboardService
{
    private readonly IApplicationDbContext _db; private readonly IDateTimeProvider _clock;
    public CrmDashboardService(IApplicationDbContext db, IDateTimeProvider clock) { _db = db; _clock = clock; }
    public async Task<CrmDashboardDto> GetAsync(CancellationToken ct = default)
    {
        var now = _clock.UtcNow; var today = _clock.UtcToday;
        var summary = new CrmDashboardSummaryDto(
            await _db.Clients.CountAsync(x => x.IsActive && x.LifecycleStatus == ClientLifecycleStatus.Active, ct),
            await _db.Leads.CountAsync(x => x.Status != LeadStatus.Converted && x.Status != LeadStatus.Lost, ct),
            await _db.Leads.CountAsync(x => x.Temperature == LeadTemperature.Hot && x.Status != LeadStatus.Converted && x.Status != LeadStatus.Lost, ct),
            await _db.CRMTasks.CountAsync(x => x.Status != CrmTaskStatus.Completed && x.Status != CrmTaskStatus.Cancelled, ct),
            await _db.CRMTasks.CountAsync(x => x.Status != CrmTaskStatus.Completed && x.Status != CrmTaskStatus.Cancelled && x.DueAt < now, ct),
            await _db.Opportunities.CountAsync(x => x.Stage != OpportunityStage.Won && x.Stage != OpportunityStage.Lost, ct),
            await _db.Opportunities.Where(x => x.Stage != OpportunityStage.Won && x.Stage != OpportunityStage.Lost).Select(x => (decimal?)x.ExpectedValue).SumAsync(ct) ?? 0m,
            await _db.Quotations.CountAsync(x => (x.Status == QuotationStatus.Submitted || x.Status == QuotationStatus.Reviewed || x.Status == QuotationStatus.Accepted) && x.ValidUntil >= today, ct),
            await _db.Quotations.Where(x => (x.Status == QuotationStatus.Submitted || x.Status == QuotationStatus.Reviewed || x.Status == QuotationStatus.Accepted) && x.ValidUntil >= today).Select(x => (decimal?)x.NetAmount).SumAsync(ct) ?? 0m,
            await _db.Complaints.CountAsync(x => x.Status != ComplaintStatus.Resolved && x.Status != ComplaintStatus.Closed && x.Status != ComplaintStatus.Rejected, ct),
            await _db.Complaints.CountAsync(x => x.SLADueAt < now && x.Status != ComplaintStatus.Resolved && x.Status != ComplaintStatus.Closed && x.Status != ComplaintStatus.Rejected, ct),
            await _db.ReactivationCases.CountAsync(x => x.Status != ReactivationCaseStatus.Successful && x.Status != ReactivationCaseStatus.Unsuccessful && x.Status != ReactivationCaseStatus.Closed, ct));
        var leadFunnel = await _db.Leads.GroupBy(x => x.Status).Select(g => new FunnelStageDto(g.Key.ToString(), g.Count(), g.Select(x => x.EstimatedValue).Sum() ?? 0m)).OrderBy(x => x.Stage).ToListAsync(ct);
        var opportunityFunnel = await _db.Opportunities.GroupBy(x => x.Stage).Select(g => new FunnelStageDto(g.Key.ToString(), g.Count(), g.Sum(x => x.ExpectedValue))).OrderBy(x => x.Stage).ToListAsync(ct);
        return new CrmDashboardDto(summary, leadFunnel, opportunityFunnel);
    }
}
