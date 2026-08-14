using MarketSphere.Domain.Entities.OrganizationSecurity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrganizationSecurity;

public sealed class UserRoleConfiguration :
    IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");
        builder.HasKey(x => x.UserRoleID);

        builder.HasIndex(x => new
        {
            x.UserID,
            x.RoleID
        })
            .IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Role)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.RoleID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AssignedByUser)
            .WithMany()
            .HasForeignKey(x => x.AssignedByUserID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
