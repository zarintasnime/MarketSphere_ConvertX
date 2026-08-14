using MarketSphere.Domain.Entities.OrderFulfilment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrderFulfilment;

public sealed class ReturnItemConfiguration : IEntityTypeConfiguration<ReturnItem>
{
    public void Configure(EntityTypeBuilder<ReturnItem> builder)
    {
        builder.ToTable("ReturnItems", t =>
        {
            t.HasCheckConstraint("CK_ReturnItems_Quantities", "[RequestedQuantity] > 0 AND [ApprovedQuantity] >= 0 AND [ReceivedQuantity] >= 0 AND [RestockQuantity] >= 0 AND [QuarantineQuantity] >= 0 AND [DamageQuantity] >= 0 AND [ReplacementQuantity] >= 0");
            t.HasCheckConstraint("CK_ReturnItems_Disposition", "[RestockQuantity] + [QuarantineQuantity] + [DamageQuantity] + [ReplacementQuantity] = [ReceivedQuantity]");
        });
        builder.HasKey(x => x.ReturnItemID);
        builder.Property(x => x.RequestedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.ApprovedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.ReceivedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.RestockQuantity).HasPrecision(18, 3);
        builder.Property(x => x.QuarantineQuantity).HasPrecision(18, 3);
        builder.Property(x => x.DamageQuantity).HasPrecision(18, 3);
        builder.Property(x => x.ReplacementQuantity).HasPrecision(18, 3);
        builder.Property(x => x.CreditAmount).HasPrecision(18, 2);
        builder.Property(x => x.ConditionStatus).HasConversion<int>();
        builder.Property(x => x.Disposition).HasConversion<int>();
        builder.Property(x => x.InspectionResult).HasMaxLength(1000);
        builder.HasOne(x => x.ReturnRequest).WithMany(x => x.Items).HasForeignKey(x => x.ReturnRequestID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.DeliveryItem).WithMany(x => x.ReturnItems).HasForeignKey(x => x.DeliveryItemID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Batch).WithMany().HasForeignKey(x => x.BatchID).OnDelete(DeleteBehavior.Restrict);
    }
}
