using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrderFulfilment;

public sealed class ModernTradePurchaseOrder : AuditableEntity
{
    public int ModernTradePurchaseOrderID { get; set; }
    public int ClientID { get; set; }
    public string PONumber { get; set; } = string.Empty;
    public DateTime PODate { get; set; }
    public DateTime ReceivedDate { get; set; }
    public int UploadedByEmployeeID { get; set; }
    public ModernTradePurchaseOrderStatus Status { get; set; } = ModernTradePurchaseOrderStatus.Draft;
    public ModernTradeVerificationStatus VerificationStatus { get; set; } = ModernTradeVerificationStatus.Pending;
    public ModernTradeCompletenessStatus CompletenessStatus { get; set; } = ModernTradeCompletenessStatus.Incomplete;
    public string? VerificationNote { get; set; }
    public string? RejectionReason { get; set; }
    public int? VerifiedByEmployeeID { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? DuplicateHash { get; set; }
    public DateTime? RequestedDeliveryDate { get; set; }

    public Client Client { get; set; } = null!;
    public Employee UploadedByEmployee { get; set; } = null!;
    public Employee? VerifiedByEmployee { get; set; }
    public ICollection<ModernTradePurchaseOrderItem> Items { get; set; } = new List<ModernTradePurchaseOrderItem>();
    public Order? ConvertedOrder { get; set; }
}
