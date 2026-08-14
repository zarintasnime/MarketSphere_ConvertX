using MarketSphere.Domain.Common;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.CRM;

public sealed class DuplicateReviewCase : AuditableEntity
{
    public int DuplicateReviewCaseID { get; set; }
    public string SourceEntityType { get; set; } = string.Empty;
    public int SourceEntityID { get; set; }
    public string MatchedEntityType { get; set; } = string.Empty;
    public int MatchedEntityID { get; set; }
    public decimal? MatchScore { get; set; }
    public string? MatchReasonsJson { get; set; }
    public DuplicateReviewStatus Status { get; set; } = DuplicateReviewStatus.Open;
    public DuplicateResolutionType? ResolutionType { get; set; }
    public int? SurvivorEntityID { get; set; }
    public int? ResolvedByUserID { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
