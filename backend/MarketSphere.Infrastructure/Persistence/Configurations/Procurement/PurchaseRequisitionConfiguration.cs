using MarketSphere.Domain.Entities.Procurement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Procurement;

public sealed class PurchaseRequisitionConfiguration : IEntityTypeConfiguration<PurchaseRequisition>
{
    public void Configure(EntityTypeBuilder<PurchaseRequisition> builder)
    {
        builder.ToTable("PurchaseRequisitions");
        builder.HasKey(x => x.PurchaseRequisitionID);
        builder.Property(x => x.PurchaseRequisitionNo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.Property(x => x.Status).HasConversion<int>();
        builder.HasIndex(x => x.PurchaseRequisitionNo).IsUnique();
        builder.HasIndex(x => new { x.BranchID, x.Status, x.RequiredDate });
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RequestedByEmployee).WithMany().HasForeignKey(x => x.RequestedByEmployeeID).OnDelete(DeleteBehavior.Restrict);
    }
}
