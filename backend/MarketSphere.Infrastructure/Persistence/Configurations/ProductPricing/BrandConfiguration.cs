using MarketSphere.Domain.Entities.ProductPricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.ProductPricing;

public sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");
        builder.HasKey(x => x.BrandID);
        builder.Property(x => x.BrandCode).HasMaxLength(30).IsRequired();
        builder.Property(x => x.BrandName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.OwnerCompanyName).HasMaxLength(150);
        builder.HasIndex(x => x.BrandCode).IsUnique();
        builder.HasIndex(x => x.BrandName);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
