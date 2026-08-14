using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.Inventory;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Entities.ProductPricing;

namespace MarketSphere.Domain.Entities.OrderFulfilment;

public sealed class PickListItem : AuditableEntity
{
    public int PickListItemID { get; set; }
    public int PickListID { get; set; }
    public int OrderItemID { get; set; }
    public int? StockReservationID { get; set; }
    public int SKUID { get; set; }
    public int? BatchID { get; set; }
    public decimal RequestedQuantity { get; set; }
    public decimal PickedQuantity { get; set; }
    public decimal ShortQuantity { get; set; }
    public int? PickedByEmployeeID { get; set; }
    public DateTime? PickedAt { get; set; }
    public string? VerificationNote { get; set; }

    public PickList PickList { get; set; } = null!;
    public OrderItem OrderItem { get; set; } = null!;
    public StockReservation? StockReservation { get; set; }
    public SKU SKU { get; set; } = null!;
    public Batch? Batch { get; set; }
    public Employee? PickedByEmployee { get; set; }
    public ICollection<DeliveryItem> DeliveryItems { get; set; } = new List<DeliveryItem>();
}
