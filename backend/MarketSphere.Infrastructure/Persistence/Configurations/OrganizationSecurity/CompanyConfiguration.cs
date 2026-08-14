using MarketSphere.Domain.Entities.OrganizationSecurity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrganizationSecurity;

public sealed class CompanyConfiguration :
    IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");
        builder.HasKey(x => x.CompanyID);

        builder.Property(x => x.CompanyCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CompanyName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.TradeLicenseNo)
            .HasMaxLength(100);

        builder.Property(x => x.Phone)
            .HasMaxLength(30);

        builder.Property(x => x.Email)
            .HasMaxLength(256);

        builder.Property(x => x.Address)
            .HasMaxLength(500);

        builder.HasIndex(x => x.CompanyCode)
            .IsUnique();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
