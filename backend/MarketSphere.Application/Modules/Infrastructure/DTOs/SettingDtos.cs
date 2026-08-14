using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.Infrastructure.DTOs;

public sealed record SystemSettingDto(int SystemSettingID, string SettingKey, string SettingValue, SettingDataType DataType, SettingScopeType ScopeType, int? ScopeID, string? Description, bool IsEncrypted, int? UpdatedByUserID, DateTime? UpdatedAt);
public sealed class SaveSystemSettingRequestDto { public string SettingKey { get; init; } = string.Empty; public string SettingValue { get; init; } = string.Empty; public SettingDataType DataType { get; init; } public SettingScopeType ScopeType { get; init; } = SettingScopeType.Global; public int? ScopeID { get; init; } public string? Description { get; init; } public bool IsEncrypted { get; init; } }
