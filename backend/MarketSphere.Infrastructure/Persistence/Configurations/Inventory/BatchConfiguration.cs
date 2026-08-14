using MarketSphere.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Inventory;

public sealed class BatchConfiguration : IEntityTypeConfiguration<Batch>
{
    public void Configure(EntityTypeBuilder<Batch> builder)
    {
        builder.ToTable("Batches", t =>
        {
            t.HasCheckConstraint("CK_Batches_CostPrice", "[CostPrice] >= 0");
            t.HasCheckConstraint("CK_Batches_Dates", "[ExpiryDate] IS NULL OR [ManufacturingDate] IS NULL OR [ExpiryDate] >= [ManufacturingDate]");
        });
        builder.HasKey(x => x.BatchID);
        builder.Property(x => x.BatchNo).HasMaxLength(80).IsRequired();
        builder.Property(x => x.CostPrice).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasConversion<int>();
        builder.HasIndex(x => new { x.SKUID, x.BatchNo }).IsUnique();
        builder.HasIndex(x => new { x.ExpiryDate, x.Status });
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
    }
}
