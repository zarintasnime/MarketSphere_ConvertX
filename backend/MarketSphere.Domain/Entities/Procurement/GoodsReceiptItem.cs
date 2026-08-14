using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.Inventory;
using MarketSphere.Domain.Entities.ProductPricing;

namespace MarketSphere.Domain.Entities.Procurement;

public sealed class GoodsReceiptItem : AuditableEntity
{
    public int GoodsReceiptItemID { get; set; }
    public int GoodsReceiptID { get; set; }
    public int PurchaseOrderItemID { get; set; }
    public int SKUID { get; set; }
    public decimal AcceptedQuantity { get; set; }
    public decimal RejectedQuantity { get; set; }
    public string? BatchNo { get; set; }
    public DateTime? ManufacturingDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal UnitCost { get; set; }
    public int? BatchID { get; set; }
    public string? RejectionReason { get; set; }

    public GoodsReceipt GoodsReceipt { get; set; } = null!;
    public PurchaseOrderItem PurchaseOrderItem { get; set; } = null!;
    public SKU SKU { get; set; } = null!;
    public Batch? Batch { get; set; }
}
