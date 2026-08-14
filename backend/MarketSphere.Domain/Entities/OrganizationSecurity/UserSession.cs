using MarketSphere.Domain.Common;

namespace MarketSphere.Domain.Entities.OrganizationSecurity;

public class UserSession : AuditableEntity
{
    public int UserSessionID { get; set; }
    public int UserID { get; set; }
    public string DeviceIdentifier { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
    public string RefreshTokenHash { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime? LastSeenAt { get; set; }

    public User User { get; set; } = null!;
}
