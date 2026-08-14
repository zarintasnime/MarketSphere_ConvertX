using MarketSphere.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Inventory;

public sealed class StockTransferItemConfiguration : IEntityTypeConfiguration<StockTransferItem>
{
    public void Configure(EntityTypeBuilder<StockTransferItem> builder)
    {
        builder.ToTable("StockTransferItems", t =>
        {
            t.HasCheckConstraint("CK_StockTransferItems_Quantities", "[RequestedQuantity] > 0 AND [DispatchedQuantity] >= 0 AND [ReceivedQuantity] >= 0 AND [ReceivedQuantity] <= [DispatchedQuantity] AND [DispatchedQuantity] <= [RequestedQuantity]");
        });
        builder.HasKey(x => x.StockTransferItemID);
        builder.Property(x => x.RequestedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.DispatchedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.ReceivedQuantity).HasPrecision(18, 3);
        builder.HasIndex(x => new { x.StockTransferID, x.SKUID, x.BatchID }).IsUnique();
        builder.HasOne(x => x.StockTransfer).WithMany(x => x.Items).HasForeignKey(x => x.StockTransferID).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Batch).WithMany().HasForeignKey(x => x.BatchID).OnDelete(DeleteBehavior.Restrict);
    }
}
