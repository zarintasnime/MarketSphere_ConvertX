using MarketSphere.Domain.Entities.MarketingField;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.MarketingField;

public sealed class CampaignAttributionConfiguration : IEntityTypeConfiguration<CampaignAttribution>
{
    public void Configure(EntityTypeBuilder<CampaignAttribution> builder)
    {
        builder.ToTable("CampaignAttributions", t =>
        {
            t.HasCheckConstraint("CK_CampaignAttributions_Reference", "[LeadID] IS NOT NULL OR [OpportunityID] IS NOT NULL OR [QuotationID] IS NOT NULL OR [OrderID] IS NOT NULL");
            t.HasCheckConstraint("CK_CampaignAttributions_Weight", "[WeightPercent] >= 0 AND [WeightPercent] <= 100");
        });
        builder.HasKey(x => x.CampaignAttributionID);
        builder.Property(x => x.AttributionType).HasConversion<int>();
        builder.Property(x => x.WeightPercent).HasPrecision(5, 2);
        builder.Property(x => x.AttributedAmount).HasPrecision(18, 2);
        builder.HasOne(x => x.Campaign).WithMany(x => x.Attributions).HasForeignKey(x => x.CampaignID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Lead).WithMany().HasForeignKey(x => x.LeadID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Opportunity).WithMany().HasForeignKey(x => x.OpportunityID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Quotation).WithMany().HasForeignKey(x => x.QuotationID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderID).OnDelete(DeleteBehavior.Restrict);
    }
}
