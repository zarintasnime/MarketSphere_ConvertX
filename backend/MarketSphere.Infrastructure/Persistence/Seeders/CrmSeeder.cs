using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Infrastructure.Persistence.Seeders;

public sealed class CrmSeeder
{
    private readonly MarketSphereDbContext _db;
    private readonly IDateTimeProvider _clock;
    public CrmSeeder(MarketSphereDbContext db, IDateTimeProvider clock) { _db = db; _clock = clock; }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedSegmentsAsync(cancellationToken);
        await SeedLeadScoreRulesAsync(cancellationToken);
    }

    private async Task SeedSegmentsAsync(CancellationToken cancellationToken)
    {
        var seeds = new[]
        {
            new SegmentSeed("REGULAR", "Regular Client", ClientSegmentType.Value, false),
            new SegmentSeed("KEY_ACCOUNT", "Key Account", ClientSegmentType.Value, false),
            new SegmentSeed("NEW_CLIENT", "New Client", ClientSegmentType.Lifecycle, true),
            new SegmentSeed("AT_RISK", "At Risk", ClientSegmentType.Risk, true),
            new SegmentSeed("INACTIVE", "Inactive Client", ClientSegmentType.Lifecycle, true)
        };
        foreach (var seed in seeds)
        {
            var entity = await _db.ClientSegments.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.SegmentCode == seed.Code, cancellationToken);
            if (entity is null)
            {
                entity = new ClientSegment { SegmentCode = seed.Code, SegmentName = seed.Name, SegmentType = seed.Type, IsSystemSegment = seed.System, IsActive = true, CreatedAt = _clock.UtcNow };
                await _db.ClientSegments.AddAsync(entity, cancellationToken);
            }
            else { entity.SegmentName = seed.Name; entity.SegmentType = seed.Type; entity.IsSystemSegment = seed.System; entity.IsActive = true; entity.IsDeleted = false; }
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedLeadScoreRulesAsync(CancellationToken cancellationToken)
    {
        var effectiveFrom = new DateOnly(2026, 1, 1);
        var seeds = new[]
        {
            new RuleSeed("Referral source", LeadScoreConditionType.Source, ComparisonOperator.Equals, LeadSource.Referral.ToString(), 20),
            new RuleSeed("Campaign source", LeadScoreConditionType.Source, ComparisonOperator.Equals, LeadSource.Campaign.ToString(), 15),
            new RuleSeed("Phone available", LeadScoreConditionType.HasPhone, ComparisonOperator.IsTrue, null, 10),
            new RuleSeed("Email available", LeadScoreConditionType.HasEmail, ComparisonOperator.IsTrue, null, 10),
            new RuleSeed("Estimated value above 100000", LeadScoreConditionType.EstimatedValue, ComparisonOperator.GreaterThanOrEqual, "100000", 25),
            new RuleSeed("Product interest available", LeadScoreConditionType.ProductInterest, ComparisonOperator.IsTrue, null, 10),
            new RuleSeed("Follow-up planned", LeadScoreConditionType.NextFollowUp, ComparisonOperator.IsTrue, null, 10)
        };
        foreach (var seed in seeds)
        {
            var entity = await _db.LeadScoreRules.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.RuleName == seed.Name, cancellationToken);
            if (entity is null)
            {
                entity = new LeadScoreRule { RuleName = seed.Name, ConditionType = seed.Type, Operator = seed.Operator, ComparisonValue = seed.Value, ScoreValue = seed.Score, EffectiveFrom = effectiveFrom, IsActive = true, CreatedAt = _clock.UtcNow };
                await _db.LeadScoreRules.AddAsync(entity, cancellationToken);
            }
            else { entity.ConditionType = seed.Type; entity.Operator = seed.Operator; entity.ComparisonValue = seed.Value; entity.ScoreValue = seed.Score; entity.EffectiveFrom = effectiveFrom; entity.IsActive = true; entity.IsDeleted = false; }
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    private sealed record SegmentSeed(string Code, string Name, ClientSegmentType Type, bool System);
    private sealed record RuleSeed(string Name, LeadScoreConditionType Type, ComparisonOperator Operator, string? Value, int Score);
}
