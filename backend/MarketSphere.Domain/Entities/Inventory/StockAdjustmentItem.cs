using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.ProductPricing;

namespace MarketSphere.Domain.Entities.Inventory;

public sealed class StockAdjustmentItem : AuditableEntity
{
    public int StockAdjustmentItemID { get; set; }
    public int StockAdjustmentID { get; set; }
    public int SKUID { get; set; }
    public int? BatchID { get; set; }
    public decimal AdjustmentQuantity { get; set; }
    public decimal? UnitCost { get; set; }
    public string? Note { get; set; }
    public int? StockMovementID { get; set; }

    public StockAdjustment StockAdjustment { get; set; } = null!;
    public SKU SKU { get; set; } = null!;
    public Batch? Batch { get; set; }
    public StockMovement? StockMovement { get; set; }
}
