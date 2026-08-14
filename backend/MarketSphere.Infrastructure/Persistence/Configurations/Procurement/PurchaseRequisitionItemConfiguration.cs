using MarketSphere.Domain.Entities.Procurement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Procurement;

public sealed class PurchaseRequisitionItemConfiguration : IEntityTypeConfiguration<PurchaseRequisitionItem>
{
    public void Configure(EntityTypeBuilder<PurchaseRequisitionItem> builder)
    {
        builder.ToTable("PurchaseRequisitionItems", t =>
        {
            t.HasCheckConstraint("CK_PurchaseRequisitionItems_Quantity", "[RequestedQuantity] > 0");
            t.HasCheckConstraint("CK_PurchaseRequisitionItems_Cost", "[EstimatedUnitCost] IS NULL OR [EstimatedUnitCost] >= 0");
        });
        builder.HasKey(x => x.PurchaseRequisitionItemID);
        builder.Property(x => x.RequestedQuantity).HasPrecision(18, 3);
        builder.Property(x => x.EstimatedUnitCost).HasPrecision(18, 2);
        builder.Property(x => x.Note).HasMaxLength(1000);
        builder.HasIndex(x => new { x.PurchaseRequisitionID, x.SKUID }).IsUnique();
        builder.HasOne(x => x.PurchaseRequisition).WithMany(x => x.Items).HasForeignKey(x => x.PurchaseRequisitionID).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
    }
}
