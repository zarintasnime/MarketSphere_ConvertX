using MarketSphere.Domain.Entities.OrderFulfilment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrderFulfilment;

public sealed class DeliveryItemConfiguration : IEntityTypeConfiguration<DeliveryItem>
{
    public void Configure(EntityTypeBuilder<DeliveryItem> builder)
    {
        builder.ToTable("DeliveryItems", t =>
        {
            t.HasCheckConstraint("CK_DeliveryItems_Quantities", "[QuantityDispatched] > 0 AND [QuantityDelivered] >= 0 AND [QuantityRejectedAtDelivery] >= 0");
            t.HasCheckConstraint("CK_DeliveryItems_Total", "[QuantityDelivered] + [QuantityRejectedAtDelivery] <= [QuantityDispatched]");
        });
        builder.HasKey(x => x.DeliveryItemID);
        builder.Property(x => x.QuantityDispatched).HasPrecision(18, 3);
        builder.Property(x => x.QuantityDelivered).HasPrecision(18, 3);
        builder.Property(x => x.QuantityRejectedAtDelivery).HasPrecision(18, 3);
        builder.HasOne(x => x.Delivery).WithMany(x => x.Items).HasForeignKey(x => x.DeliveryID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PickListItem).WithMany(x => x.DeliveryItems).HasForeignKey(x => x.PickListItemID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.OrderItem).WithMany(x => x.DeliveryItems).HasForeignKey(x => x.OrderItemID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.InvoiceItem).WithMany().HasForeignKey(x => x.InvoiceItemID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Batch).WithMany().HasForeignKey(x => x.BatchID).OnDelete(DeleteBehavior.Restrict);
    }
}
