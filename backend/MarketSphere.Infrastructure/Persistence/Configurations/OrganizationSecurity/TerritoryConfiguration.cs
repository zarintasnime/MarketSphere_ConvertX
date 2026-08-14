using MarketSphere.Domain.Entities.OrganizationSecurity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrganizationSecurity;

public sealed class TerritoryConfiguration :
    IEntityTypeConfiguration<Territory>
{
    public void Configure(
        EntityTypeBuilder<Territory> builder)
    {
        builder.ToTable("Territories");
        builder.HasKey(x => x.TerritoryID);

        builder.Property(x => x.TerritoryCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.TerritoryName)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.AreaID,
            x.TerritoryCode
        })
            .IsUnique();

        builder.HasOne(x => x.Area)
            .WithMany(x => x.Territories)
            .HasForeignKey(x => x.AreaID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
