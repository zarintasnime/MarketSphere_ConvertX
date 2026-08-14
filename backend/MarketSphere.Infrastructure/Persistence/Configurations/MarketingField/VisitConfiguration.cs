using MarketSphere.Domain.Entities.MarketingField;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.MarketingField;

public sealed class VisitConfiguration : IEntityTypeConfiguration<Visit>
{
    public void Configure(EntityTypeBuilder<Visit> b)
    {
        b.ToTable("Visits", t =>
        {
            t.HasCheckConstraint("CK_Visits_Checkout", "[CheckOutAt] IS NULL OR [CheckOutAt] >= [CheckInAt]");
            t.HasCheckConstraint("CK_Visits_CheckInLatitude", "[CheckInGPSLat] >= -90 AND [CheckInGPSLat] <= 90");
            t.HasCheckConstraint("CK_Visits_CheckInLongitude", "[CheckInGPSLng] >= -180 AND [CheckInGPSLng] <= 180");
            t.HasCheckConstraint("CK_Visits_CheckOutLatitude", "[CheckOutGPSLat] IS NULL OR ([CheckOutGPSLat] >= -90 AND [CheckOutGPSLat] <= 90)");
            t.HasCheckConstraint("CK_Visits_CheckOutLongitude", "[CheckOutGPSLng] IS NULL OR ([CheckOutGPSLng] >= -180 AND [CheckOutGPSLng] <= 180)");
            t.HasCheckConstraint("CK_Visits_Accuracy", "[AccuracyMeters] IS NULL OR [AccuracyMeters] > 0");
        });
        b.HasKey(x => x.VisitID);
        b.Property(x => x.CheckInGPSLat).HasPrecision(10, 7);
        b.Property(x => x.CheckInGPSLng).HasPrecision(10, 7);
        b.Property(x => x.CheckOutGPSLat).HasPrecision(10, 7);
        b.Property(x => x.CheckOutGPSLng).HasPrecision(10, 7);
        b.Property(x => x.AccuracyMeters).HasPrecision(10, 2);
        b.Property(x => x.Note).HasMaxLength(2000);
        b.HasIndex(x => new { x.EmployeeID, x.CheckInAt });
        b.HasIndex(x => new { x.ClientID, x.CheckInAt });
        b.HasIndex(x => new { x.Status, x.CheckInAt });
        b.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeID).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientID).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Route).WithMany().HasForeignKey(x => x.RouteID).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Campaign).WithMany(x => x.Visits).HasForeignKey(x => x.CampaignID).OnDelete(DeleteBehavior.Restrict);
    }
}
