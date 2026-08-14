using MarketSphere.Domain.Entities.MarketingField;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.MarketingField;

public sealed class CampaignOfferConfiguration : IEntityTypeConfiguration<CampaignOffer>
{
    public void Configure(EntityTypeBuilder<CampaignOffer> builder)
    {
        builder.ToTable("CampaignOffers", table =>
        {
            table.HasCheckConstraint("CK_CampaignOffers_DiscountValue", "[DiscountValue] IS NULL OR [DiscountValue] >= 0");
            table.HasCheckConstraint("CK_CampaignOffers_UsageLimit", "[UsageLimit] IS NULL OR [UsageLimit] > 0");
            table.HasCheckConstraint("CK_CampaignOffers_PerClientLimit", "[PerClientLimit] IS NULL OR [PerClientLimit] > 0");
        });
        builder.HasKey(x => x.CampaignOfferID);
        builder.Property(x => x.OfferCode).HasMaxLength(40).IsRequired();
        builder.Property(x => x.RuleJson).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(x => x.DiscountValue).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.CampaignID, x.OfferCode }).IsUnique();
        builder.HasIndex(x => new { x.CampaignID, x.IsActive, x.Priority });
        builder.HasOne(x => x.Campaign).WithMany(x => x.Offers).HasForeignKey(x => x.CampaignID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.FreeSKU).WithMany().HasForeignKey(x => x.FreeSKUID).OnDelete(DeleteBehavior.Restrict);
    }
}
