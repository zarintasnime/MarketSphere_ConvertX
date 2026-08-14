using MarketSphere.Domain.Common;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Procurement;

public sealed class PurchaseInvoice : AuditableEntity
{
    public int PurchaseInvoiceID { get; set; }
    public string PurchaseInvoiceNo { get; set; } = string.Empty;
    public int SupplierID { get; set; }
    public int? PurchaseOrderID { get; set; }
    public int? GoodsReceiptID { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public SupplierInvoicePaymentStatus PaymentStatus { get; set; } = SupplierInvoicePaymentStatus.Unpaid;
    public PurchaseInvoiceStatus Status { get; set; } = PurchaseInvoiceStatus.Draft;

    public Supplier Supplier { get; set; } = null!;
    public PurchaseOrder? PurchaseOrder { get; set; }
    public GoodsReceipt? GoodsReceipt { get; set; }
    public ICollection<SupplierPayment> Payments { get; set; } = new List<SupplierPayment>();
}
