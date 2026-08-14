using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.ProductPricing;

namespace MarketSphere.Domain.Entities.Inventory;

public sealed class StockBalance : AuditableEntity, IHasRowVersion
{
    public int StockBalanceID { get; set; }
    public int WarehouseID { get; set; }
    public int SKUID { get; set; }
    public int? BatchID { get; set; }
    public decimal OnHandQuantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal QuarantineQuantity { get; set; }
    public decimal DamagedQuantity { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Warehouse Warehouse { get; set; } = null!;
    public SKU SKU { get; set; } = null!;
    public Batch? Batch { get; set; }
}
