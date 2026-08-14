using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketSphere.Domain.Entities.KPI;

namespace MarketSphere.Infrastructure.Persistence.Configurations.KPI;

public sealed class EmployeeTargetConfiguration : IEntityTypeConfiguration<EmployeeTarget>
{
    public void Configure(EntityTypeBuilder<EmployeeTarget> builder)
    {
        builder.ToTable("EmployeeTargets", t => { t.HasCheckConstraint("CK_EmployeeTargets_Period", "[TargetPeriodEnd] >= [TargetPeriodStart]"); t.HasCheckConstraint("CK_EmployeeTargets_Value", "[TargetValue] > 0 AND ([TargetAmount] IS NULL OR [TargetAmount] >= 0)"); });
        builder.HasKey(x => x.EmployeeTargetID);
        builder.Property(x => x.TargetType).HasConversion<int>(); builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.TargetValue).HasPrecision(18, 3); builder.Property(x => x.TargetAmount).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.EmployeeID, x.TargetPeriodStart, x.TargetPeriodEnd, x.TargetType, x.CampaignID, x.SKUID, x.ClientID }).IsUnique();
        builder.HasIndex(x => new { x.Status, x.TargetPeriodEnd });
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Campaign).WithMany().HasForeignKey(x => x.CampaignID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientID).OnDelete(DeleteBehavior.Restrict);
    }
}

