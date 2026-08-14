using MarketSphere.Domain.Entities.OrganizationSecurity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrganizationSecurity;

public sealed class EmployeeConfiguration :
    IEntityTypeConfiguration<Employee>
{
    public void Configure(
        EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable(
            "Employees",
            table => table.HasCheckConstraint(
                "CK_Employees_EmploymentDates",
                "[EndDate] IS NULL OR [EndDate] >= [JoiningDate]"));

        builder.HasKey(x => x.EmployeeID);

        builder.Property(x => x.EmployeeCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Phone)
            .HasMaxLength(30);

        builder.Property(x => x.Email)
            .HasMaxLength(256);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => x.EmployeeCode)
            .IsUnique();

        builder.HasIndex(x => x.UserID)
            .IsUnique()
            .HasFilter("[UserID] IS NOT NULL");

        builder.HasOne(x => x.User)
            .WithOne(x => x.Employee)
            .HasForeignKey<Employee>(x => x.UserID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Designation)
            .WithMany(x => x.Employees)
            .HasForeignKey(x => x.DesignationID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Manager)
            .WithMany(x => x.DirectReports)
            .HasForeignKey(x => x.ManagerEmployeeID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Branch)
            .WithMany(x => x.Employees)
            .HasForeignKey(x => x.BranchID)
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

        builder.HasIndex(x => new
        {
            x.BranchID,
            x.Status
        });
    }
}
