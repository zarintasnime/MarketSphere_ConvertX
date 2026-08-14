using MarketSphere.Domain.Entities.ProductPricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.ProductPricing;

public sealed class StandardDiscountRuleConfiguration : IEntityTypeConfiguration<StandardDiscountRule>
{
    public void Configure(EntityTypeBuilder<StandardDiscountRule> builder)
    {
        builder.ToTable("StandardDiscountRules", table =>
        {
            table.HasCheckConstraint(
                "CK_StandardDiscountRules_DateRange",
                "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]");
            table.HasCheckConstraint(
                "CK_StandardDiscountRules_MinQuantity",
                "[MinQuantity] IS NULL OR [MinQuantity] > 0");
            table.HasCheckConstraint(
                "CK_StandardDiscountRules_MaxDiscount",
                "[MaxDiscountPercent] >= 0 AND [MaxDiscountPercent] <= 100");
            table.HasCheckConstraint(
                "CK_StandardDiscountRules_ProductScope",
                "[SKUID] IS NULL OR [ProductCategoryID] IS NULL");
        });
        builder.HasKey(x => x.StandardDiscountRuleID);
        builder.Property(x => x.RuleName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.MinQuantity).HasPrecision(18, 3);
        builder.Property(x => x.MaxDiscountPercent).HasPrecision(5, 2);
        builder.HasIndex(x => new
        {
            x.Channel,
            x.ClientSegmentID,
            x.SKUID,
            x.ProductCategoryID,
            x.IsActive,
            x.EffectiveFrom,
            x.EffectiveTo
        });
        builder.HasOne(x => x.ClientSegment)
            .WithMany()
            .HasForeignKey(x => x.ClientSegmentID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SKU)
            .WithMany()
            .HasForeignKey(x => x.SKUID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ProductCategory)
            .WithMany()
            .HasForeignKey(x => x.ProductCategoryID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
