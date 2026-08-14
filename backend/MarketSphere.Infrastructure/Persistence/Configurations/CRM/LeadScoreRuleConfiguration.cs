using MarketSphere.Domain.Entities.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace MarketSphere.Infrastructure.Persistence.Configurations.CRM;
public sealed class LeadScoreRuleConfiguration : IEntityTypeConfiguration<LeadScoreRule> { public void Configure(EntityTypeBuilder<LeadScoreRule> b) { b.ToTable("LeadScoreRules", t => t.HasCheckConstraint("CK_LeadScoreRules_DateRange", "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]")); b.HasKey(x => x.LeadScoreRuleID); b.Property(x => x.RuleName).HasMaxLength(150).IsRequired(); b.Property(x => x.ComparisonValue).HasMaxLength(500); b.HasIndex(x => new { x.IsActive, x.EffectiveFrom, x.EffectiveTo }); b.HasQueryFilter(x => !x.IsDeleted); } }
