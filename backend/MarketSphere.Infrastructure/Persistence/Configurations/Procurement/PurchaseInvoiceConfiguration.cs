using MarketSphere.Domain.Entities.Procurement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Procurement;

public sealed class PurchaseInvoiceConfiguration : IEntityTypeConfiguration<PurchaseInvoice>
{
    public void Configure(EntityTypeBuilder<PurchaseInvoice> builder)
    {
        builder.ToTable("PurchaseInvoices", t =>
        {
            t.HasCheckConstraint("CK_PurchaseInvoices_Amounts", "[GrossAmount] >= 0 AND [DiscountAmount] >= 0 AND [TaxAmount] >= 0 AND [TotalAmount] >= 0 AND [PaidAmount] >= 0 AND [DueAmount] >= 0 AND ([PaidAmount] + [DueAmount]) = [TotalAmount]");
        });
        builder.HasKey(x => x.PurchaseInvoiceID);
        builder.Property(x => x.PurchaseInvoiceNo).HasMaxLength(80).IsRequired();
        builder.Property(x => x.GrossAmount).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.PaidAmount).HasPrecision(18, 2);
        builder.Property(x => x.DueAmount).HasPrecision(18, 2);
        builder.Property(x => x.PaymentStatus).HasConversion<int>();
        builder.Property(x => x.Status).HasConversion<int>();
        builder.HasIndex(x => new { x.SupplierID, x.PurchaseInvoiceNo }).IsUnique();
        builder.HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PurchaseOrder).WithMany().HasForeignKey(x => x.PurchaseOrderID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.GoodsReceipt).WithMany().HasForeignKey(x => x.GoodsReceiptID).OnDelete(DeleteBehavior.Restrict);
    }
}
