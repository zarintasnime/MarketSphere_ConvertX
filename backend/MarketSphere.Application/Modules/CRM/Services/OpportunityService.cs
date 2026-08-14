using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Mapping;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.CRM.DTOs;
using MarketSphere.Application.Modules.CRM.Interfaces;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.CRM.Services;

public sealed class OpportunityService : IOpportunityService
{
    private readonly IApplicationDbContext _db; private readonly IDateTimeProvider _clock;
    public OpportunityService(IApplicationDbContext db, IDateTimeProvider clock) { _db = db; _clock = clock; }
    public Task<PagedResult<OpportunityListDto>> GetPagedAsync(PagedRequest r, CancellationToken ct = default)
    {
        var q = _db.Opportunities.AsNoTracking(); if (!string.IsNullOrWhiteSpace(r.Search)) { var s = r.Search.Trim(); q = q.Where(x => x.OpportunityCode.Contains(s) || x.OpportunityName.Contains(s)); }
        return CrmServiceHelper.ToPagedAsync(q.OrderByDescending(x => x.CreatedAt).Select(x => new OpportunityListDto(x.OpportunityID, x.OpportunityCode, x.OpportunityName, x.LeadID, x.ClientID, x.OwnerEmployeeID, x.Stage, x.ExpectedValue, x.ProbabilityPercent, x.ExpectedCloseDate)), r, ct);
    }
    public async Task<OpportunityDetailsDto> GetByIdAsync(int id, CancellationToken ct = default) => await _db.Opportunities.AsNoTracking().Where(x => x.OpportunityID == id).Select(x => new OpportunityDetailsDto(x.OpportunityID, x.OpportunityCode, x.OpportunityName, x.LeadID, x.ClientID, x.CampaignID, x.OwnerEmployeeID, x.Stage, x.ExpectedValue, x.ProbabilityPercent, x.ExpectedCloseDate, x.Competitor, x.LostReason, x.WonAt)).SingleOrDefaultAsync(ct) ?? throw new NotFoundException("Opportunity was not found.");
    public async Task<int> CreateAsync(SaveOpportunityRequestDto r, CancellationToken ct = default) { Validate(r); await ValidateRefs(r, ct); var code = r.OpportunityCode.NormalizeCode(); if (await _db.Opportunities.AnyAsync(x => x.OpportunityCode == code, ct)) throw new ConflictException(BusinessRuleMessages.DuplicateCode); var e = new Opportunity(); Apply(e, r, code); await _db.AddAsync(e, ct); await _db.SaveChangesAsync(ct); return e.OpportunityID; }
    public async Task UpdateAsync(int id, SaveOpportunityRequestDto r, CancellationToken ct = default) { Validate(r); await ValidateRefs(r, ct); var e = await CrmServiceHelper.RequireAsync(_db.Opportunities.Where(x => x.OpportunityID == id), "Opportunity", ct); if (e.Stage is OpportunityStage.Won or OpportunityStage.Lost) throw new BusinessRuleException("A closed opportunity cannot be edited."); var code = r.OpportunityCode.NormalizeCode(); if (await _db.Opportunities.AnyAsync(x => x.OpportunityCode == code && x.OpportunityID != id, ct)) throw new ConflictException(BusinessRuleMessages.DuplicateCode); Apply(e, r, code); await _db.SaveChangesAsync(ct); }
    public async Task ChangeStageAsync(int id, ChangeOpportunityStageRequestDto r, CancellationToken ct = default) { var e = await CrmServiceHelper.RequireAsync(_db.Opportunities.Where(x => x.OpportunityID == id), "Opportunity", ct); if (e.Stage == r.Stage) return; if (r.Stage == OpportunityStage.Lost) ValidationHelper.RequireNotBlank(r.LostReason, nameof(r.LostReason), 500); if (e.Stage is OpportunityStage.Won or OpportunityStage.Lost) throw new BusinessRuleException(BusinessRuleMessages.InvalidStatusTransition); e.Stage = r.Stage; e.LostReason = r.Stage == OpportunityStage.Lost ? r.LostReason.NullIfWhiteSpace() : null; e.WonAt = r.Stage == OpportunityStage.Won ? _clock.UtcNow : null; await _db.SaveChangesAsync(ct); }
    private async Task ValidateRefs(SaveOpportunityRequestDto r, CancellationToken ct) { if (!await _db.Employees.AnyAsync(x => x.EmployeeID == r.OwnerEmployeeID, ct)) throw new NotFoundException("Owner employee was not found."); if (r.LeadID.HasValue && !await _db.Leads.AnyAsync(x => x.LeadID == r.LeadID, ct)) throw new NotFoundException("Lead was not found."); if (r.ClientID.HasValue && !await _db.Clients.AnyAsync(x => x.ClientID == r.ClientID, ct)) throw new NotFoundException("Client was not found."); if (r.CampaignID.HasValue && !await _db.Campaigns.AnyAsync(x => x.CampaignID == r.CampaignID, ct)) throw new NotFoundException("Campaign was not found."); }
    private static void Validate(SaveOpportunityRequestDto r) { ValidationHelper.RequireNotBlank(r.OpportunityCode, nameof(r.OpportunityCode), 30); ValidationHelper.RequireNotBlank(r.OpportunityName, nameof(r.OpportunityName), 200); ValidationHelper.Require(r.LeadID.HasValue || r.ClientID.HasValue, nameof(r.LeadID), "LeadID or ClientID is required."); ValidationHelper.Require(r.ExpectedValue >= 0, nameof(r.ExpectedValue), "Expected value cannot be negative."); ValidationHelper.Require(r.ProbabilityPercent is >= 0 and <= 100, nameof(r.ProbabilityPercent), "Probability must be between 0 and 100."); }
    private static void Apply(Opportunity e, SaveOpportunityRequestDto r, string code) { e.OpportunityCode = code; e.LeadID = r.LeadID; e.ClientID = r.ClientID; e.CampaignID = r.CampaignID; e.OwnerEmployeeID = r.OwnerEmployeeID; e.OpportunityName = r.OpportunityName.Trim(); e.ExpectedValue = r.ExpectedValue; e.ProbabilityPercent = r.ProbabilityPercent; e.ExpectedCloseDate = r.ExpectedCloseDate; e.Competitor = r.Competitor.NullIfWhiteSpace(); }
}
