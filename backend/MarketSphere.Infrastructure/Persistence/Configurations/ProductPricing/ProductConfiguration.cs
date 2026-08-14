using MarketSphere.Domain.Entities.ProductPricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.ProductPricing;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", table =>
        {
            table.HasCheckConstraint(
                "CK_Products_ExpiryRequiresBatch",
                "[RequiresExpiryDate] = 0 OR [RequiresBatch] = 1");
        });
        builder.HasKey(x => x.ProductID);
        builder.Property(x => x.ProductCode).HasMaxLength(40).IsRequired();
        builder.Property(x => x.ProductName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.HasIndex(x => x.ProductCode).IsUnique();
        builder.HasIndex(x => new { x.ProductCategoryID, x.IsActive });
        builder.HasIndex(x => new { x.BrandID, x.IsActive });
        builder.HasOne(x => x.ProductCategory)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.ProductCategoryID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Brand)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.BrandID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
