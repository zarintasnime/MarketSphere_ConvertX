using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.Inventory;
using MarketSphere.Domain.Entities.ProductPricing;

namespace MarketSphere.Domain.Entities.OrderFulfilment;

public sealed class DeliveryItem : AuditableEntity
{
    public int DeliveryItemID { get; set; }
    public int DeliveryID { get; set; }
    public int? PickListItemID { get; set; }
    public int OrderItemID { get; set; }
    public int? InvoiceItemID { get; set; }
    public int SKUID { get; set; }
    public int? BatchID { get; set; }
    public decimal QuantityDispatched { get; set; }
    public decimal QuantityDelivered { get; set; }
    public decimal QuantityRejectedAtDelivery { get; set; }

    public Delivery Delivery { get; set; } = null!;
    public PickListItem? PickListItem { get; set; }
    public OrderItem OrderItem { get; set; } = null!;
    public InvoiceItem? InvoiceItem { get; set; }
    public SKU SKU { get; set; } = null!;
    public Batch? Batch { get; set; }
    public ICollection<ReturnItem> ReturnItems { get; set; } = new List<ReturnItem>();
}
