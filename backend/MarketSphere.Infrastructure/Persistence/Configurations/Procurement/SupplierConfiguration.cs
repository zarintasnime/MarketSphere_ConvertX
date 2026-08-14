using MarketSphere.Domain.Entities.Procurement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.Procurement;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers", t => t.HasCheckConstraint("CK_Suppliers_PaymentTerms", "[PaymentTermsDays] >= 0"));
        builder.HasKey(x => x.SupplierID);
        builder.Property(x => x.SupplierCode).HasMaxLength(40).IsRequired();
        builder.Property(x => x.SupplierName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ContactPerson).HasMaxLength(150);
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.Status).HasConversion<int>();
        builder.HasIndex(x => x.SupplierCode).IsUnique();
        builder.HasIndex(x => new { x.SupplierName, x.Status });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
