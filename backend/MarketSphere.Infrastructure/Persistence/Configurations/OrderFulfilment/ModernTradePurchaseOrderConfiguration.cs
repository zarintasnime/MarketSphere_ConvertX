using MarketSphere.Domain.Entities.OrderFulfilment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrderFulfilment;

public sealed class ModernTradePurchaseOrderConfiguration : IEntityTypeConfiguration<ModernTradePurchaseOrder>
{
    public void Configure(EntityTypeBuilder<ModernTradePurchaseOrder> builder)
    {
        builder.ToTable("ModernTradePurchaseOrders", t => t.HasCheckConstraint("CK_ModernTradePurchaseOrders_Dates", "[ReceivedDate] >= [PODate]"));
        builder.HasKey(x => x.ModernTradePurchaseOrderID);
        builder.Property(x => x.PONumber).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.VerificationStatus).HasConversion<int>();
        builder.Property(x => x.CompletenessStatus).HasConversion<int>();
        builder.Property(x => x.VerificationNote).HasMaxLength(1000);
        builder.Property(x => x.RejectionReason).HasMaxLength(1000);
        builder.Property(x => x.DuplicateHash).HasMaxLength(128);
        builder.HasIndex(x => new { x.ClientID, x.PONumber }).IsUnique();
        builder.HasIndex(x => x.DuplicateHash).IsUnique().HasFilter("[DuplicateHash] IS NOT NULL");
        builder.HasIndex(x => new { x.VerificationStatus, x.Status, x.ReceivedDate });
        builder.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.UploadedByEmployee).WithMany().HasForeignKey(x => x.UploadedByEmployeeID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.VerifiedByEmployee).WithMany().HasForeignKey(x => x.VerifiedByEmployeeID).OnDelete(DeleteBehavior.Restrict);
    }
}
