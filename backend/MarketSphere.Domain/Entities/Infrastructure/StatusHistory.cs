using MarketSphere.Domain.Entities.OrganizationSecurity;

namespace MarketSphere.Domain.Entities.Infrastructure;

public sealed class StatusHistory
{
    public int StatusHistoryID { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int EntityID { get; set; }
    public string? OldStatus { get; set; }
    public string NewStatus { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public int? ChangedByUserID { get; set; }
    public DateTime ChangedAt { get; set; }

    public User? ChangedByUser { get; set; }
}
