using MarketSphere.Domain.Common;

namespace MarketSphere.Domain.Entities.OrganizationSecurity;

public class Role : AuditableEntity
{
    public int RoleID { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int RoleLevel { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
