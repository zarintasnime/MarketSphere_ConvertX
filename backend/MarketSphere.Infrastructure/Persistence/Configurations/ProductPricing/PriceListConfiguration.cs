using MarketSphere.Domain.Entities.ProductPricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.ProductPricing;

public sealed class PriceListConfiguration : IEntityTypeConfiguration<PriceList>
{
    public void Configure(EntityTypeBuilder<PriceList> builder)
    {
        builder.ToTable("PriceLists", table =>
        {
            table.HasCheckConstraint(
                "CK_PriceLists_DateRange",
                "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]");
        });
        builder.HasKey(x => x.PriceListID);
        builder.Property(x => x.PriceListCode).HasMaxLength(40).IsRequired();
        builder.Property(x => x.PriceListName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.HasIndex(x => x.PriceListCode).IsUnique();
        builder.HasIndex(x => new
        {
            x.Channel,
            x.ClientSegmentID,
            x.Status,
            x.EffectiveFrom,
            x.EffectiveTo
        });
        builder.HasOne(x => x.ClientSegment)
            .WithMany()
            .HasForeignKey(x => x.ClientSegmentID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
