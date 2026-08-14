using MarketSphere.Domain.Entities.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace MarketSphere.Infrastructure.Persistence.Configurations.CRM;
public sealed class ClientSegmentConfiguration : IEntityTypeConfiguration<ClientSegment> { public void Configure(EntityTypeBuilder<ClientSegment> b) { b.ToTable("ClientSegments"); b.HasKey(x => x.ClientSegmentID); b.Property(x => x.SegmentCode).HasMaxLength(30).IsRequired(); b.Property(x => x.SegmentName).HasMaxLength(100).IsRequired(); b.Property(x => x.Description).HasMaxLength(500); b.HasIndex(x => x.SegmentCode).IsUnique(); b.HasQueryFilter(x => !x.IsDeleted); } }
