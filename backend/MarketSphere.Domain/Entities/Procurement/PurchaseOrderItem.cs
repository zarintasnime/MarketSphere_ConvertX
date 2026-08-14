using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.ProductPricing;

namespace MarketSphere.Domain.Entities.Procurement;

public sealed class PurchaseOrderItem : AuditableEntity
{
    public int PurchaseOrderItemID { get; set; }
    public int PurchaseOrderID { get; set; }
    public int SKUID { get; set; }
    public decimal OrderedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }

    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public SKU SKU { get; set; } = null!;
    public ICollection<GoodsReceiptItem> GoodsReceiptItems { get; set; } = new List<GoodsReceiptItem>();
}
