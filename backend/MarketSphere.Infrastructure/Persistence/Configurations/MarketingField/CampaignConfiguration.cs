using MarketSphere.Domain.Entities.MarketingField;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.MarketingField;

public sealed class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> b)
    {
        b.ToTable("Campaigns", t =>
        {
            t.HasCheckConstraint("CK_Campaigns_Budget", "[Budget] >= 0 AND [ActualExpense] >= 0");
            t.HasCheckConstraint("CK_Campaigns_DateRange", "[EndDate] >= [StartDate]");
        });
        b.HasKey(x => x.CampaignID);
        b.Property(x => x.CampaignCode).HasMaxLength(30).IsRequired();
        b.Property(x => x.CampaignTitle).HasMaxLength(200).IsRequired();
        b.Property(x => x.Objective).HasMaxLength(1000).IsRequired();
        b.Property(x => x.Budget).HasPrecision(18, 2);
        b.Property(x => x.ActualExpense).HasPrecision(18, 2);
        b.HasIndex(x => x.CampaignCode).IsUnique();
        b.HasIndex(x => new { x.Status, x.StartDate, x.EndDate });
        b.HasOne(x => x.CreatedByEmployee).WithMany().HasForeignKey(x => x.CreatedByEmployeeID).OnDelete(DeleteBehavior.Restrict);
    }
}
