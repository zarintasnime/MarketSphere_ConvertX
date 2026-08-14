using MarketSphere.Domain.Entities.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace MarketSphere.Infrastructure.Persistence.Configurations.CRM;
public sealed class DuplicateReviewCaseConfiguration : IEntityTypeConfiguration<DuplicateReviewCase> { public void Configure(EntityTypeBuilder<DuplicateReviewCase> b) { b.ToTable("DuplicateReviewCases", t => t.HasCheckConstraint("CK_DuplicateReviewCases_NotSelf", "NOT ([SourceEntityType] = [MatchedEntityType] AND [SourceEntityID] = [MatchedEntityID])")); b.HasKey(x => x.DuplicateReviewCaseID); b.Property(x => x.SourceEntityType).HasMaxLength(50).IsRequired(); b.Property(x => x.MatchedEntityType).HasMaxLength(50).IsRequired(); b.Property(x => x.MatchScore).HasPrecision(5, 2); b.Property(x => x.MatchReasonsJson).HasColumnType("nvarchar(max)"); b.HasIndex(x => new { x.SourceEntityType, x.SourceEntityID, x.MatchedEntityType, x.MatchedEntityID, x.Status }); } }
