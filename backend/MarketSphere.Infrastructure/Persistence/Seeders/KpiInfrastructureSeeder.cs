using Microsoft.EntityFrameworkCore;
using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Domain.Constants;
using MarketSphere.Domain.Entities.Infrastructure;
using MarketSphere.Domain.Entities.KPI;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Infrastructure.Persistence.Seeders;

public sealed class KpiInfrastructureSeeder
{
    private readonly MarketSphereDbContext _db; private readonly IDateTimeProvider _clock;
    public KpiInfrastructureSeeder(MarketSphereDbContext db, IDateTimeProvider clock) { _db = db; _clock = clock; }
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!await _db.RewardRules.AnyAsync(cancellationToken)) await _db.RewardRules.AddAsync(new RewardRule { RuleName = "Standard Sales Achievement Incentive", RewardType = RewardType.Incentive, TargetType = TargetType.SalesAmount, MinimumAchievementPercent = 100, MaximumAchievementPercent = null, CalculationType = RewardCalculationType.Percentage, RatePercent = 1, MaximumCap = 50000, EffectiveFrom = new DateTime(_clock.UtcNow.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true, CreatedAt = _clock.UtcNow }, cancellationToken);
        if (!await _db.ApprovalPolicies.AnyAsync(x => x.ApprovalType == ApprovalType.Order && x.EntityType == ReferenceTypeCodes.Order, cancellationToken))
        {
            var policy = new ApprovalPolicy { ApprovalType = ApprovalType.Order, EntityType = ReferenceTypeCodes.Order, MinimumAmount = 100000, EffectiveFrom = new DateTime(_clock.UtcNow.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc), Priority = 100, IsActive = true, CreatedAt = _clock.UtcNow }; await _db.ApprovalPolicies.AddAsync(policy, cancellationToken); await _db.SaveChangesAsync(cancellationToken);
            var step = new ApprovalStepDefinition { ApprovalPolicyID = policy.ApprovalPolicyID, StepNo = 1, StepName = "Sales Manager Approval", ApprovalMode = ApprovalMode.AnyOne, MinimumApprovals = 1, IsFinalStep = true, CreatedAt = _clock.UtcNow }; await _db.ApprovalStepDefinitions.AddAsync(step, cancellationToken); await _db.SaveChangesAsync(cancellationToken);
            var roleID = await _db.Roles.Where(x => x.RoleCode == RoleCodes.SalesManager).Select(x => x.RoleID).SingleAsync(cancellationToken); await _db.ApprovalStepAssignees.AddAsync(new ApprovalStepAssignee { ApprovalStepDefinitionID = step.ApprovalStepDefinitionID, AssigneeType = ApprovalAssigneeType.Role, RoleID = roleID, Priority = 1, CreatedAt = _clock.UtcNow }, cancellationToken);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }
}
