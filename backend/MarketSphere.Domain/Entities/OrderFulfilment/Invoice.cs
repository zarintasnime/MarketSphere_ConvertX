using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrderFulfilment;

public sealed class Invoice : AuditableEntity
{
    public int InvoiceID { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public int OrderID { get; set; }
    public int ClientID { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    public Order Order { get; set; } = null!;
    public Client Client { get; set; } = null!;
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    public ICollection<PaymentAllocation> PaymentAllocations { get; set; } = new List<PaymentAllocation>();
    public ICollection<CreditNote> CreditNotes { get; set; } = new List<CreditNote>();
}
