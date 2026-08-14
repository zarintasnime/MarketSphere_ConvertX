using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Infrastructure;

public sealed class SystemSetting : AuditableEntity
{
    public int SystemSettingID { get; set; }

    public string SettingKey { get; set; } = string.Empty;

    public string SettingValue { get; set; } = string.Empty;

    public SettingDataType DataType { get; set; }

    public SettingScopeType ScopeType { get; set; }
        = SettingScopeType.Global;

    public int? ScopeID { get; set; }

    public string? Description { get; set; }

    public bool IsEncrypted { get; set; }

    public User? UpdatedByUser { get; set; }
}
