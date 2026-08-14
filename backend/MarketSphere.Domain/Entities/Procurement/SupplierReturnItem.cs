using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.Inventory;
using MarketSphere.Domain.Entities.ProductPricing;

namespace MarketSphere.Domain.Entities.Procurement;

public sealed class SupplierReturnItem : AuditableEntity
{
    public int SupplierReturnItemID { get; set; }
    public int SupplierReturnID { get; set; }
    public int SKUID { get; set; }
    public int? BatchID { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int? StockMovementID { get; set; }

    public SupplierReturn SupplierReturn { get; set; } = null!;
    public SKU SKU { get; set; } = null!;
    public Batch? Batch { get; set; }
    public StockMovement? StockMovement { get; set; }
}
