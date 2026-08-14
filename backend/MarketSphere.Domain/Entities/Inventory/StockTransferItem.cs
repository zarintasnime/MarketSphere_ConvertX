using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.ProductPricing;

namespace MarketSphere.Domain.Entities.Inventory;

public sealed class StockTransferItem : AuditableEntity
{
    public int StockTransferItemID { get; set; }
    public int StockTransferID { get; set; }
    public int SKUID { get; set; }
    public int? BatchID { get; set; }
    public decimal RequestedQuantity { get; set; }
    public decimal DispatchedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }

    public StockTransfer StockTransfer { get; set; } = null!;
    public SKU SKU { get; set; } = null!;
    public Batch? Batch { get; set; }
}
