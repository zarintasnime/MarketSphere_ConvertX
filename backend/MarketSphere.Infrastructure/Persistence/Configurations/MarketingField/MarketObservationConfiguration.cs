using MarketSphere.Domain.Entities.MarketingField;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.MarketingField;

public sealed class MarketObservationConfiguration : IEntityTypeConfiguration<MarketObservation>
{
    public void Configure(EntityTypeBuilder<MarketObservation> builder)
    {
        builder.ToTable("MarketObservations", table =>
        {
            table.HasCheckConstraint("CK_MarketObservations_Facing", "[FacingCount] IS NULL OR [FacingCount] >= 0");
            table.HasCheckConstraint("CK_MarketObservations_Planogram", "[PlanogramScore] IS NULL OR ([PlanogramScore] >= 0 AND [PlanogramScore] <= 100)");
            table.HasCheckConstraint("CK_MarketObservations_Display", "[DisplayScore] IS NULL OR ([DisplayScore] >= 0 AND [DisplayScore] <= 100)");
            table.HasCheckConstraint("CK_MarketObservations_CompetitorPrice", "[CompetitorPrice] IS NULL OR [CompetitorPrice] >= 0");
        });
        builder.HasKey(x => x.MarketObservationID);
        builder.Property(x => x.PlanogramScore).HasPrecision(5, 2);
        builder.Property(x => x.DisplayScore).HasPrecision(5, 2);
        builder.Property(x => x.CompetitorPrice).HasPrecision(18, 2);
        builder.Property(x => x.CompetitorBrand).HasMaxLength(150);
        builder.Property(x => x.CompetitorProduct).HasMaxLength(200);
        builder.Property(x => x.CompetitorOffer).HasMaxLength(500);
        builder.Property(x => x.Note).HasMaxLength(2000);
        builder.HasIndex(x => new { x.ClientID, x.ObservationType });
        builder.HasIndex(x => new { x.EmployeeID, x.CreatedAt });
        builder.HasOne(x => x.Visit).WithMany(x => x.MarketObservations).HasForeignKey(x => x.VisitID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SKU).WithMany().HasForeignKey(x => x.SKUID).OnDelete(DeleteBehavior.Restrict);
    }
}
