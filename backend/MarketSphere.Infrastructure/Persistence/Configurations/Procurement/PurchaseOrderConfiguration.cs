using MarketSphere.Domain.Entities.Procurement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Procurement;

public sealed class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrders", t =>
        {
            t.HasCheckConstraint("CK_PurchaseOrders_Amounts", "[GrossAmount] >= 0 AND [DiscountAmount] >= 0 AND [TaxAmount] >= 0 AND [NetAmount] >= 0");
        });
        builder.HasKey(x => x.PurchaseOrderID);
        builder.Property(x => x.PurchaseOrderNo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.GrossAmount).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.NetAmount).HasPrecision(18, 2);
        builder.HasIndex(x => x.PurchaseOrderNo).IsUnique();
        builder.HasIndex(x => new { x.SupplierID, x.Status, x.OrderDate });
        builder.HasOne(x => x.Supplier).WithMany(x => x.PurchaseOrders).HasForeignKey(x => x.SupplierID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PurchaseRequisition).WithMany(x => x.PurchaseOrders).HasForeignKey(x => x.PurchaseRequisitionID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchID).OnDelete(DeleteBehavior.Restrict);
    }
}
