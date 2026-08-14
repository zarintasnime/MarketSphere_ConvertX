using MarketSphere.Domain.Entities.OrganizationSecurity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrganizationSecurity;

public sealed class UserConfiguration :
    IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(
            "Users",
            table => table.HasCheckConstraint(
                "CK_Users_FailedLoginCount",
                "[FailedLoginCount] >= 0"));

        builder.HasKey(x => x.UserID);

        builder.Property(x => x.FullName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Phone)
            .HasMaxLength(30);

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.HasIndex(x => x.Status);
    }
}
