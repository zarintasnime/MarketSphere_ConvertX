using MarketSphere.Domain.Entities.OrganizationSecurity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrganizationSecurity;

public sealed class RoleConfiguration :
    IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable(
            "Roles",
            table => table.HasCheckConstraint(
                "CK_Roles_RoleLevel",
                "[RoleLevel] >= 0"));

        builder.HasKey(x => x.RoleID);

        builder.Property(x => x.RoleName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.RoleCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasIndex(x => x.RoleCode)
            .IsUnique();
    }
}
