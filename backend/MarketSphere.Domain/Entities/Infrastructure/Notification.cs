using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Infrastructure;

public sealed class Notification
{
    public int NotificationID { get; set; }
    public int UserID { get; set; }
    public NotificationType NotificationType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
    public string? ReferenceType { get; set; }
    public int? ReferenceID { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? ReadAt { get; set; }

    public User User { get; set; } = null!;
}
