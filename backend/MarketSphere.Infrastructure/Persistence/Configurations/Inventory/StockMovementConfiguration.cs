using MarketSphere.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Inventory;

public sealed class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovements", t =>
        {
            t.HasCheckConstraint("CK_StockMovements_OneDirection", "([QuantityIn] > 0 AND [QuantityOut] = 0) OR ([QuantityOut] > 0 AND [QuantityIn] = 0)");
            t.HasCheckConstraint("CK_StockMovements_Balance", "[BalanceAfter] >= 0");
        });
        builder.HasKey(x => x.StockMovementID);
        builder.Property(x => x.MovementType).HasConversion<int>();
        builder.Property(x => x.QuantityIn).HasPrecision(18, 3);
        builder.Property(x => x.QuantityOut).HasPrecision(18, 3);
        builder.Property(x => x.BalanceAfter).HasPrecision(18, 3);
        builder.Property(x => x.ReferenceType).HasMaxLength(60).IsRequired();
        builder.Property(x => x.Note).HasMaxLength(1000);
        builder.HasIndex(x => new { x.WarehouseID, x.SKUID, x.BatchID, x.MovementAt });
        builder.HasIndex(x => new { x.ReferenceType, x.ReferenceID });
        builder.HasOne(x => x.Warehouse).WithMany().HasForeignKey(x => x.WarehouseID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Batch).WithMany().HasForeignKey(x => x.BatchID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PerformedByUser).WithMany().HasForeignKey(x => x.PerformedByUserID).OnDelete(DeleteBehavior.Restrict);
    }
}
