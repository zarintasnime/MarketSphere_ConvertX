using MarketSphere.Domain.Entities.OrderFulfilment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrderFulfilment;

public sealed class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("InvoiceItems", t => t.HasCheckConstraint("CK_InvoiceItems_Values", "[Quantity] > 0 AND [UnitPrice] >= 0 AND [DiscountAmount] >= 0 AND [TaxAmount] >= 0 AND [LineTotal] >= 0"));
        builder.HasKey(x => x.InvoiceItemID);
        builder.Property(x => x.Quantity).HasPrecision(18, 3);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.LineTotal).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.InvoiceID, x.OrderItemID });
        builder.HasOne(x => x.Invoice).WithMany(x => x.Items).HasForeignKey(x => x.InvoiceID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.OrderItem).WithMany(x => x.InvoiceItems).HasForeignKey(x => x.OrderItemID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
    }
}
