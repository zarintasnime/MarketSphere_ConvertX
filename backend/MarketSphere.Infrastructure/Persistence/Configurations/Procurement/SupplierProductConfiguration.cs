using MarketSphere.Domain.Entities.Procurement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Procurement;

public sealed class SupplierProductConfiguration : IEntityTypeConfiguration<SupplierProduct>
{
    public void Configure(EntityTypeBuilder<SupplierProduct> builder)
    {
        builder.ToTable("SupplierProducts", t =>
        {
            t.HasCheckConstraint("CK_SupplierProducts_LastPrice", "[LastPurchasePrice] IS NULL OR [LastPurchasePrice] >= 0");
            t.HasCheckConstraint("CK_SupplierProducts_MinQty", "[MinimumOrderQuantity] IS NULL OR [MinimumOrderQuantity] > 0");
            t.HasCheckConstraint("CK_SupplierProducts_LeadTime", "[LeadTimeDays] IS NULL OR [LeadTimeDays] >= 0");
        });
        builder.HasKey(x => x.SupplierProductID);
        builder.Property(x => x.SupplierSKUCode).HasMaxLength(80);
        builder.Property(x => x.LastPurchasePrice).HasPrecision(18, 2);
        builder.Property(x => x.MinimumOrderQuantity).HasPrecision(18, 3);
        builder.HasIndex(x => new { x.SupplierID, x.SKUID }).IsUnique();
        builder.HasOne(x => x.Supplier).WithMany(x => x.SupplierProducts).HasForeignKey(x => x.SupplierID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
    }
}
