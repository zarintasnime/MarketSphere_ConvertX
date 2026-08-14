using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Procurement;

public sealed class PurchaseOrder : AuditableEntity
{
    public int PurchaseOrderID { get; set; }
    public string PurchaseOrderNo { get; set; } = string.Empty;
    public int SupplierID { get; set; }
    public int? PurchaseRequisitionID { get; set; }
    public int BranchID { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }

    public Supplier Supplier { get; set; } = null!;
    public PurchaseRequisition? PurchaseRequisition { get; set; }
    public Branch Branch { get; set; } = null!;
    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
    public ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();
}
