using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketSphere.Domain.Entities.KPI;

namespace MarketSphere.Infrastructure.Persistence.Configurations.KPI;

public sealed class RewardCalculationConfiguration : IEntityTypeConfiguration<RewardCalculation>
{
    public void Configure(EntityTypeBuilder<RewardCalculation> builder)
    {
        builder.ToTable("RewardCalculations", t => { t.HasCheckConstraint("CK_RewardCalculations_Period", "[PeriodEnd] >= [PeriodStart]"); t.HasCheckConstraint("CK_RewardCalculations_Amounts", "[ActualValue] >= 0 AND [AchievementPercent] >= 0 AND [EligibleBaseAmount] >= 0 AND [RewardAmount] >= 0 AND [FinalAmount] >= 0"); });
        builder.HasKey(x => x.RewardCalculationID); builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.ActualValue).HasPrecision(18, 3); builder.Property(x => x.AchievementPercent).HasPrecision(9, 4); builder.Property(x => x.EligibleBaseAmount).HasPrecision(18, 2); builder.Property(x => x.RewardAmount).HasPrecision(18, 2); builder.Property(x => x.AdjustmentAmount).HasPrecision(18, 2); builder.Property(x => x.FinalAmount).HasPrecision(18, 2);
        builder.HasIndex(x => x.EmployeeTargetID).IsUnique().HasFilter("[EmployeeTargetID] IS NOT NULL"); builder.HasIndex(x => new { x.EmployeeID, x.PeriodStart, x.PeriodEnd, x.Status });
        builder.HasOne(x => x.EmployeeTarget).WithMany(x => x.RewardCalculations).HasForeignKey(x => x.EmployeeTargetID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RewardRule).WithMany(x => x.RewardCalculations).HasForeignKey(x => x.RewardRuleID).OnDelete(DeleteBehavior.Restrict);
    }
}
