using MarketSphere.Domain.Entities.OrderFulfilment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrderFulfilment;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices", t =>
        {
            t.HasCheckConstraint("CK_Invoices_Amounts", "[GrossAmount] >= 0 AND [DiscountAmount] >= 0 AND [TaxAmount] >= 0 AND [TotalAmount] >= 0 AND [PaidAmount] >= 0 AND [DueAmount] >= 0");
            t.HasCheckConstraint("CK_Invoices_Balance", "[PaidAmount] + [DueAmount] = [TotalAmount]");
        });
        builder.HasKey(x => x.InvoiceID);
        builder.Property(x => x.InvoiceNo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.GrossAmount).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.PaidAmount).HasPrecision(18, 2);
        builder.Property(x => x.DueAmount).HasPrecision(18, 2);
        builder.HasIndex(x => x.InvoiceNo).IsUnique();
        builder.HasIndex(x => new { x.ClientID, x.Status, x.DueDate });
        builder.HasOne(x => x.Order).WithMany(x => x.Invoices).HasForeignKey(x => x.OrderID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientID).OnDelete(DeleteBehavior.Restrict);
    }
}
