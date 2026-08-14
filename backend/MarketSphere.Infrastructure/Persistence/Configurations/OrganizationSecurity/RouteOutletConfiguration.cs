using MarketSphere.Domain.Entities.OrganizationSecurity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace MarketSphere.Infrastructure.Persistence.Configurations.OrganizationSecurity;
public sealed class RouteOutletConfiguration : IEntityTypeConfiguration<RouteOutlet>
{
    public void Configure(EntityTypeBuilder<RouteOutlet> b) { b.ToTable("RouteOutlets", t => { t.HasCheckConstraint("CK_RouteOutlets_SequenceNo", "[SequenceNo] > 0"); t.HasCheckConstraint("CK_RouteOutlets_VisitFrequency", "[VisitFrequency] > 0"); t.HasCheckConstraint("CK_RouteOutlets_DateRange", "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]"); }); b.HasKey(x => x.RouteOutletID); b.HasIndex(x => new { x.RouteID, x.SequenceNo, x.EffectiveFrom }); b.HasIndex(x => new { x.RouteID, x.ClientID }).HasFilter("[EffectiveTo] IS NULL").IsUnique(); b.HasOne(x => x.Route).WithMany(x => x.RouteOutlets).HasForeignKey(x => x.RouteID).OnDelete(DeleteBehavior.Restrict); b.HasOne(x => x.Client).WithMany(x => x.RouteOutlets).HasForeignKey(x => x.ClientID).OnDelete(DeleteBehavior.Restrict); }
}
