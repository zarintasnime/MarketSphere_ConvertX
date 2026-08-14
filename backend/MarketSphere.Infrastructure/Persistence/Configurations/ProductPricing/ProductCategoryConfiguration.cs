using MarketSphere.Domain.Entities.ProductPricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.ProductPricing;

public sealed class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("ProductCategories");
        builder.HasKey(x => x.ProductCategoryID);
        builder.Property(x => x.CategoryCode).HasMaxLength(30).IsRequired();
        builder.Property(x => x.CategoryName).HasMaxLength(150).IsRequired();
        builder.HasIndex(x => x.CategoryCode).IsUnique();
        builder.HasIndex(x => new { x.ParentProductCategoryID, x.CategoryName });
        builder.HasOne(x => x.ParentProductCategory)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentProductCategoryID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
