using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.ProductPricing;

namespace MarketSphere.Domain.Entities.OrderFulfilment;

public sealed class InvoiceItem : AuditableEntity
{
    public int InvoiceItemID { get; set; }
    public int InvoiceID { get; set; }
    public int OrderItemID { get; set; }
    public int SKUID { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }

    public Invoice Invoice { get; set; } = null!;
    public OrderItem OrderItem { get; set; } = null!;
    public SKU SKU { get; set; } = null!;
}
