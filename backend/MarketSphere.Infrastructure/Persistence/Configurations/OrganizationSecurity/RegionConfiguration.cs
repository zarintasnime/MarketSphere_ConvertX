using MarketSphere.Domain.Entities.OrganizationSecurity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrganizationSecurity;

public sealed class RegionConfiguration :
    IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.ToTable("Regions");
        builder.HasKey(x => x.RegionID);

        builder.Property(x => x.RegionCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.RegionName)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.CompanyID,
            x.RegionCode
        })
            .IsUnique();

        builder.HasOne(x => x.Company)
            .WithMany(x => x.Regions)
            .HasForeignKey(x => x.CompanyID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
