using MarketSphere.Domain.Entities.OrganizationSecurity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrganizationSecurity;

public sealed class RolePermissionConfiguration :
    IEntityTypeConfiguration<RolePermission>
{
    public void Configure(
        EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");
        builder.HasKey(x => x.RolePermissionID);

        builder.HasIndex(x => new
        {
            x.RoleID,
            x.PermissionID
        })
            .IsUnique();

        builder.HasOne(x => x.Role)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.RoleID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Permission)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.PermissionID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
