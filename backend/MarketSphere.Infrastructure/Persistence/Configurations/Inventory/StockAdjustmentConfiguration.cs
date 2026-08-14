using MarketSphere.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Inventory;

public sealed class StockAdjustmentConfiguration : IEntityTypeConfiguration<StockAdjustment>
{
    public void Configure(EntityTypeBuilder<StockAdjustment> builder)
    {
        builder.ToTable("StockAdjustments");
        builder.HasKey(x => x.StockAdjustmentID);
        builder.Property(x => x.StockAdjustmentNo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Reason).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>();
        builder.HasIndex(x => x.StockAdjustmentNo).IsUnique();
        builder.HasIndex(x => new { x.WarehouseID, x.Status, x.AdjustmentDate });
        builder.HasOne(x => x.Warehouse).WithMany().HasForeignKey(x => x.WarehouseID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PerformedByEmployee).WithMany().HasForeignKey(x => x.PerformedByEmployeeID).OnDelete(DeleteBehavior.Restrict);
    }
}
