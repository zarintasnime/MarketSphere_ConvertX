using MarketSphere.Domain.Entities.OrderFulfilment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrderFulfilment;

public sealed class ReturnRequestConfiguration : IEntityTypeConfiguration<ReturnRequest>
{
    public void Configure(EntityTypeBuilder<ReturnRequest> builder)
    {
        builder.ToTable("ReturnRequests");
        builder.HasKey(x => x.ReturnRequestID);
        builder.Property(x => x.ReturnNo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ReturnReason).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.ResolutionType).HasConversion<int>();
        builder.Property(x => x.ResolutionNote).HasMaxLength(2000);
        builder.HasIndex(x => x.ReturnNo).IsUnique();
        builder.HasIndex(x => new { x.ClientID, x.Status, x.RequestDate });
        builder.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Invoice).WithMany().HasForeignKey(x => x.InvoiceID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Delivery).WithMany().HasForeignKey(x => x.DeliveryID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Complaint).WithMany().HasForeignKey(x => x.ComplaintID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ReplacementOrder).WithMany().HasForeignKey(x => x.ReplacementOrderID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ReplacementDelivery).WithMany().HasForeignKey(x => x.ReplacementDeliveryID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SupplierReturn).WithMany().HasForeignKey(x => x.SupplierReturnID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ResolvedByEmployee).WithMany().HasForeignKey(x => x.ResolvedByEmployeeID).OnDelete(DeleteBehavior.Restrict);
    }
}
