using MarketSphere.Domain.Entities.OrganizationSecurity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrganizationSecurity;

public sealed class EmployeeTerritoryAssignmentConfiguration :
    IEntityTypeConfiguration<EmployeeTerritoryAssignment>
{
    public void Configure(
        EntityTypeBuilder<EmployeeTerritoryAssignment> builder)
    {
        builder.ToTable(
            "EmployeeTerritoryAssignments",
            table =>
            {
                table.HasCheckConstraint(
                    "CK_EmployeeTerritoryAssignments_EffectiveDates",
                    "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]");

                table.HasCheckConstraint(
                    "CK_EmployeeTerritoryAssignments_Scope",
                    "([ScopeType] = 1 AND [RegionID] IS NOT NULL AND [AreaID] IS NULL AND [TerritoryID] IS NULL) OR " +
                    "([ScopeType] = 2 AND [RegionID] IS NULL AND [AreaID] IS NOT NULL AND [TerritoryID] IS NULL) OR " +
                    "([ScopeType] = 3 AND [RegionID] IS NULL AND [AreaID] IS NULL AND [TerritoryID] IS NOT NULL)");
            });

        builder.HasKey(
            x => x.EmployeeTerritoryAssignmentID);

        builder.Property(x => x.ScopeType)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.EmployeeID,
            x.ScopeType,
            x.RegionID,
            x.AreaID,
            x.TerritoryID,
            x.EffectiveFrom
        });

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.TerritoryAssignments)
            .HasForeignKey(x => x.EmployeeID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Region)
            .WithMany()
            .HasForeignKey(x => x.RegionID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Area)
            .WithMany()
            .HasForeignKey(x => x.AreaID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Territory)
            .WithMany()
            .HasForeignKey(x => x.TerritoryID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
