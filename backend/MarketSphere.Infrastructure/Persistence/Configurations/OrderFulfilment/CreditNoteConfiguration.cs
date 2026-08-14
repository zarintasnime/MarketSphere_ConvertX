using MarketSphere.Domain.Entities.OrderFulfilment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrderFulfilment;

public sealed class CreditNoteConfiguration : IEntityTypeConfiguration<CreditNote>
{
    public void Configure(EntityTypeBuilder<CreditNote> builder)
    {
        builder.ToTable("CreditNotes", t => t.HasCheckConstraint("CK_CreditNotes_Amount", "[Amount] > 0"));
        builder.HasKey(x => x.CreditNoteID);
        builder.Property(x => x.CreditNoteNo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.HasIndex(x => x.CreditNoteNo).IsUnique();
        builder.HasIndex(x => x.ReturnRequestID).IsUnique();
        builder.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Invoice).WithMany(x => x.CreditNotes).HasForeignKey(x => x.InvoiceID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ReturnRequest).WithOne(x => x.CreditNote).HasForeignKey<CreditNote>(x => x.ReturnRequestID).OnDelete(DeleteBehavior.Restrict);
    }
}
