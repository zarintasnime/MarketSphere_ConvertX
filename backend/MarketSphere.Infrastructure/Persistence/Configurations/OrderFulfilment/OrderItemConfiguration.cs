using MarketSphere.Domain.Entities.OrderFulfilment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrderFulfilment;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems", t =>
        {
            t.HasCheckConstraint("CK_OrderItems_Quantities", "[OrderedQuantity] > 0 AND [FreeQuantity] >= 0 AND [ApprovedQuantity] >= 0 AND [DeliveredQuantity] >= 0 AND [ReturnedQuantity] >= 0 AND [BackorderQuantity] >= 0");
            t.HasCheckConstraint("CK_OrderItems_Amounts", "[UnitPrice] >= 0 AND [DiscountPercent] BETWEEN 0 AND 100 AND [DiscountAmount] >= 0 AND [TaxAmount] >= 0 AND [LineTotal] >= 0");
        });
        builder.HasKey(x => x.OrderItemID);
        builder.Property(x => x.OrderedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.FreeQuantity).HasPrecision(18, 3);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.DiscountPercent).HasPrecision(5, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.LineTotal).HasPrecision(18, 2);
        builder.Property(x => x.ApprovedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.DeliveredQuantity).HasPrecision(18, 3);
        builder.Property(x => x.ReturnedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.BackorderQuantity).HasPrecision(18, 3);
        builder.HasIndex(x => new { x.OrderID, x.SKUID }).IsUnique();
        builder.HasOne(x => x.Order).WithMany(x => x.Items).HasForeignKey(x => x.OrderID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
    }
}
