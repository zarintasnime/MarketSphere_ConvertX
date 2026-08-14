using MarketSphere.Domain.Entities.OrganizationSecurity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrganizationSecurity;

public sealed class EmployeeRouteAssignmentConfiguration :
    IEntityTypeConfiguration<EmployeeRouteAssignment>
{
    public void Configure(
        EntityTypeBuilder<EmployeeRouteAssignment> builder)
    {
        builder.ToTable(
            "EmployeeRouteAssignments",
            table => table.HasCheckConstraint(
                "CK_EmployeeRouteAssignments_EffectiveDates",
                "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]"));

        builder.HasKey(
            x => x.EmployeeRouteAssignmentID);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.EmployeeID,
            x.RouteID,
            x.EffectiveFrom
        });

        builder.HasIndex(x => new
        {
            x.EmployeeID,
            x.IsPrimary,
            x.Status,
            x.EffectiveFrom
        });

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.RouteAssignments)
            .HasForeignKey(x => x.EmployeeID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Route)
            .WithMany(x => x.EmployeeAssignments)
            .HasForeignKey(x => x.RouteID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
