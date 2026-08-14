using MarketSphere.Domain.Entities.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.CRM;

public sealed class QuotationItemConfiguration : IEntityTypeConfiguration<QuotationItem>
{
    public void Configure(EntityTypeBuilder<QuotationItem> builder)
    {
        builder.ToTable("QuotationItems", table =>
        {
            table.HasCheckConstraint("CK_QuotationItems_Quantity", "[Quantity] > 0");
            table.HasCheckConstraint(
                "CK_QuotationItems_Discount",
                "[DiscountPercent] >= 0 AND [DiscountPercent] <= 100");
            table.HasCheckConstraint(
                "CK_QuotationItems_Amounts",
                "[UnitPrice] >= 0 AND [DiscountAmount] >= 0 AND [TaxAmount] >= 0 AND [LineTotal] >= 0");
        });
        builder.HasKey(x => x.QuotationItemID);
        builder.Property(x => x.Quantity).HasPrecision(18, 3);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.DiscountPercent).HasPrecision(5, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.LineTotal).HasPrecision(18, 2);
        builder.Property(x => x.Note).HasMaxLength(500);
        builder.HasIndex(x => new { x.QuotationID, x.SKUID });
        builder.HasOne(x => x.Quotation)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.QuotationID)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.SKU)
            .WithMany()
            .HasForeignKey(x => x.SKUID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
