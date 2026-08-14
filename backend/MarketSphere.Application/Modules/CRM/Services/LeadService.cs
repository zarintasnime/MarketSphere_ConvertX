using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Mapping;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Security;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.CRM.DTOs;
using MarketSphere.Application.Modules.CRM.Interfaces;
using MarketSphere.Domain.Constants;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;

namespace MarketSphere.Application.Modules.CRM.Services;

public sealed class LeadService : ILeadService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _clock;

    public LeadService(IApplicationDbContext db, ICurrentUserService currentUser, IDateTimeProvider clock)
    {
        _db = db;
        _currentUser = currentUser;
        _clock = clock;
    }

    public Task<PagedResult<LeadListDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var query = _db.Leads.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x => x.LeadCode.Contains(search) || x.LeadName.Contains(search) || (x.BusinessName != null && x.BusinessName.Contains(search)) || (x.Phone != null && x.Phone.Contains(search)));
        }
        var projected = query.OrderByDescending(x => x.CreatedAt).Select(x => new LeadListDto(x.LeadID, x.LeadCode, x.LeadName, x.BusinessName, x.Phone, x.Source, x.CurrentScore, x.Temperature, x.Status, x.NextFollowUpAt, x.AssignedEmployeeID));
        return CrmServiceHelper.ToPagedAsync(projected, request, cancellationToken);
    }

    public async Task<LeadDetailsDto> GetByIdAsync(int leadID, CancellationToken cancellationToken = default)
        => await _db.Leads.AsNoTracking().Where(x => x.LeadID == leadID)
            .Select(x => new LeadDetailsDto(x.LeadID, x.LeadCode, x.LeadName, x.BusinessName, x.Phone, x.Email, x.Source, x.SourceCampaignID, x.AssignedEmployeeID, x.RegionID, x.ProductInterest, x.EstimatedValue, x.CurrentScore, x.Temperature, x.Status, x.NextFollowUpAt, x.LostReason, x.ReactivationAt, x.ConvertedClientID))
            .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException("Lead was not found.");

    public async Task<int> CreateAsync(SaveLeadRequestDto request, CancellationToken cancellationToken = default)
    {
        Validate(request);
        var code = request.LeadCode.NormalizeCode();
        if (await _db.Leads.AnyAsync(x => x.LeadCode == code, cancellationToken))
            throw new ConflictException(BusinessRuleMessages.DuplicateCode);
        await ValidateReferencesAsync(request, cancellationToken);
        var lead = new Lead();
        Apply(lead, request, code);
        await _db.AddAsync(lead, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        await RecalculateScoreAsync(lead.LeadID, cancellationToken);
        await CreateDuplicateCasesAsync(lead.LeadID, cancellationToken);
        return lead.LeadID;
    }

    public async Task UpdateAsync(int leadID, SaveLeadRequestDto request, CancellationToken cancellationToken = default)
    {
        Validate(request);
        var lead = await CrmServiceHelper.RequireAsync(_db.Leads.Where(x => x.LeadID == leadID), "Lead", cancellationToken);
        if (lead.Status == LeadStatus.Converted) throw new BusinessRuleException("A converted lead cannot be edited.");
        var code = request.LeadCode.NormalizeCode();
        if (await _db.Leads.AnyAsync(x => x.LeadCode == code && x.LeadID != leadID, cancellationToken))
            throw new ConflictException(BusinessRuleMessages.DuplicateCode);
        await ValidateReferencesAsync(request, cancellationToken);
        Apply(lead, request, code);
        await _db.SaveChangesAsync(cancellationToken);
        await RecalculateScoreAsync(leadID, cancellationToken);
        await CreateDuplicateCasesAsync(leadID, cancellationToken);
    }

    public async Task ChangeStatusAsync(int leadID, ChangeLeadStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        var lead = await CrmServiceHelper.RequireAsync(_db.Leads.Where(x => x.LeadID == leadID), "Lead", cancellationToken);
        if (lead.Status == request.Status) return;
        var allowed = lead.Status switch
        {
            LeadStatus.New => request.Status is LeadStatus.Contacted or LeadStatus.Qualified or LeadStatus.Lost,
            LeadStatus.Contacted => request.Status is LeadStatus.Qualified or LeadStatus.Interested or LeadStatus.Lost,
            LeadStatus.Qualified => request.Status is LeadStatus.Interested or LeadStatus.SampleGiven or LeadStatus.Negotiation or LeadStatus.Lost,
            LeadStatus.Interested => request.Status is LeadStatus.SampleGiven or LeadStatus.Negotiation or LeadStatus.Lost,
            LeadStatus.SampleGiven => request.Status is LeadStatus.Negotiation or LeadStatus.Lost,
            LeadStatus.Negotiation => request.Status is LeadStatus.Converted or LeadStatus.Lost,
            LeadStatus.Lost => request.Status is LeadStatus.Contacted or LeadStatus.Qualified,
            _ => false
        };
        if (!allowed) throw new BusinessRuleException(BusinessRuleMessages.InvalidStatusTransition);
        if (request.Status == LeadStatus.Lost) ValidationHelper.RequireNotBlank(request.LostReason, nameof(request.LostReason), 500);
        if (request.Status == LeadStatus.Converted && lead.ConvertedClientID is null)
            throw new BusinessRuleException("Convert the lead to a client before setting the status to Converted.");
        lead.Status = request.Status;
        lead.LostReason = request.Status == LeadStatus.Lost ? request.LostReason.NullIfWhiteSpace() : null;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<LeadScoreResultDto> RecalculateScoreAsync(int leadID, CancellationToken cancellationToken = default)
    {
        var lead = await CrmServiceHelper.RequireAsync(_db.Leads.Where(x => x.LeadID == leadID), "Lead", cancellationToken);
        var today = _clock.UtcToday;
        var rules = await _db.LeadScoreRules.AsNoTracking().Where(x => x.IsActive && x.EffectiveFrom <= today && (x.EffectiveTo == null || x.EffectiveTo >= today)).OrderBy(x => x.LeadScoreRuleID).ToListAsync(cancellationToken);
        var score = 0;
        var applied = new List<string>();
        foreach (var rule in rules)
        {
            if (!IsRuleMatched(lead, rule)) continue;
            score += rule.ScoreValue;
            applied.Add(rule.RuleName);
        }
        score = Math.Clamp(score, 0, 100);
        var previous = lead.CurrentScore;
        lead.CurrentScore = score;
        lead.Temperature = score >= 70 ? LeadTemperature.Hot : score >= 40 ? LeadTemperature.Warm : LeadTemperature.Cold;
        await _db.SaveChangesAsync(cancellationToken);
        return new LeadScoreResultDto(leadID, previous, lead.CurrentScore, lead.Temperature, applied);
    }

    public async Task<IReadOnlyCollection<DuplicateCandidateDto>> FindDuplicatesAsync(int leadID, CancellationToken cancellationToken = default)
    {
        var lead = await CrmServiceHelper.RequireAsync(_db.Leads.AsNoTracking().Where(x => x.LeadID == leadID), "Lead", cancellationToken);
        var candidates = new List<DuplicateCandidateDto>();
        var phone = NormalizePhone(lead.Phone);
        var email = lead.Email?.NormalizeEmail();
        var name = NormalizeName(lead.BusinessName ?? lead.LeadName);

        var leads = await _db.Leads.AsNoTracking().Where(x => x.LeadID != leadID && x.Status != LeadStatus.Converted).Take(300).ToListAsync(cancellationToken);
        foreach (var item in leads)
        {
            var match = CalculateMatch(name, phone, email, NormalizeName(item.BusinessName ?? item.LeadName), NormalizePhone(item.Phone), item.Email?.NormalizeEmail());
            if (match.Score >= 50) candidates.Add(new DuplicateCandidateDto(ReferenceTypeCodes.Lead, item.LeadID, item.BusinessName ?? item.LeadName, match.Score, match.Reasons));
        }
        var clients = await _db.Clients.AsNoTracking().Where(x => x.IsActive).Take(300).ToListAsync(cancellationToken);
        foreach (var item in clients)
        {
            var match = CalculateMatch(name, phone, email, NormalizeName(item.ClientName), NormalizePhone(item.Phone), item.Email?.NormalizeEmail());
            if (match.Score >= 50) candidates.Add(new DuplicateCandidateDto(ReferenceTypeCodes.Client, item.ClientID, item.ClientName, match.Score, match.Reasons));
        }
        return candidates.OrderByDescending(x => x.MatchScore).Take(20).ToList();
    }

    public async Task<IReadOnlyCollection<DuplicateReviewDto>> GetDuplicateReviewsAsync(CancellationToken cancellationToken = default)
        => await _db.DuplicateReviewCases.AsNoTracking().OrderBy(x => x.Status).ThenByDescending(x => x.CreatedAt)
            .Select(x => new DuplicateReviewDto(x.DuplicateReviewCaseID, x.SourceEntityType, x.SourceEntityID, x.MatchedEntityType, x.MatchedEntityID, x.MatchScore, x.MatchReasonsJson, x.Status, x.ResolutionType, x.SurvivorEntityID, x.ResolvedAt))
            .ToListAsync(cancellationToken);

    public async Task ResolveDuplicateReviewAsync(int duplicateReviewCaseID, ResolveDuplicateReviewRequestDto request, CancellationToken cancellationToken = default)
    {
        var item = await CrmServiceHelper.RequireAsync(_db.DuplicateReviewCases.Where(x => x.DuplicateReviewCaseID == duplicateReviewCaseID), "Duplicate review case", cancellationToken);
        if (item.Status is DuplicateReviewStatus.Resolved or DuplicateReviewStatus.Dismissed) throw new BusinessRuleException("The duplicate review case is already closed.");
        if (request.ResolutionType is DuplicateResolutionType.Linked or DuplicateResolutionType.Merged)
            ValidationHelper.Require(request.SurvivorEntityID.HasValue && request.SurvivorEntityID > 0, nameof(request.SurvivorEntityID), "A survivor entity is required.");
        item.ResolutionType = request.ResolutionType;
        item.SurvivorEntityID = request.SurvivorEntityID;
        item.Status = request.ResolutionType == DuplicateResolutionType.NotDuplicate ? DuplicateReviewStatus.Dismissed : DuplicateReviewStatus.Resolved;
        item.ResolvedByUserID = _currentUser.RequireUserID();
        item.ResolvedAt = _clock.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CreateScoreRuleAsync(SaveLeadScoreRuleRequestDto request, CancellationToken cancellationToken = default)
    {
        ValidationHelper.RequireNotBlank(request.RuleName, nameof(request.RuleName), 150);
        if (request.EffectiveTo.HasValue) CrmServiceHelper.ValidateDateRange(request.EffectiveFrom, request.EffectiveTo.Value, nameof(request.EffectiveTo));
        var rule = new LeadScoreRule { RuleName = request.RuleName.Trim(), ConditionType = request.ConditionType, Operator = request.Operator, ComparisonValue = request.ComparisonValue.NullIfWhiteSpace(), ScoreValue = request.ScoreValue, EffectiveFrom = request.EffectiveFrom, EffectiveTo = request.EffectiveTo, IsActive = request.IsActive };
        await _db.AddAsync(rule, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return rule.LeadScoreRuleID;
    }

    public async Task<int> ConvertToClientAsync(int leadID, ConvertLeadToClientRequestDto request, CancellationToken cancellationToken = default)
    {
        return await _db.ExecuteInTransactionAsync(async ct =>
        {
            var lead = await CrmServiceHelper.RequireAsync(_db.Leads.Where(x => x.LeadID == leadID), "Lead", ct);
            if (lead.ConvertedClientID.HasValue || lead.Status == LeadStatus.Converted) throw new ConflictException("The lead has already been converted.");
            ValidationHelper.RequireNotBlank(request.ClientCode, nameof(request.ClientCode), 30);
            ValidationHelper.RequireNotBlank(request.Address, nameof(request.Address), 500);
            var code = request.ClientCode.NormalizeCode();
            if (await _db.Clients.AnyAsync(x => x.ClientCode == code, ct)) throw new ConflictException(BusinessRuleMessages.DuplicateCode);
            var client = new Client
            {
                ClientCode = code,
                ClientName = (lead.BusinessName ?? lead.LeadName).Trim(),
                ClientType = request.ClientType,
                Channel = request.Channel,
                Phone = lead.Phone,
                Email = lead.Email,
                Address = request.Address.Trim(),
                RegionID = lead.RegionID,
                LifecycleStatus = ClientLifecycleStatus.Active,
                RiskStatus = ClientRiskStatus.Normal,
                IsActive = true
            };
            await _db.AddAsync(client, ct);
            await _db.SaveChangesAsync(ct);
            lead.ConvertedClientID = client.ClientID;
            lead.Status = LeadStatus.Converted;
            lead.LostReason = null;
            await _db.SaveChangesAsync(ct);
            return client.ClientID;
        }, cancellationToken);
    }

    private async Task ValidateReferencesAsync(SaveLeadRequestDto request, CancellationToken cancellationToken)
    {
        if (request.AssignedEmployeeID.HasValue && !await _db.Employees.AnyAsync(x => x.EmployeeID == request.AssignedEmployeeID, cancellationToken)) throw new NotFoundException("Assigned employee was not found.");
        if (request.RegionID.HasValue && !await _db.Regions.AnyAsync(x => x.RegionID == request.RegionID, cancellationToken)) throw new NotFoundException("Region was not found.");
        if (request.SourceCampaignID.HasValue && !await _db.Campaigns.AnyAsync(x => x.CampaignID == request.SourceCampaignID, cancellationToken)) throw new NotFoundException("Source campaign was not found.");
    }

    private async Task CreateDuplicateCasesAsync(int leadID, CancellationToken cancellationToken)
    {
        var candidates = await FindDuplicatesAsync(leadID, cancellationToken);
        foreach (var candidate in candidates.Where(x => x.MatchScore >= 70))
        {
            var exists = await _db.DuplicateReviewCases.AnyAsync(x => x.SourceEntityType == ReferenceTypeCodes.Lead && x.SourceEntityID == leadID && x.MatchedEntityType == candidate.EntityType && x.MatchedEntityID == candidate.EntityID && x.Status != DuplicateReviewStatus.Resolved && x.Status != DuplicateReviewStatus.Dismissed, cancellationToken);
            if (exists) continue;
            await _db.AddAsync(new DuplicateReviewCase
            {
                SourceEntityType = ReferenceTypeCodes.Lead,
                SourceEntityID = leadID,
                MatchedEntityType = candidate.EntityType,
                MatchedEntityID = candidate.EntityID,
                MatchScore = candidate.MatchScore,
                MatchReasonsJson = JsonSerializer.Serialize(candidate.Reasons),
                Status = DuplicateReviewStatus.Open
            }, cancellationToken);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    private static void Validate(SaveLeadRequestDto request)
    {
        ValidationHelper.RequireNotBlank(request.LeadCode, nameof(request.LeadCode), 30);
        ValidationHelper.RequireNotBlank(request.LeadName, nameof(request.LeadName), 150);
        ValidationHelper.Require(!string.IsNullOrWhiteSpace(request.Phone) || !string.IsNullOrWhiteSpace(request.Email), nameof(request.Phone), "Phone or email is required.");
        if (request.EstimatedValue.HasValue) ValidationHelper.Require(request.EstimatedValue >= 0, nameof(request.EstimatedValue), "Estimated value cannot be negative.");
    }

    private static void Apply(Lead lead, SaveLeadRequestDto request, string code)
    {
        lead.LeadCode = code;
        lead.LeadName = request.LeadName.Trim();
        lead.BusinessName = request.BusinessName.NullIfWhiteSpace();
        lead.Phone = request.Phone.NullIfWhiteSpace();
        lead.Email = request.Email.NullIfWhiteSpace()?.NormalizeEmail();
        lead.Source = request.Source;
        lead.SourceCampaignID = request.SourceCampaignID;
        lead.AssignedEmployeeID = request.AssignedEmployeeID;
        lead.RegionID = request.RegionID;
        lead.ProductInterest = request.ProductInterest.NullIfWhiteSpace();
        lead.EstimatedValue = request.EstimatedValue;
        lead.NextFollowUpAt = request.NextFollowUpAt;
    }

    private static bool IsRuleMatched(Lead lead, LeadScoreRule rule)
    {
        return rule.ConditionType switch
        {
            LeadScoreConditionType.Source => CompareText(lead.Source.ToString(), rule.Operator, rule.ComparisonValue),
            LeadScoreConditionType.EstimatedValue => CompareNumber(lead.EstimatedValue ?? 0, rule.Operator, rule.ComparisonValue),
            LeadScoreConditionType.HasPhone => CompareBoolean(!string.IsNullOrWhiteSpace(lead.Phone), rule.Operator),
            LeadScoreConditionType.HasEmail => CompareBoolean(!string.IsNullOrWhiteSpace(lead.Email), rule.Operator),
            LeadScoreConditionType.ProductInterest => CompareText(lead.ProductInterest, rule.Operator, rule.ComparisonValue),
            LeadScoreConditionType.Region => CompareNumber(lead.RegionID ?? 0, rule.Operator, rule.ComparisonValue),
            LeadScoreConditionType.NextFollowUp => CompareBoolean(lead.NextFollowUpAt.HasValue, rule.Operator),
            _ => false
        };
    }

    private static bool CompareText(string? actual, ComparisonOperator op, string? expected) => op switch
    {
        ComparisonOperator.Equals => string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase),
        ComparisonOperator.NotEquals => !string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase),
        ComparisonOperator.Contains => actual?.Contains(expected ?? string.Empty, StringComparison.OrdinalIgnoreCase) == true,
        ComparisonOperator.IsTrue => !string.IsNullOrWhiteSpace(actual),
        ComparisonOperator.IsFalse => string.IsNullOrWhiteSpace(actual),
        _ => false
    };

    private static bool CompareNumber(decimal actual, ComparisonOperator op, string? expectedText)
    {
        if (!decimal.TryParse(expectedText, NumberStyles.Any, CultureInfo.InvariantCulture, out var expected)) return false;
        return op switch
        {
            ComparisonOperator.Equals => actual == expected,
            ComparisonOperator.NotEquals => actual != expected,
            ComparisonOperator.GreaterThan => actual > expected,
            ComparisonOperator.GreaterThanOrEqual => actual >= expected,
            ComparisonOperator.LessThan => actual < expected,
            ComparisonOperator.LessThanOrEqual => actual <= expected,
            _ => false
        };
    }

    private static bool CompareBoolean(bool actual, ComparisonOperator op) => op switch
    {
        ComparisonOperator.IsTrue => actual,
        ComparisonOperator.IsFalse => !actual,
        ComparisonOperator.Equals => actual,
        ComparisonOperator.NotEquals => !actual,
        _ => false
    };

    private static (decimal Score, IReadOnlyCollection<string> Reasons) CalculateMatch(string name, string phone, string? email, string otherName, string otherPhone, string? otherEmail)
    {
        var score = 0m;
        var reasons = new List<string>();
        if (!string.IsNullOrEmpty(phone) && phone == otherPhone) { score += 50; reasons.Add("Phone matched"); }
        if (!string.IsNullOrEmpty(email) && email == otherEmail) { score += 40; reasons.Add("Email matched"); }
        if (!string.IsNullOrEmpty(name) && name == otherName) { score += 30; reasons.Add("Name matched"); }
        else if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(otherName) && (name.Contains(otherName) || otherName.Contains(name))) { score += 15; reasons.Add("Name was similar"); }
        return (Math.Min(score, 100m), reasons);
    }

    private static string NormalizePhone(string? value) => string.Concat((value ?? string.Empty).Where(char.IsDigit));
    private static string NormalizeName(string? value) => string.Concat((value ?? string.Empty).Where(char.IsLetterOrDigit)).ToUpperInvariant();
}
