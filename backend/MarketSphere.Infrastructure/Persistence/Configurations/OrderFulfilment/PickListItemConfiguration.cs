using MarketSphere.Domain.Entities.OrderFulfilment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrderFulfilment;

public sealed class PickListItemConfiguration : IEntityTypeConfiguration<PickListItem>
{
    public void Configure(EntityTypeBuilder<PickListItem> builder)
    {
        builder.ToTable("PickListItems", t =>
        {
            t.HasCheckConstraint("CK_PickListItems_Quantities", "[RequestedQuantity] > 0 AND [PickedQuantity] >= 0 AND [ShortQuantity] >= 0");
            t.HasCheckConstraint("CK_PickListItems_Total", "[PickedQuantity] + [ShortQuantity] <= [RequestedQuantity]");
        });
        builder.HasKey(x => x.PickListItemID);
        builder.Property(x => x.RequestedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.PickedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.ShortQuantity).HasPrecision(18, 3);
        builder.Property(x => x.VerificationNote).HasMaxLength(1000);
        builder.HasIndex(x => new { x.PickListID, x.StockReservationID });
        builder.HasOne(x => x.PickList).WithMany(x => x.Items).HasForeignKey(x => x.PickListID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.OrderItem).WithMany(x => x.PickListItems).HasForeignKey(x => x.OrderItemID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.StockReservation).WithMany().HasForeignKey(x => x.StockReservationID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Batch).WithMany().HasForeignKey(x => x.BatchID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PickedByEmployee).WithMany().HasForeignKey(x => x.PickedByEmployeeID).OnDelete(DeleteBehavior.Restrict);
    }
}
