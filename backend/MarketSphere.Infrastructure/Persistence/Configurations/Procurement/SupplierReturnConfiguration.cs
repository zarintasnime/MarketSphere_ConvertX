using MarketSphere.Domain.Entities.Procurement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Procurement;

public sealed class SupplierReturnConfiguration : IEntityTypeConfiguration<SupplierReturn>
{
    public void Configure(EntityTypeBuilder<SupplierReturn> builder)
    {
        builder.ToTable("SupplierReturns");
        builder.HasKey(x => x.SupplierReturnID);
        builder.Property(x => x.SupplierReturnNo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Reason).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>();
        builder.HasIndex(x => x.SupplierReturnNo).IsUnique();
        builder.HasIndex(x => new { x.SupplierID, x.Status, x.ReturnDate });
        builder.HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.GoodsReceipt).WithMany().HasForeignKey(x => x.GoodsReceiptID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Warehouse).WithMany().HasForeignKey(x => x.WarehouseID).OnDelete(DeleteBehavior.Restrict);
    }
}
