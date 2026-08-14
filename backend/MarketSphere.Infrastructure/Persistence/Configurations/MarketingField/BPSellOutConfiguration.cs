using MarketSphere.Domain.Entities.MarketingField;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.MarketingField;

public sealed class BPSellOutConfiguration : IEntityTypeConfiguration<BPSellOut>
{
    public void Configure(EntityTypeBuilder<BPSellOut> b)
    {
        b.ToTable("BPSellOuts", t =>
        {
            t.HasCheckConstraint("CK_BPSellOuts_Totals", "[TotalQuantity] > 0 AND [TotalValue] >= 0");
            t.HasCheckConstraint("CK_BPSellOuts_Latitude", "[GPSLat] IS NULL OR ([GPSLat] >= -90 AND [GPSLat] <= 90)");
            t.HasCheckConstraint("CK_BPSellOuts_Longitude", "[GPSLng] IS NULL OR ([GPSLng] >= -180 AND [GPSLng] <= 180)");
        });
        b.HasKey(x => x.BPSellOutID);
        b.Property(x => x.TotalQuantity).HasPrecision(18, 3);
        b.Property(x => x.TotalValue).HasPrecision(18, 2);
        b.Property(x => x.GPSLat).HasPrecision(10, 7);
        b.Property(x => x.GPSLng).HasPrecision(10, 7);
        b.HasIndex(x => new { x.ClientID, x.SellOutDate });
        b.HasIndex(x => new { x.VerificationStatus, x.SellOutDate });
        b.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeID).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientID).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Visit).WithMany(x => x.BPSellOuts).HasForeignKey(x => x.VisitID).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Campaign).WithMany(x => x.BPSellOuts).HasForeignKey(x => x.CampaignID).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.VerifiedByEmployee).WithMany().HasForeignKey(x => x.VerifiedByEmployeeID).OnDelete(DeleteBehavior.Restrict);
    }
}
