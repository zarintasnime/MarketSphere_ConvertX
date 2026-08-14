using MarketSphere.Domain.Entities.OrganizationSecurity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrganizationSecurity;

public sealed class UserAccountTokenConfiguration :
    IEntityTypeConfiguration<UserAccountToken>
{
    public void Configure(
        EntityTypeBuilder<UserAccountToken> builder)
    {
        builder.ToTable(
            "UserAccountTokens",
            table => table.HasCheckConstraint(
                "CK_UserAccountTokens_Expiry",
                "[ExpiresAt] > [CreatedAt]"));

        builder.HasKey(x => x.UserAccountTokenID);

        builder.Property(x => x.TokenType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.TokenHash)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.HasIndex(x => new
        {
            x.UserID,
            x.TokenType,
            x.ExpiresAt
        });

        builder.HasOne(x => x.User)
            .WithMany(x => x.AccountTokens)
            .HasForeignKey(x => x.UserID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
