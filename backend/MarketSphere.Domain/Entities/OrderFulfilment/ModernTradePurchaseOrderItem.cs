using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.ProductPricing;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrderFulfilment;

public sealed class ModernTradePurchaseOrderItem : AuditableEntity
{
    public int ModernTradePurchaseOrderItemID { get; set; }
    public int ModernTradePurchaseOrderID { get; set; }
    public string? ExternalItemCode { get; set; }
    public string? ExternalItemName { get; set; }
    public int? SKUID { get; set; }
    public ItemMappingStatus MappingStatus { get; set; } = ItemMappingStatus.Unmapped;
    public decimal OrderedQuantity { get; set; }
    public decimal? AgreedUnitPrice { get; set; }
    public decimal? Discount { get; set; }
    public string? Note { get; set; }

    public ModernTradePurchaseOrder ModernTradePurchaseOrder { get; set; } = null!;
    public SKU? SKU { get; set; }
}
