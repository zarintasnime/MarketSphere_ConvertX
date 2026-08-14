using MarketSphere.Domain.Entities.OrganizationSecurity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrganizationSecurity;

public sealed class DesignationConfiguration :
    IEntityTypeConfiguration<Designation>
{
    public void Configure(
        EntityTypeBuilder<Designation> builder)
    {
        builder.ToTable(
            "Designations",
            table => table.HasCheckConstraint(
                "CK_Designations_HierarchyLevel",
                "[HierarchyLevel] >= 0"));

        builder.HasKey(x => x.DesignationID);

        builder.Property(x => x.DesignationCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.DesignationName)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(x => x.DesignationCode)
            .IsUnique();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
