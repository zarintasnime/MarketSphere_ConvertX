using MarketSphere.Domain.Entities.Procurement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Procurement;

public sealed class SupplierReturnItemConfiguration : IEntityTypeConfiguration<SupplierReturnItem>
{
    public void Configure(EntityTypeBuilder<SupplierReturnItem> builder)
    {
        builder.ToTable("SupplierReturnItems", t =>
        {
            t.HasCheckConstraint("CK_SupplierReturnItems_Quantity", "[Quantity] > 0");
            t.HasCheckConstraint("CK_SupplierReturnItems_UnitCost", "[UnitCost] >= 0");
        });
        builder.HasKey(x => x.SupplierReturnItemID);
        builder.Property(x => x.Quantity).HasPrecision(18, 3);
        builder.Property(x => x.UnitCost).HasPrecision(18, 2);
        builder.Property(x => x.Reason).HasMaxLength(1000).IsRequired();
        builder.HasIndex(x => new { x.SupplierReturnID, x.SKUID, x.BatchID }).IsUnique();
        builder.HasOne(x => x.SupplierReturn).WithMany(x => x.Items).HasForeignKey(x => x.SupplierReturnID).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Batch).WithMany().HasForeignKey(x => x.BatchID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.StockMovement).WithMany().HasForeignKey(x => x.StockMovementID).OnDelete(DeleteBehavior.Restrict);
    }
}
