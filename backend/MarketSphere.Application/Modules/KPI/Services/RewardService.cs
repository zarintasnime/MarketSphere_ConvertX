using Microsoft.EntityFrameworkCore;
using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.KPI.DTOs;
using MarketSphere.Application.Modules.KPI.Interfaces;
using MarketSphere.Domain.Entities.KPI;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;

namespace MarketSphere.Application.Modules.KPI.Services;

public sealed class RewardService : IRewardService
{
    private readonly IApplicationDbContext _db;
    private readonly IKpiProjectionService _projection;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _clock;

    public RewardService(
        IApplicationDbContext db,
        IKpiProjectionService projection,
        ICurrentUserService currentUser,
        IDateTimeProvider clock)
    {
        _db = db;
        _projection = projection;
        _currentUser = currentUser;
        _clock = clock;
    }

    public Task<PagedResult<RewardRuleDto>> GetRulesAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var query = _db.RewardRules.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search)) { var search = request.Search.Trim(); query = query.Where(x => x.RuleName.Contains(search)); }
        return KpiServiceHelper.ToPagedAsync(query.OrderBy(x => x.TargetType).ThenBy(x => x.MinimumAchievementPercent).Select(ToRuleProjection()), request, cancellationToken);
    }

    public async Task<int> CreateRuleAsync(SaveRewardRuleRequestDto request, CancellationToken cancellationToken = default)
    {
        await ValidateRuleAsync(request, null, cancellationToken);
        var entity = new RewardRule(); ApplyRule(entity, request);
        await _db.AddAsync(entity, cancellationToken); await _db.SaveChangesAsync(cancellationToken); return entity.RewardRuleID;
    }

    public async Task UpdateRuleAsync(int id, SaveRewardRuleRequestDto request, CancellationToken cancellationToken = default)
    {
        var entity = await KpiServiceHelper.RequireAsync(_db.RewardRules.Where(x => x.RewardRuleID == id), "Reward rule", cancellationToken);
        await ValidateRuleAsync(request, id, cancellationToken); ApplyRule(entity, request); await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CalculateAsync(CalculateRewardRequestDto request, CancellationToken cancellationToken = default)
        => await _db.ExecuteInTransactionAsync(async ct =>
        {
            var target = await KpiServiceHelper.RequireAsync(_db.EmployeeTargets.Where(x => x.EmployeeTargetID == request.EmployeeTargetID), "Employee target", ct);
            if (target.Status is not (EmployeeTargetStatus.Active or EmployeeTargetStatus.Completed)) throw new BusinessRuleException("Reward can be calculated only for an active or completed target.");
            var employee = await KpiServiceHelper.RequireAsync(_db.Employees.AsNoTracking().Where(x => x.EmployeeID == target.EmployeeID), "Employee", ct);
            var actual = await _projection.GetActualValueAsync(target.EmployeeID, (int)target.TargetType, target.TargetPeriodStart, target.TargetPeriodEnd, target.CampaignID, target.SKUID, target.ClientID, ct);
            var achievement = KpiServiceHelper.Percent(actual, target.TargetValue);
            var date = target.TargetPeriodEnd;
            var rules = await _db.RewardRules.Where(x => x.IsActive && x.TargetType == target.TargetType && (!x.ApplicableDesignationID.HasValue || x.ApplicableDesignationID == employee.DesignationID) && x.EffectiveFrom <= date && (!x.EffectiveTo.HasValue || x.EffectiveTo >= date) && x.MinimumAchievementPercent <= achievement && (!x.MaximumAchievementPercent.HasValue || x.MaximumAchievementPercent >= achievement)).OrderByDescending(x => x.ApplicableDesignationID.HasValue).ThenByDescending(x => x.MinimumAchievementPercent).ToListAsync(ct);
            var rule = rules.FirstOrDefault() ?? throw new BusinessRuleException("No active reward rule matches the target achievement.");
            if (await _db.RewardCalculations.AnyAsync(x => x.EmployeeTargetID == target.EmployeeTargetID && x.Status != RewardCalculationStatus.Rejected, ct)) throw new ConflictException("A reward calculation already exists for this target.");
            var amount = rule.CalculationType switch
            {
                RewardCalculationType.FixedAmount => rule.FixedAmount ?? 0,
                RewardCalculationType.Percentage => request.EligibleBaseAmount * (rule.RatePercent ?? 0) / 100m,
                RewardCalculationType.AchievementSlab => request.EligibleBaseAmount * (rule.RatePercent ?? 0) / 100m,
                _ => 0
            };
            if (rule.MaximumCap.HasValue) amount = Math.Min(amount, rule.MaximumCap.Value);
            var calculation = new RewardCalculation { EmployeeTargetID = target.EmployeeTargetID, EmployeeID = target.EmployeeID, RewardRuleID = rule.RewardRuleID, PeriodStart = target.TargetPeriodStart, PeriodEnd = target.TargetPeriodEnd, ActualValue = actual, AchievementPercent = achievement, EligibleBaseAmount = request.EligibleBaseAmount, RewardAmount = Math.Round(amount, 2), AdjustmentAmount = 0, FinalAmount = Math.Round(amount, 2), Status = RewardCalculationStatus.Calculated };
            await _db.AddAsync(calculation, ct); await _db.SaveChangesAsync(ct); return calculation.RewardCalculationID;
        }, cancellationToken);

    public Task<PagedResult<RewardCalculationDto>> GetCalculationsAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var query = _db.RewardCalculations.AsNoTracking();
        return KpiServiceHelper.ToPagedAsync(query.OrderByDescending(x => x.PeriodEnd).Select(x => new RewardCalculationDto(x.RewardCalculationID, x.EmployeeTargetID, x.EmployeeID, x.RewardRuleID, x.PeriodStart, x.PeriodEnd, x.ActualValue, x.AchievementPercent, x.EligibleBaseAmount, x.RewardAmount, x.AdjustmentAmount, x.FinalAmount, x.Status, x.ApprovedAt)), request, cancellationToken);
    }

    public async Task AdjustAsync(int id, AdjustRewardRequestDto request, CancellationToken cancellationToken = default)
    {
        var entity = await KpiServiceHelper.RequireAsync(_db.RewardCalculations.Where(x => x.RewardCalculationID == id), "Reward calculation", cancellationToken);
        if (entity.Status is RewardCalculationStatus.Approved or RewardCalculationStatus.Paid) throw new BusinessRuleException("An approved or paid reward is immutable.");
        if (string.IsNullOrWhiteSpace(request.Reason)) throw new BusinessRuleException("Adjustment reason is required.");
        entity.AdjustmentAmount = request.AdjustmentAmount; entity.FinalAmount = Math.Max(0, entity.RewardAmount + request.AdjustmentAmount); await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeStatusAsync(int id, ChangeRewardStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        var entity = await KpiServiceHelper.RequireAsync(_db.RewardCalculations.Where(x => x.RewardCalculationID == id), "Reward calculation", cancellationToken);
        var allowed = entity.Status switch
        {
            RewardCalculationStatus.Calculated => request.Status is RewardCalculationStatus.Submitted or RewardCalculationStatus.Rejected,
            RewardCalculationStatus.Submitted => request.Status is RewardCalculationStatus.Approved or RewardCalculationStatus.Rejected,
            RewardCalculationStatus.Approved => request.Status == RewardCalculationStatus.Paid,
            _ => false
        };
        if (!allowed) throw new BusinessRuleException("The requested reward status transition is not allowed.");
        entity.Status = request.Status;
        if (request.Status == RewardCalculationStatus.Approved) entity.ApprovedAt = _clock.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateRuleAsync(SaveRewardRuleRequestDto request, int? id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RuleName)) throw new BusinessRuleException("Rule name is required.");
        if (request.MinimumAchievementPercent < 0 || request.MaximumAchievementPercent < request.MinimumAchievementPercent) throw new BusinessRuleException("Achievement range is invalid.");
        if (request.EffectiveTo < request.EffectiveFrom) throw new BusinessRuleException("Effective end cannot be earlier than effective start.");
        if (request.CalculationType == RewardCalculationType.FixedAmount && request.FixedAmount is null or < 0) throw new BusinessRuleException("Fixed amount is required for a fixed reward rule.");
        if (request.CalculationType != RewardCalculationType.FixedAmount && request.RatePercent is null or < 0) throw new BusinessRuleException("Rate percent is required for a percentage reward rule.");
        if (request.ApplicableDesignationID.HasValue && !await _db.Designations.AnyAsync(x => x.DesignationID == request.ApplicableDesignationID && x.IsActive, cancellationToken)) throw new NotFoundException("Active designation was not found.");
        var overlap = await _db.RewardRules.AnyAsync(x => x.RewardRuleID != id && x.IsActive && request.IsActive && x.TargetType == request.TargetType && x.ApplicableDesignationID == request.ApplicableDesignationID && x.MinimumAchievementPercent <= (request.MaximumAchievementPercent ?? decimal.MaxValue) && (x.MaximumAchievementPercent ?? decimal.MaxValue) >= request.MinimumAchievementPercent && x.EffectiveFrom <= (request.EffectiveTo ?? DateTime.MaxValue) && (x.EffectiveTo ?? DateTime.MaxValue) >= request.EffectiveFrom, cancellationToken);
        if (overlap) throw new ConflictException("An active reward rule overlaps the same target, designation, achievement range and effective period.");
    }

    private static void ApplyRule(RewardRule entity, SaveRewardRuleRequestDto request) { entity.RuleName = request.RuleName.Trim(); entity.ApplicableDesignationID = request.ApplicableDesignationID; entity.RewardType = request.RewardType; entity.TargetType = request.TargetType; entity.MinimumAchievementPercent = request.MinimumAchievementPercent; entity.MaximumAchievementPercent = request.MaximumAchievementPercent; entity.CalculationType = request.CalculationType; entity.FixedAmount = request.FixedAmount; entity.RatePercent = request.RatePercent; entity.MaximumCap = request.MaximumCap; entity.EffectiveFrom = request.EffectiveFrom; entity.EffectiveTo = request.EffectiveTo; entity.IsActive = request.IsActive; }
    private static System.Linq.Expressions.Expression<Func<RewardRule, RewardRuleDto>> ToRuleProjection() => x => new RewardRuleDto(x.RewardRuleID, x.RuleName, x.ApplicableDesignationID, x.RewardType, x.TargetType, x.MinimumAchievementPercent, x.MaximumAchievementPercent, x.CalculationType, x.FixedAmount, x.RatePercent, x.MaximumCap, x.EffectiveFrom, x.EffectiveTo, x.IsActive);
}
