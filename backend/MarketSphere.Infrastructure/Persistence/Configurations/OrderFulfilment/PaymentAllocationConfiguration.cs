using MarketSphere.Domain.Entities.OrderFulfilment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrderFulfilment;

public sealed class PaymentAllocationConfiguration : IEntityTypeConfiguration<PaymentAllocation>
{
    public void Configure(EntityTypeBuilder<PaymentAllocation> builder)
    {
        builder.ToTable("PaymentAllocations", t => t.HasCheckConstraint("CK_PaymentAllocations_Amount", "[AllocatedAmount] > 0"));
        builder.HasKey(x => x.PaymentAllocationID);
        builder.Property(x => x.AllocationType).HasConversion<int>();
        builder.Property(x => x.AllocatedAmount).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.PaymentID, x.InvoiceID, x.AllocationType });
        builder.HasIndex(x => x.ReversalOfPaymentAllocationID).IsUnique().HasFilter("[ReversalOfPaymentAllocationID] IS NOT NULL");
        builder.HasOne(x => x.Payment).WithMany(x => x.Allocations).HasForeignKey(x => x.PaymentID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Invoice).WithMany(x => x.PaymentAllocations).HasForeignKey(x => x.InvoiceID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ReversalOfPaymentAllocation).WithMany(x => x.Reversals).HasForeignKey(x => x.ReversalOfPaymentAllocationID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AllocatedByUser).WithMany().HasForeignKey(x => x.AllocatedByUserID).OnDelete(DeleteBehavior.Restrict);
    }
}
