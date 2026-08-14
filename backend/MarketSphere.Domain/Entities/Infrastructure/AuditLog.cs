using MarketSphere.Domain.Entities.OrganizationSecurity;

namespace MarketSphere.Domain.Entities.Infrastructure;

public sealed class AuditLog
{
    public int AuditLogID { get; set; }
    public int? UserID { get; set; }
    public string ActionName { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int? EntityID { get; set; }
    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }
    public string? IPAddress { get; set; }
    public string? DeviceIdentifier { get; set; }
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
}
