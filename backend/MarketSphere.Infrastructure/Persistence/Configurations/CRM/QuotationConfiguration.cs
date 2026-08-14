using MarketSphere.Domain.Entities.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.CRM;

public sealed class QuotationConfiguration : IEntityTypeConfiguration<Quotation>
{
    public void Configure(EntityTypeBuilder<Quotation> builder)
    {
        builder.ToTable("Quotations", table =>
        {
            table.HasCheckConstraint("CK_Quotations_DateRange", "[ValidUntil] >= [ValidFrom]");
            table.HasCheckConstraint(
                "CK_Quotations_Amounts",
                "[GrossAmount] >= 0 AND [DiscountAmount] >= 0 AND [TaxAmount] >= 0 AND [NetAmount] >= 0");
        });
        builder.HasKey(x => x.QuotationID);
        builder.Property(x => x.QuotationNo).HasMaxLength(40).IsRequired();
        builder.Property(x => x.GrossAmount).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.NetAmount).HasPrecision(18, 2);
        builder.Property(x => x.Terms).HasColumnType("nvarchar(max)");
        builder.HasIndex(x => new { x.QuotationNo, x.VersionNo }).IsUnique();
        builder.HasIndex(x => new { x.ClientID, x.Status, x.ValidUntil });
        builder.HasOne(x => x.RootQuotation)
            .WithMany(x => x.Versions)
            .HasForeignKey(x => x.RootQuotationID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Opportunity)
            .WithMany(x => x.Quotations)
            .HasForeignKey(x => x.OpportunityID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Campaign)
            .WithMany()
            .HasForeignKey(x => x.CampaignID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PriceList)
            .WithMany()
            .HasForeignKey(x => x.PriceListID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
