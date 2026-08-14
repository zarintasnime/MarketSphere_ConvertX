using MarketSphere.Domain.Entities.Procurement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Procurement;

public sealed class GoodsReceiptConfiguration : IEntityTypeConfiguration<GoodsReceipt>
{
    public void Configure(EntityTypeBuilder<GoodsReceipt> builder)
    {
        builder.ToTable("GoodsReceipts");
        builder.HasKey(x => x.GoodsReceiptID);
        builder.Property(x => x.GoodsReceiptNo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.SupplierChallanNo).HasMaxLength(80);
        builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.QualityCheckStatus).HasConversion<int>();
        builder.HasIndex(x => x.GoodsReceiptNo).IsUnique();
        builder.HasIndex(x => new { x.PurchaseOrderID, x.Status, x.ReceivedDate });
        builder.HasOne(x => x.PurchaseOrder).WithMany(x => x.GoodsReceipts).HasForeignKey(x => x.PurchaseOrderID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Warehouse).WithMany().HasForeignKey(x => x.WarehouseID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ReceivedByEmployee).WithMany().HasForeignKey(x => x.ReceivedByEmployeeID).OnDelete(DeleteBehavior.Restrict);
    }
}
