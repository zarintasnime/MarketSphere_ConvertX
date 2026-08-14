using MarketSphere.Domain.Entities.MarketingField;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.MarketingField;

public sealed class CampaignTargetConfiguration : IEntityTypeConfiguration<CampaignTarget>
{
    public void Configure(EntityTypeBuilder<CampaignTarget> builder)
    {
        builder.ToTable("CampaignTargets", table =>
        {
            table.HasCheckConstraint("CK_CampaignTargets_TargetValue", "[TargetValue] IS NULL OR [TargetValue] >= 0");
            table.HasCheckConstraint("CK_CampaignTargets_OneReference", "(CASE WHEN [RegionID] IS NULL THEN 0 ELSE 1 END + CASE WHEN [AreaID] IS NULL THEN 0 ELSE 1 END + CASE WHEN [ClientSegmentID] IS NULL THEN 0 ELSE 1 END + CASE WHEN [ClientID] IS NULL THEN 0 ELSE 1 END + CASE WHEN [ProductCategoryID] IS NULL THEN 0 ELSE 1 END + CASE WHEN [SKUID] IS NULL THEN 0 ELSE 1 END) = 1");
            table.HasCheckConstraint("CK_CampaignTargets_TypeReference", "([TargetType] = 1 AND [RegionID] IS NOT NULL) OR ([TargetType] = 2 AND [AreaID] IS NOT NULL) OR ([TargetType] = 3 AND [ClientSegmentID] IS NOT NULL) OR ([TargetType] = 4 AND [ClientID] IS NOT NULL) OR ([TargetType] = 5 AND [ProductCategoryID] IS NOT NULL) OR ([TargetType] = 6 AND [SKUID] IS NOT NULL)");
        });
        builder.HasKey(x => x.CampaignTargetID);
        builder.Property(x => x.TargetValue).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.CampaignID, x.TargetType });
        builder.HasOne(x => x.Campaign).WithMany(x => x.Targets).HasForeignKey(x => x.CampaignID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Region).WithMany().HasForeignKey(x => x.RegionID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Area).WithMany().HasForeignKey(x => x.AreaID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ClientSegment).WithMany().HasForeignKey(x => x.ClientSegmentID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ProductCategory).WithMany().HasForeignKey(x => x.ProductCategoryID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
    }
}
