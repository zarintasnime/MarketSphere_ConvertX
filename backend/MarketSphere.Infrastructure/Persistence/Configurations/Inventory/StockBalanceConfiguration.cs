using MarketSphere.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Inventory;

public sealed class StockBalanceConfiguration : IEntityTypeConfiguration<StockBalance>
{
    public void Configure(EntityTypeBuilder<StockBalance> builder)
    {
        builder.ToTable("StockBalances", t =>
        {
            t.HasCheckConstraint("CK_StockBalances_NonNegative", "[OnHandQuantity] >= 0 AND [ReservedQuantity] >= 0 AND [QuarantineQuantity] >= 0 AND [DamagedQuantity] >= 0");
            t.HasCheckConstraint("CK_StockBalances_Allocations", "([ReservedQuantity] + [QuarantineQuantity] + [DamagedQuantity]) <= [OnHandQuantity]");
        });
        builder.HasKey(x => x.StockBalanceID);
        builder.Property(x => x.OnHandQuantity).HasPrecision(18, 3);
        builder.Property(x => x.ReservedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.QuarantineQuantity).HasPrecision(18, 3);
        builder.Property(x => x.DamagedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => new { x.WarehouseID, x.SKUID, x.BatchID }).IsUnique();
        builder.HasOne(x => x.Warehouse).WithMany().HasForeignKey(x => x.WarehouseID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Batch).WithMany().HasForeignKey(x => x.BatchID).OnDelete(DeleteBehavior.Restrict);
    }
}
