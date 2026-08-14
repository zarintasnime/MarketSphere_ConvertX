using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketSphere.Domain.Entities.Infrastructure;
namespace MarketSphere.Infrastructure.Persistence.Configurations.Infrastructure;
public sealed class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting> { public void Configure(EntityTypeBuilder<SystemSetting> builder) { builder.ToTable("SystemSettings"); builder.HasKey(x => x.SystemSettingID); builder.Property(x => x.SettingKey).HasMaxLength(200).IsRequired(); builder.Property(x => x.SettingValue).HasMaxLength(4000).IsRequired(); builder.Property(x => x.DataType).HasConversion<int>(); builder.Property(x => x.ScopeType).HasConversion<int>(); builder.Property(x => x.Description).HasMaxLength(1000); builder.HasIndex(x => new { x.SettingKey, x.ScopeType, x.ScopeID }).IsUnique(); builder.HasOne(x => x.UpdatedByUser).WithMany().HasForeignKey(x => x.UpdatedByUserID).OnDelete(DeleteBehavior.Restrict); } }
