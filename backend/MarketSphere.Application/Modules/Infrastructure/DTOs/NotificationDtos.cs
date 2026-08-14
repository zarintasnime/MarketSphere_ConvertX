using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.Infrastructure.DTOs;

public sealed record NotificationDto(int NotificationID, NotificationType NotificationType, string Title, string Message, NotificationPriority Priority, string? ReferenceType, int? ReferenceID, bool IsRead, DateTime CreatedAt, DateTime? ExpiresAt, DateTime? ReadAt);
public sealed class CreateNotificationRequestDto { public int UserID { get; init; } public NotificationType NotificationType { get; init; } public string Title { get; init; } = string.Empty; public string Message { get; init; } = string.Empty; public NotificationPriority Priority { get; init; } = NotificationPriority.Normal; public string? ReferenceType { get; init; } public int? ReferenceID { get; init; } public DateTime? ExpiresAt { get; init; } }
