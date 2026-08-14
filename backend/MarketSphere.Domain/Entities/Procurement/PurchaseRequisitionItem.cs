using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.ProductPricing;

namespace MarketSphere.Domain.Entities.Procurement;

public sealed class PurchaseRequisitionItem : AuditableEntity
{
    public int PurchaseRequisitionItemID { get; set; }
    public int PurchaseRequisitionID { get; set; }
    public int SKUID { get; set; }
    public decimal RequestedQuantity { get; set; }
    public decimal? EstimatedUnitCost { get; set; }
    public string? Note { get; set; }

    public PurchaseRequisition PurchaseRequisition { get; set; } = null!;
    public SKU SKU { get; set; } = null!;
}
