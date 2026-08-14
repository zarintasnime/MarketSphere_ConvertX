using MarketSphere.Domain.Common;

namespace MarketSphere.Domain.Entities.OrganizationSecurity;

public class Permission : AuditableEntity
{
    public int PermissionID { get; set; }
    public string ModuleName { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public string PermissionCode { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
