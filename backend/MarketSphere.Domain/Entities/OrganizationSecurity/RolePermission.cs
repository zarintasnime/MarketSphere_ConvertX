using MarketSphere.Domain.Common;

namespace MarketSphere.Domain.Entities.OrganizationSecurity;

public class RolePermission : AuditableEntity
{
    public int RolePermissionID { get; set; }
    public int RoleID { get; set; }
    public int PermissionID { get; set; }
    public bool IsAllowed { get; set; } = true;

    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
