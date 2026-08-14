using MarketSphere.Domain.Entities.OrderFulfilment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrderFulfilment;

public sealed class ModernTradePurchaseOrderItemConfiguration : IEntityTypeConfiguration<ModernTradePurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<ModernTradePurchaseOrderItem> builder)
    {
        builder.ToTable("ModernTradePurchaseOrderItems", t => t.HasCheckConstraint("CK_ModernTradePurchaseOrderItems_Quantity", "[OrderedQuantity] > 0 AND ([AgreedUnitPrice] IS NULL OR [AgreedUnitPrice] >= 0) AND ([Discount] IS NULL OR [Discount] >= 0)"));
        builder.HasKey(x => x.ModernTradePurchaseOrderItemID);
        builder.Property(x => x.ExternalItemCode).HasMaxLength(100);
        builder.Property(x => x.ExternalItemName).HasMaxLength(250);
        builder.Property(x => x.MappingStatus).HasConversion<int>();
        builder.Property(x => x.OrderedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.AgreedUnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.Discount).HasPrecision(18, 2);
        builder.Property(x => x.Note).HasMaxLength(1000);
        builder.HasIndex(x => new { x.ModernTradePurchaseOrderID, x.ExternalItemCode });
        builder.HasOne(x => x.ModernTradePurchaseOrder).WithMany(x => x.Items).HasForeignKey(x => x.ModernTradePurchaseOrderID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
    }
}
