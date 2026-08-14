using MarketSphere.Domain.Entities.OrderFulfilment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrderFulfilment;

public sealed class PickListConfiguration : IEntityTypeConfiguration<PickList>
{
    public void Configure(EntityTypeBuilder<PickList> builder)
    {
        builder.ToTable("PickLists");
        builder.HasKey(x => x.PickListID);
        builder.Property(x => x.PickListNo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.WaveNo).HasMaxLength(50);
        builder.Property(x => x.Note).HasMaxLength(1000);
        builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => x.PickListNo).IsUnique();
        builder.HasIndex(x => new { x.WarehouseID, x.Status, x.ReleasedAt });
        builder.HasOne(x => x.Order).WithMany(x => x.PickLists).HasForeignKey(x => x.OrderID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Invoice).WithMany().HasForeignKey(x => x.InvoiceID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Warehouse).WithMany().HasForeignKey(x => x.WarehouseID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ReleasedByEmployee).WithMany().HasForeignKey(x => x.ReleasedByEmployeeID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.VerifiedByEmployee).WithMany().HasForeignKey(x => x.VerifiedByEmployeeID).OnDelete(DeleteBehavior.Restrict);
    }
}
