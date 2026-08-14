using MarketSphere.Domain.Entities.OrganizationSecurity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrganizationSecurity;

public sealed class UserSessionConfiguration :
    IEntityTypeConfiguration<UserSession>
{
    public void Configure(
        EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable(
            "UserSessions",
            table => table.HasCheckConstraint(
                "CK_UserSessions_Expiry",
                "[ExpiresAt] > [IssuedAt]"));

        builder.HasKey(x => x.UserSessionID);

        builder.Property(x => x.DeviceIdentifier)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.DeviceName)
            .HasMaxLength(200);

        builder.Property(x => x.RefreshTokenHash)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(x => x.RefreshTokenHash)
            .IsUnique();

        builder.HasIndex(x => new
        {
            x.UserID,
            x.RevokedAt,
            x.ExpiresAt
        });

        builder.HasOne(x => x.User)
            .WithMany(x => x.Sessions)
            .HasForeignKey(x => x.UserID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
