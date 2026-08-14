using MarketSphere.Domain.Entities.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.CRM;

public sealed class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
{
    public void Configure(EntityTypeBuilder<Complaint> builder)
    {
        builder.ToTable("Complaints", t => t.HasCheckConstraint("CK_Complaints_Satisfaction", "[SatisfactionScore] IS NULL OR ([SatisfactionScore] BETWEEN 1 AND 5)"));
        builder.HasKey(x => x.ComplaintID);
        builder.Property(x => x.ComplaintNo).HasMaxLength(40).IsRequired();
        builder.Property(x => x.ComplaintCategory).HasConversion<int>();
        builder.Property(x => x.Priority).HasConversion<int>();
        builder.Property(x => x.Subject).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Details).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.ResolutionNote).HasMaxLength(1000);
        builder.HasIndex(x => x.ComplaintNo).IsUnique();
        builder.HasIndex(x => new { x.Status, x.SLADueAt });
        builder.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Invoice).WithMany().HasForeignKey(x => x.InvoiceID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Delivery).WithMany().HasForeignKey(x => x.DeliveryID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AssignedEmployee).WithMany().HasForeignKey(x => x.AssignedEmployeeID).OnDelete(DeleteBehavior.Restrict);
    }
}
