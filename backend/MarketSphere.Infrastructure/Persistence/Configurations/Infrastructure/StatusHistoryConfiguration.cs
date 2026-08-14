using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketSphere.Domain.Entities.Infrastructure;
namespace MarketSphere.Infrastructure.Persistence.Configurations.Infrastructure;
public sealed class StatusHistoryConfiguration : IEntityTypeConfiguration<StatusHistory> { public void Configure(EntityTypeBuilder<StatusHistory> builder) { builder.ToTable("StatusHistories"); builder.HasKey(x => x.StatusHistoryID); builder.Property(x => x.EntityType).HasMaxLength(100).IsRequired(); builder.Property(x => x.OldStatus).HasMaxLength(100); builder.Property(x => x.NewStatus).HasMaxLength(100).IsRequired(); builder.Property(x => x.Reason).HasMaxLength(1000); builder.HasIndex(x => new { x.EntityType, x.EntityID, x.ChangedAt }); builder.HasOne(x => x.ChangedByUser).WithMany().HasForeignKey(x => x.ChangedByUserID).OnDelete(DeleteBehavior.Restrict); } }
