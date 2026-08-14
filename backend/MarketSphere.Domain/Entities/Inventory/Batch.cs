using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.ProductPricing;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Inventory;

public sealed class Batch : AuditableEntity
{
    public int BatchID { get; set; }
    public int SKUID { get; set; }
    public string BatchNo { get; set; } = string.Empty;
    public DateTime? ManufacturingDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? BestBeforeDate { get; set; }
    public decimal CostPrice { get; set; }
    public BatchStatus Status { get; set; } = BatchStatus.Available;

    public SKU SKU { get; set; } = null!;
}
