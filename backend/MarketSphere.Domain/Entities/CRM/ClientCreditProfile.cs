using MarketSphere.Domain.Common;

namespace MarketSphere.Domain.Entities.CRM;

public sealed class ClientCreditProfile : AuditableEntity
{
    public int ClientCreditProfileID { get; set; }
    public int ClientID { get; set; }
    public decimal CreditLimit { get; set; }
    public int CreditDays { get; set; }
    public decimal CurrentDue { get; set; }
    public bool IsBlocked { get; set; }
    public string? BlockReason { get; set; }
    public DateTime? LastReviewedAt { get; set; }
    public Client Client { get; set; } = null!;
}
