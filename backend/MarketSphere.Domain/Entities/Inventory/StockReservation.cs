using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrderFulfilment;
using MarketSphere.Domain.Entities.ProductPricing;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Inventory;

public sealed class StockReservation : AuditableEntity
{
    public int StockReservationID { get; set; }
    public int OrderItemID { get; set; }
    public int WarehouseID { get; set; }
    public int SKUID { get; set; }
    public int? BatchID { get; set; }
    public decimal ReservedQuantity { get; set; }
    public StockReservationStatus ReservationStatus { get; set; } = StockReservationStatus.Active;
    public DateTime ReservedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? ReleasedAt { get; set; }

    public OrderItem OrderItem { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public SKU SKU { get; set; } = null!;
    public Batch? Batch { get; set; }
}
