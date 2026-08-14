using MarketSphere.Domain.Common;

namespace MarketSphere.Domain.Entities.OrganizationSecurity;

public class UserRole : AuditableEntity
{
    public int UserRoleID { get; set; }
    public int UserID { get; set; }
    public int RoleID { get; set; }
    public DateTime AssignedAt { get; set; }
    public int AssignedByUserID { get; set; }

    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
    public User AssignedByUser { get; set; } = null!;
}
