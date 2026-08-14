using MarketSphere.Domain.Entities.OrganizationSecurity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrganizationSecurity;

public sealed class BranchConfiguration :
    IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");
        builder.HasKey(x => x.BranchID);

        builder.Property(x => x.BranchCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.BranchName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.BranchType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Address)
            .HasMaxLength(500);

        builder.Property(x => x.Phone)
            .HasMaxLength(30);

        builder.HasIndex(x => new
        {
            x.CompanyID,
            x.BranchCode
        })
            .IsUnique();

        builder.HasOne(x => x.Company)
            .WithMany(x => x.Branches)
            .HasForeignKey(x => x.CompanyID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
