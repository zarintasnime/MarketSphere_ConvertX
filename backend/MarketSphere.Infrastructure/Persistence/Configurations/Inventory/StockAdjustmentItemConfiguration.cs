using MarketSphere.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Inventory;

public sealed class StockAdjustmentItemConfiguration : IEntityTypeConfiguration<StockAdjustmentItem>
{
    public void Configure(EntityTypeBuilder<StockAdjustmentItem> builder)
    {
        builder.ToTable("StockAdjustmentItems", t =>
        {
            t.HasCheckConstraint("CK_StockAdjustmentItems_Quantity", "[AdjustmentQuantity] <> 0");
            t.HasCheckConstraint("CK_StockAdjustmentItems_UnitCost", "[UnitCost] IS NULL OR [UnitCost] >= 0");
        });
        builder.HasKey(x => x.StockAdjustmentItemID);
        builder.Property(x => x.AdjustmentQuantity).HasPrecision(18, 3);
        builder.Property(x => x.UnitCost).HasPrecision(18, 2);
        builder.Property(x => x.Note).HasMaxLength(1000);
        builder.HasIndex(x => new { x.StockAdjustmentID, x.SKUID, x.BatchID }).IsUnique();
        builder.HasOne(x => x.StockAdjustment).WithMany(x => x.Items).HasForeignKey(x => x.StockAdjustmentID).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Batch).WithMany().HasForeignKey(x => x.BatchID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.StockMovement).WithMany().HasForeignKey(x => x.StockMovementID).OnDelete(DeleteBehavior.Restrict);
    }
}
