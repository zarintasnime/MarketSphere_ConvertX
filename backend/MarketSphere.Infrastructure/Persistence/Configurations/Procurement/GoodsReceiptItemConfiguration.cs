using MarketSphere.Domain.Entities.Procurement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Procurement;

public sealed class GoodsReceiptItemConfiguration : IEntityTypeConfiguration<GoodsReceiptItem>
{
    public void Configure(EntityTypeBuilder<GoodsReceiptItem> builder)
    {
        builder.ToTable("GoodsReceiptItems", t =>
        {
            t.HasCheckConstraint("CK_GoodsReceiptItems_Quantities", "[AcceptedQuantity] >= 0 AND [RejectedQuantity] >= 0 AND ([AcceptedQuantity] + [RejectedQuantity]) > 0");
            t.HasCheckConstraint("CK_GoodsReceiptItems_UnitCost", "[UnitCost] >= 0");
            t.HasCheckConstraint("CK_GoodsReceiptItems_RejectionReason", "[RejectedQuantity] = 0 OR [RejectionReason] IS NOT NULL");
        });
        builder.HasKey(x => x.GoodsReceiptItemID);
        builder.Property(x => x.AcceptedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.RejectedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.UnitCost).HasPrecision(18, 2);
        builder.Property(x => x.BatchNo).HasMaxLength(80);
        builder.Property(x => x.RejectionReason).HasMaxLength(1000);
        builder.HasIndex(x => new { x.GoodsReceiptID, x.PurchaseOrderItemID }).IsUnique();
        builder.HasOne(x => x.GoodsReceipt).WithMany(x => x.Items).HasForeignKey(x => x.GoodsReceiptID).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.PurchaseOrderItem).WithMany(x => x.GoodsReceiptItems).HasForeignKey(x => x.PurchaseOrderItemID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Batch).WithMany().HasForeignKey(x => x.BatchID).OnDelete(DeleteBehavior.Restrict);
    }
}
