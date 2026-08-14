using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketSphere.Domain.Entities.OrderFulfilment;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrderFulfilment;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments", t => t.HasCheckConstraint("CK_Payments_Amount", "[Amount] > 0"));
        builder.HasKey(x => x.PaymentID);
        builder.Property(x => x.PaymentNo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.PaymentMethod).HasConversion<int>();
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.ReferenceNo).HasMaxLength(100);
        builder.Property(x => x.Status).HasConversion<int>();
        builder.HasIndex(x => x.PaymentNo).IsUnique();
        builder.HasIndex(x => new { x.ClientID, x.Status, x.PaymentDate });
        builder.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ReceivedByUser).WithMany().HasForeignKey(x => x.ReceivedByUserID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ProofFileAttachment).WithMany().HasForeignKey(x => x.ProofFileAttachmentID).OnDelete(DeleteBehavior.Restrict);
    }
}
