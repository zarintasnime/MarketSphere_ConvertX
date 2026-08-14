using MarketSphere.Domain.Entities.OrganizationSecurity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrganizationSecurity;

public sealed class AreaConfiguration :
    IEntityTypeConfiguration<Area>
{
    public void Configure(EntityTypeBuilder<Area> builder)
    {
        builder.ToTable("Areas");
        builder.HasKey(x => x.AreaID);

        builder.Property(x => x.AreaCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.AreaName)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.RegionID,
            x.AreaCode
        })
            .IsUnique();

        builder.HasOne(x => x.Region)
            .WithMany(x => x.Areas)
            .HasForeignKey(x => x.RegionID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
