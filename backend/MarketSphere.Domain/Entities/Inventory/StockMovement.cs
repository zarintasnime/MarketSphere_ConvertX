using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Entities.ProductPricing;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Inventory;

public sealed class StockMovement : AuditableEntity
{
    public int StockMovementID { get; set; }
    public int WarehouseID { get; set; }
    public int SKUID { get; set; }
    public int? BatchID { get; set; }
    public StockMovementType MovementType { get; set; }
    public decimal QuantityIn { get; set; }
    public decimal QuantityOut { get; set; }
    public decimal BalanceAfter { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public int ReferenceID { get; set; }
    public DateTime MovementAt { get; set; }
    public int? PerformedByUserID { get; set; }
    public string? Note { get; set; }

    public Warehouse Warehouse { get; set; } = null!;
    public SKU SKU { get; set; } = null!;
    public Batch? Batch { get; set; }
    public User? PerformedByUser { get; set; }
}
