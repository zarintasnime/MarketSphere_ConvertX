using MarketSphere.Domain.Entities.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.CRM;

public sealed class ReactivationCaseConfiguration : IEntityTypeConfiguration<ReactivationCase>
{
    public void Configure(EntityTypeBuilder<ReactivationCase> builder)
    {
        builder.ToTable("ReactivationCases");
        builder.HasKey(x => x.ReactivationCaseID);
        builder.Property(x => x.ChurnReason).HasMaxLength(1000);
        builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.ReactivationResult).HasConversion<int>();
        builder.HasIndex(x => x.ClientID).IsUnique().HasFilter("[Status] <> 4 AND [Status] <> 5 AND [Status] <> 6");
        builder.HasIndex(x => new { x.Status, x.AssignedEmployeeID, x.OpenedAt });
        builder.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AssignedEmployee).WithMany().HasForeignKey(x => x.AssignedEmployeeID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Campaign).WithMany().HasForeignKey(x => x.CampaignID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RepeatOrder).WithMany().HasForeignKey(x => x.RepeatOrderID).OnDelete(DeleteBehavior.Restrict);
    }
}
