using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.Inventory;
using MarketSphere.Domain.Entities.ProductPricing;

namespace MarketSphere.Domain.Entities.OrderFulfilment;

public sealed class OrderItem : AuditableEntity
{
    public int OrderItemID { get; set; }
    public int OrderID { get; set; }
    public int SKUID { get; set; }
    public decimal OrderedQuantity { get; set; }
    public decimal FreeQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public decimal ApprovedQuantity { get; set; }
    public decimal DeliveredQuantity { get; set; }
    public decimal ReturnedQuantity { get; set; }
    public decimal BackorderQuantity { get; set; }

    public Order Order { get; set; } = null!;
    public SKU SKU { get; set; } = null!;
    public ICollection<AppliedOffer> AppliedOffers { get; set; } = new List<AppliedOffer>();
    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    public ICollection<PickListItem> PickListItems { get; set; } = new List<PickListItem>();
    public ICollection<DeliveryItem> DeliveryItems { get; set; } = new List<DeliveryItem>();
    public ICollection<StockReservation> StockReservations { get; set; } = new List<StockReservation>();
}
