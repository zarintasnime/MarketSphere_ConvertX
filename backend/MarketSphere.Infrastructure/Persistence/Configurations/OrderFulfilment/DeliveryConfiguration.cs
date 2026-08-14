using MarketSphere.Domain.Entities.OrderFulfilment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrderFulfilment;

public sealed class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        builder.ToTable("Deliveries");
        builder.HasKey(x => x.DeliveryID);
        builder.Property(x => x.DeliveryNo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.ReceiverName).HasMaxLength(200);
        builder.Property(x => x.ReceiverPhone).HasMaxLength(30);
        builder.Property(x => x.FailureReason).HasMaxLength(1000);
        builder.HasIndex(x => x.DeliveryNo).IsUnique();
        builder.HasIndex(x => new { x.Status, x.PlannedDeliveryDate });
        builder.HasOne(x => x.Order).WithMany(x => x.Deliveries).HasForeignKey(x => x.OrderID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Invoice).WithMany().HasForeignKey(x => x.InvoiceID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PickList).WithMany(x => x.Deliveries).HasForeignKey(x => x.PickListID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Warehouse).WithMany().HasForeignKey(x => x.WarehouseID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.DeliveredByEmployee).WithMany().HasForeignKey(x => x.DeliveredByEmployeeID).OnDelete(DeleteBehavior.Restrict);
    }
}
