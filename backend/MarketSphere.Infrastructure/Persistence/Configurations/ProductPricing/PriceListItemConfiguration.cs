using MarketSphere.Domain.Entities.ProductPricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.ProductPricing;

public sealed class PriceListItemConfiguration : IEntityTypeConfiguration<PriceListItem>
{
    public void Configure(EntityTypeBuilder<PriceListItem> builder)
    {
        builder.ToTable("PriceListItems", table =>
        {
            table.HasCheckConstraint("CK_PriceListItems_UnitPrice", "[UnitPrice] >= 0");
            table.HasCheckConstraint(
                "CK_PriceListItems_MaximumDiscount",
                "[MaximumDiscountPercent] >= 0 AND [MaximumDiscountPercent] <= 100");
            table.HasCheckConstraint(
                "CK_PriceListItems_MinimumOrderQuantity",
                "[MinimumOrderQuantity] IS NULL OR [MinimumOrderQuantity] > 0");
        });
        builder.HasKey(x => x.PriceListItemID);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.MaximumDiscountPercent).HasPrecision(5, 2);
        builder.Property(x => x.MinimumOrderQuantity).HasPrecision(18, 3);
        builder.HasIndex(x => new { x.PriceListID, x.SKUID }).IsUnique();
        builder.HasOne(x => x.PriceList)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.PriceListID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SKU)
            .WithMany(x => x.PriceListItems)
            .HasForeignKey(x => x.SKUID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
