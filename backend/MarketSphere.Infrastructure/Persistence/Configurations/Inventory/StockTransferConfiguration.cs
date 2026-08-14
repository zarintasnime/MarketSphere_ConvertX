using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketSphere.Domain.Entities.Inventory;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Inventory;

public sealed class StockTransferConfiguration : IEntityTypeConfiguration<StockTransfer>
{
    public void Configure(EntityTypeBuilder<StockTransfer> builder)
    {
        builder.ToTable("StockTransfers", t => t.HasCheckConstraint("CK_StockTransfers_DifferentWarehouses", "[FromWarehouseID] <> [ToWarehouseID]"));
        builder.HasKey(x => x.StockTransferID);
        builder.Property(x => x.StockTransferNo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>();
        builder.HasIndex(x => x.StockTransferNo).IsUnique();
        builder.HasIndex(x => new { x.FromWarehouseID, x.ToWarehouseID, x.Status, x.RequestedAt });
        builder.HasOne(x => x.FromWarehouse).WithMany().HasForeignKey(x => x.FromWarehouseID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ToWarehouse).WithMany().HasForeignKey(x => x.ToWarehouseID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ApprovalRequest).WithMany().HasForeignKey(x => x.ApprovalRequestID).OnDelete(DeleteBehavior.Restrict);
    }
}
