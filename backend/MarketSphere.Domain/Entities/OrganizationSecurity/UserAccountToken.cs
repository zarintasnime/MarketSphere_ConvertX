using MarketSphere.Domain.Common;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrganizationSecurity;

public class UserAccountToken : AuditableEntity
{
    public int UserAccountTokenID { get; set; }
    public int UserID { get; set; }
    public AccountTokenType TokenType { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }

    public User User { get; set; } = null!;
}
