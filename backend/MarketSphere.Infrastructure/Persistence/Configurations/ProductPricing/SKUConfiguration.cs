using MarketSphere.Domain.Entities.ProductPricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.ProductPricing;

public sealed class SKUConfiguration : IEntityTypeConfiguration<SKU>
{
    public void Configure(EntityTypeBuilder<SKU> builder)
    {
        builder.ToTable("SKUs", table =>
        {
            table.HasCheckConstraint("CK_SKUs_MRP", "[MRP] >= 0");
            table.HasCheckConstraint("CK_SKUs_TradePrice", "[StandardTradePrice] >= 0");
        });
        builder.HasKey(x => x.SKUID);
        builder.Property(x => x.SKUCode).HasMaxLength(50).IsRequired();
        builder.Property(x => x.SKUName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Size).HasMaxLength(50);
        builder.Property(x => x.Unit).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Barcode).HasMaxLength(100);
        builder.Property(x => x.MRP).HasPrecision(18, 2);
        builder.Property(x => x.StandardTradePrice).HasPrecision(18, 2);
        builder.HasIndex(x => x.SKUCode).IsUnique();
        builder.HasIndex(x => x.Barcode)
            .IsUnique()
            .HasFilter("[Barcode] IS NOT NULL");
        builder.HasIndex(x => new { x.ProductID, x.IsActive });
        builder.HasOne(x => x.Product)
            .WithMany(x => x.SKUs)
            .HasForeignKey(x => x.ProductID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
