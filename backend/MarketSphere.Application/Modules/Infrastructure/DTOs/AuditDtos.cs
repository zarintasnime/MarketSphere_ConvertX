namespace MarketSphere.Application.Modules.Infrastructure.DTOs;

public sealed record AuditLogDto(int AuditLogID, int? UserID, string ActionName, string EntityType, int? EntityID, string? OldValuesJson, string? NewValuesJson, string? IPAddress, string? DeviceIdentifier, DateTime CreatedAt);
public sealed record StatusHistoryDto(int StatusHistoryID, string EntityType, int EntityID, string? OldStatus, string NewStatus, string? Reason, int? ChangedByUserID, DateTime ChangedAt);
public sealed class WriteAuditRequestDto { public string ActionName { get; init; } = string.Empty; public string EntityType { get; init; } = string.Empty; public int? EntityID { get; init; } public object? OldValues { get; init; } public object? NewValues { get; init; } public string? IPAddress { get; init; } public string? DeviceIdentifier { get; init; } }
public sealed class AppendStatusHistoryRequestDto { public string EntityType { get; init; } = string.Empty; public int EntityID { get; init; } public string? OldStatus { get; init; } public string NewStatus { get; init; } = string.Empty; public string? Reason { get; init; } }
