using MarketSphere.Domain.Entities.OrganizationSecurity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrganizationSecurity;

public sealed class PermissionConfiguration :
    IEntityTypeConfiguration<Permission>
{
    public void Configure(
        EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");
        builder.HasKey(x => x.PermissionID);

        builder.Property(x => x.ModuleName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ActionName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.PermissionCode)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasIndex(x => x.PermissionCode)
            .IsUnique();

        builder.HasIndex(x => new
        {
            x.ModuleName,
            x.ActionName
        })
            .IsUnique();
    }
}
