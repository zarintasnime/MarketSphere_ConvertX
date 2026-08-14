using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrderFulfilment;

public sealed class CreditNote : AuditableEntity
{
    public int CreditNoteID { get; set; }
    public string CreditNoteNo { get; set; } = string.Empty;
    public int ClientID { get; set; }
    public int InvoiceID { get; set; }
    public int ReturnRequestID { get; set; }
    public DateTime CreditDate { get; set; }
    public decimal Amount { get; set; }
    public CreditNoteStatus Status { get; set; } = CreditNoteStatus.Draft;
    public DateTime? PostedAt { get; set; }
    public string? Reason { get; set; }

    public Client Client { get; set; } = null!;
    public Invoice Invoice { get; set; } = null!;
    public ReturnRequest ReturnRequest { get; set; } = null!;
}
