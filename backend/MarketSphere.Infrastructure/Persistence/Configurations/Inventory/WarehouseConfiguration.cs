using MarketSphere.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Inventory;

public sealed class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses");
        builder.HasKey(x => x.WarehouseID);
        builder.Property(x => x.WarehouseCode).HasMaxLength(40).IsRequired();
        builder.Property(x => x.WarehouseName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.WarehouseType).HasConversion<int>();
        builder.HasIndex(x => new { x.BranchID, x.WarehouseCode }).IsUnique();
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchID).OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
