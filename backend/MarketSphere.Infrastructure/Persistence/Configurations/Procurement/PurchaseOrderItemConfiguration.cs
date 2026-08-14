using MarketSphere.Domain.Entities.Procurement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Procurement;

public sealed class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.ToTable("PurchaseOrderItems", t =>
        {
            t.HasCheckConstraint("CK_PurchaseOrderItems_Quantities", "[OrderedQuantity] > 0 AND [ReceivedQuantity] >= 0 AND [ReceivedQuantity] <= [OrderedQuantity]");
            t.HasCheckConstraint("CK_PurchaseOrderItems_Amounts", "[UnitCost] >= 0 AND [DiscountAmount] >= 0 AND [TaxAmount] >= 0 AND [LineTotal] >= 0");
        });
        builder.HasKey(x => x.PurchaseOrderItemID);
        builder.Property(x => x.OrderedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.ReceivedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.UnitCost).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.LineTotal).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.PurchaseOrderID, x.SKUID }).IsUnique();
        builder.HasOne(x => x.PurchaseOrder).WithMany(x => x.Items).HasForeignKey(x => x.PurchaseOrderID).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
    }
}
