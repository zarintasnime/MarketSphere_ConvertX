using MarketSphere.Domain.Entities.MarketingField;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.MarketingField;

public sealed class SamplingLogConfiguration : IEntityTypeConfiguration<SamplingLog>
{
    public void Configure(EntityTypeBuilder<SamplingLog> builder)
    {
        builder.ToTable("SamplingLogs", table =>
        {
            table.HasCheckConstraint("CK_SamplingLogs_Party", "[ClientID] IS NOT NULL OR [LeadID] IS NOT NULL");
            table.HasCheckConstraint("CK_SamplingLogs_Quantities", "[IssuedQuantity] > 0 AND [ConsumedQuantity] >= 0 AND [ReturnedQuantity] >= 0 AND [DamagedQuantity] >= 0");
            table.HasCheckConstraint("CK_SamplingLogs_Balance", "[IssuedQuantity] = [ConsumedQuantity] + [ReturnedQuantity] + [DamagedQuantity]");
        });
        builder.HasKey(x => x.SamplingLogID);
        builder.Property(x => x.IssuedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.ConsumedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.ReturnedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.DamagedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.FeedbackSummary).HasMaxLength(2000);
        builder.HasIndex(x => new { x.EmployeeID, x.SampleDate });
        builder.HasIndex(x => new { x.CampaignID, x.SampleDate });
        builder.HasOne(x => x.Visit).WithMany(x => x.SamplingLogs).HasForeignKey(x => x.VisitID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Campaign).WithMany().HasForeignKey(x => x.CampaignID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Lead).WithMany().HasForeignKey(x => x.LeadID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
    }
}
