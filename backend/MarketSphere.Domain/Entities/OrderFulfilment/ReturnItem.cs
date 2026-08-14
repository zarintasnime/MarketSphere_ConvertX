using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.Inventory;
using MarketSphere.Domain.Entities.ProductPricing;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrderFulfilment;

public sealed class ReturnItem : AuditableEntity
{
    public int ReturnItemID { get; set; }
    public int ReturnRequestID { get; set; }
    public int? DeliveryItemID { get; set; }
    public int SKUID { get; set; }
    public int? BatchID { get; set; }
    public decimal RequestedQuantity { get; set; }
    public decimal ApprovedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public ReturnConditionStatus? ConditionStatus { get; set; }
    public string? InspectionResult { get; set; }
    public ReturnDisposition Disposition { get; set; } = ReturnDisposition.Pending;
    public decimal RestockQuantity { get; set; }
    public decimal QuarantineQuantity { get; set; }
    public decimal DamageQuantity { get; set; }
    public decimal ReplacementQuantity { get; set; }
    public decimal CreditAmount { get; set; }

    public ReturnRequest ReturnRequest { get; set; } = null!;
    public DeliveryItem? DeliveryItem { get; set; }
    public SKU SKU { get; set; } = null!;
    public Batch? Batch { get; set; }
}
