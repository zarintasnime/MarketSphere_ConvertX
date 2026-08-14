using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketSphere.Domain.Entities.KPI;

namespace MarketSphere.Infrastructure.Persistence.Configurations.KPI;

public sealed class RewardRuleConfiguration : IEntityTypeConfiguration<RewardRule>
{
    public void Configure(EntityTypeBuilder<RewardRule> builder)
    {
        builder.ToTable("RewardRules", t => { t.HasCheckConstraint("CK_RewardRules_Achievement", "[MinimumAchievementPercent] >= 0 AND ([MaximumAchievementPercent] IS NULL OR [MaximumAchievementPercent] >= [MinimumAchievementPercent])"); t.HasCheckConstraint("CK_RewardRules_Period", "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]"); t.HasCheckConstraint("CK_RewardRules_Amounts", "([FixedAmount] IS NULL OR [FixedAmount] >= 0) AND ([RatePercent] IS NULL OR [RatePercent] >= 0) AND ([MaximumCap] IS NULL OR [MaximumCap] >= 0)"); });
        builder.HasKey(x => x.RewardRuleID); builder.Property(x => x.RuleName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.RewardType).HasConversion<int>(); builder.Property(x => x.TargetType).HasConversion<int>(); builder.Property(x => x.CalculationType).HasConversion<int>();
        builder.Property(x => x.MinimumAchievementPercent).HasPrecision(9, 4); builder.Property(x => x.MaximumAchievementPercent).HasPrecision(9, 4); builder.Property(x => x.FixedAmount).HasPrecision(18, 2); builder.Property(x => x.RatePercent).HasPrecision(9, 4); builder.Property(x => x.MaximumCap).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.TargetType, x.ApplicableDesignationID, x.IsActive, x.EffectiveFrom });
        builder.HasOne(x => x.ApplicableDesignation).WithMany().HasForeignKey(x => x.ApplicableDesignationID).OnDelete(DeleteBehavior.Restrict);
    }
}
