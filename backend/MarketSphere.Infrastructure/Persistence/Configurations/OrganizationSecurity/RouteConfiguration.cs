using MarketSphere.Domain.Entities.OrganizationSecurity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrganizationSecurity;

public sealed class RouteConfiguration :
    IEntityTypeConfiguration<Route>
{
    public void Configure(EntityTypeBuilder<Route> builder)
    {
        builder.ToTable("Routes");
        builder.HasKey(x => x.RouteID);

        builder.Property(x => x.RouteCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.RouteName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.VisitFrequency)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.TerritoryID,
            x.RouteCode
        })
            .IsUnique();

        builder.HasOne(x => x.Territory)
            .WithMany(x => x.Routes)
            .HasForeignKey(x => x.TerritoryID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
