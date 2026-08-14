using MarketSphere.Domain.Common;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrganizationSecurity;

public class User : AuditableEntity, IHasStatus<UserStatus>
{
    public int UserID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public UserStatus Status { get; set; } = UserStatus.Invited;
    public bool MustChangePassword { get; set; } = true;
    public DateTime? AccountActivatedAt { get; set; }
    public int FailedLoginCount { get; set; }
    public DateTime? LockoutEndAt { get; set; }

    public Employee? Employee { get; set; }
    public ICollection<UserAccountToken> AccountTokens { get; set; } = new List<UserAccountToken>();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
}
