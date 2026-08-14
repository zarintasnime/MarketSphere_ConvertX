using MarketSphere.Domain.Entities.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace MarketSphere.Infrastructure.Persistence.Configurations.CRM;
public sealed class ClientContactConfiguration : IEntityTypeConfiguration<ClientContact>
{
    public void Configure(EntityTypeBuilder<ClientContact> b) { b.ToTable("ClientContacts"); b.HasKey(x => x.ClientContactID); b.Property(x => x.ContactName).HasMaxLength(150).IsRequired(); b.Property(x => x.Designation).HasMaxLength(100); b.Property(x => x.Phone).HasMaxLength(30); b.Property(x => x.Email).HasMaxLength(256); b.HasIndex(x => x.ClientID).HasFilter("[IsPrimary] = 1 AND [IsActive] = 1").IsUnique(); b.HasOne(x => x.Client).WithMany(x => x.Contacts).HasForeignKey(x => x.ClientID).OnDelete(DeleteBehavior.Restrict); }
}
