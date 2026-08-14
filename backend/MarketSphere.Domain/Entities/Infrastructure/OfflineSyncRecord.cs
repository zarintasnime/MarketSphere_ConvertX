using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Infrastructure;

public sealed class OfflineSyncRecord
{
    public int OfflineSyncRecordID { get; set; }
    public int UserSessionID { get; set; }
    public string LocalRecordKey { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public OfflineOperationType OperationType { get; set; }
    public string PayloadJson { get; set; } = string.Empty;
    public DateTime ClientTimestamp { get; set; }
    public DateTime? ServerTimestamp { get; set; }
    public OfflineSyncStatus SyncStatus { get; set; } = OfflineSyncStatus.Pending;
    public int RetryCount { get; set; }
    public string? LastError { get; set; }
    public int? ServerEntityID { get; set; }

    public UserSession UserSession { get; set; } = null!;
}
