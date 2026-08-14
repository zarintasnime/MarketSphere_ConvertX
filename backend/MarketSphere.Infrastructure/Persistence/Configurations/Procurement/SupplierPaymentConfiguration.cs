using MarketSphere.Domain.Entities.Procurement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Procurement;

public sealed class SupplierPaymentConfiguration : IEntityTypeConfiguration<SupplierPayment>
{
    public void Configure(EntityTypeBuilder<SupplierPayment> builder)
    {
        builder.ToTable("SupplierPayments", t => t.HasCheckConstraint("CK_SupplierPayments_Amount", "[Amount] > 0"));
        builder.HasKey(x => x.SupplierPaymentID);
        builder.Property(x => x.PaymentNo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ReferenceNo).HasMaxLength(100);
        builder.Property(x => x.PaymentMethod).HasConversion<int>();
        builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.HasIndex(x => x.PaymentNo).IsUnique();
        builder.HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PurchaseInvoice).WithMany(x => x.Payments).HasForeignKey(x => x.PurchaseInvoiceID).OnDelete(DeleteBehavior.Restrict);
    }
}
