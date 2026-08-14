using MarketSphere.Domain.Entities.MarketingField;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.MarketingField;

public sealed class BPSellOutItemConfiguration : IEntityTypeConfiguration<BPSellOutItem>
{
    public void Configure(EntityTypeBuilder<BPSellOutItem> builder)
    {
        builder.ToTable("BPSellOutItems", table =>
        {
            table.HasCheckConstraint("CK_BPSellOutItems_Quantity", "[QuantitySold] > 0");
            table.HasCheckConstraint("CK_BPSellOutItems_Price", "[UnitSellingPrice] IS NULL OR [UnitSellingPrice] >= 0");
            table.HasCheckConstraint("CK_BPSellOutItems_Value", "[LineValue] IS NULL OR [LineValue] >= 0");
        });
        builder.HasKey(x => x.BPSellOutItemID);
        builder.Property(x => x.QuantitySold).HasPrecision(18, 3);
        builder.Property(x => x.UnitSellingPrice).HasPrecision(18, 2);
        builder.Property(x => x.LineValue).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.BPSellOutID, x.SKUID }).IsUnique();
        builder.HasOne(x => x.BPSellOut).WithMany(x => x.Items).HasForeignKey(x => x.BPSellOutID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
    }
}
