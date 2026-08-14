using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.ProductPricing;

namespace MarketSphere.Domain.Entities.CRM;

public sealed class QuotationItem : AuditableEntity
{
    public int QuotationItemID { get; set; }
    public int QuotationID { get; set; }
    public int SKUID { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public string? Note { get; set; }

    public Quotation Quotation { get; set; } = null!;
    public SKU SKU { get; set; } = null!;
}
