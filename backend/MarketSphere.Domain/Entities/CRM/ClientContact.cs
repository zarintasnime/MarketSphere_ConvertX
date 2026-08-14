using MarketSphere.Domain.Common;

namespace MarketSphere.Domain.Entities.CRM;

public sealed class ClientContact : AuditableEntity
{
    public int ClientContactID { get; set; }
    public int ClientID { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string? Designation { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; } = true;
    public Client Client { get; set; } = null!;
}
