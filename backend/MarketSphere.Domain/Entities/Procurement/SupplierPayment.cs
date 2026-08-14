using MarketSphere.Domain.Common;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Procurement;

public sealed class SupplierPayment : AuditableEntity
{
    public int SupplierPaymentID { get; set; }
    public int SupplierID { get; set; }
    public int PurchaseInvoiceID { get; set; }
    public string PaymentNo { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public decimal Amount { get; set; }
    public string? ReferenceNo { get; set; }
    public SupplierPaymentStatus Status { get; set; } = SupplierPaymentStatus.Pending;

    public Supplier Supplier { get; set; } = null!;
    public PurchaseInvoice PurchaseInvoice { get; set; } = null!;
}
