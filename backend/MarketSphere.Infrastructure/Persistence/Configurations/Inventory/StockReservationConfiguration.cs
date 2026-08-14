using MarketSphere.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Inventory;

public sealed class StockReservationConfiguration : IEntityTypeConfiguration<StockReservation>
{
    public void Configure(EntityTypeBuilder<StockReservation> builder)
    {
        builder.ToTable("StockReservations", t => t.HasCheckConstraint("CK_StockReservations_Quantity", "[ReservedQuantity] > 0"));
        builder.HasKey(x => x.StockReservationID);
        builder.Property(x => x.ReservedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.ReservationStatus).HasConversion<int>();
        builder.HasIndex(x => new { x.OrderItemID, x.ReservationStatus });
        builder.HasIndex(x => new { x.WarehouseID, x.SKUID, x.BatchID, x.ReservationStatus });
        builder.HasOne(x => x.OrderItem).WithMany(x => x.StockReservations).HasForeignKey(x => x.OrderItemID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Warehouse).WithMany().HasForeignKey(x => x.WarehouseID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Batch).WithMany().HasForeignKey(x => x.BatchID).OnDelete(DeleteBehavior.Restrict);
    }
}
