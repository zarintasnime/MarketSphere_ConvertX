using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketSphere.Domain.Entities.Infrastructure;
namespace MarketSphere.Infrastructure.Persistence.Configurations.Infrastructure;
public sealed class IdempotencyRequestConfiguration : IEntityTypeConfiguration<IdempotencyRequest> { public void Configure(EntityTypeBuilder<IdempotencyRequest> builder) { builder.ToTable("IdempotencyRequests"); builder.HasKey(x => x.IdempotencyRequestID); builder.Property(x => x.IdempotencyKey).HasMaxLength(200).IsRequired(); builder.Property(x => x.Endpoint).HasMaxLength(500).IsRequired(); builder.Property(x => x.RequestHash).HasMaxLength(128).IsRequired(); builder.Property(x => x.ResponseBody).HasColumnType("nvarchar(max)"); builder.HasIndex(x => x.IdempotencyKey).IsUnique(); builder.HasIndex(x => x.ExpiresAt); builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserID).OnDelete(DeleteBehavior.Restrict); } }
