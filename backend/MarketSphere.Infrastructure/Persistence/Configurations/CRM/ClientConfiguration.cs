using MarketSphere.Domain.Entities.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.CRM;
public sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> b)
    {
        b.ToTable("Clients"); b.HasKey(x => x.ClientID);
        b.Property(x => x.ClientCode).HasMaxLength(30).IsRequired(); b.Property(x => x.ClientName).HasMaxLength(200).IsRequired();
        b.Property(x => x.Phone).HasMaxLength(30); b.Property(x => x.Email).HasMaxLength(256); b.Property(x => x.Address).HasMaxLength(500).IsRequired();
        b.Property(x => x.GPSLat).HasPrecision(10, 7); b.Property(x => x.GPSLng).HasPrecision(10, 7);
        b.HasIndex(x => x.ClientCode).IsUnique(); b.HasIndex(x => x.Phone); b.HasIndex(x => x.Email); b.HasIndex(x => new { x.RegionID, x.AreaID, x.TerritoryID });
        b.HasOne(x => x.Region).WithMany().HasForeignKey(x => x.RegionID).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Area).WithMany().HasForeignKey(x => x.AreaID).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Territory).WithMany().HasForeignKey(x => x.TerritoryID).OnDelete(DeleteBehavior.Restrict);
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}
