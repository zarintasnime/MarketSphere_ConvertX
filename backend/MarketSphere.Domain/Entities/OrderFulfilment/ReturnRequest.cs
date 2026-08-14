using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Entities.Procurement;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrderFulfilment;

public sealed class ReturnRequest : AuditableEntity
{
    public int ReturnRequestID { get; set; }
    public string ReturnNo { get; set; } = string.Empty;
    public int ClientID { get; set; }
    public int OrderID { get; set; }
    public int? InvoiceID { get; set; }
    public int? DeliveryID { get; set; }
    public int? ComplaintID { get; set; }
    public DateTime RequestDate { get; set; }
    public string ReturnReason { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ReturnRequestStatus Status { get; set; } = ReturnRequestStatus.Requested;
    public DateTime? ReceivedAtWarehouseAt { get; set; }
    public ReturnResolutionType? ResolutionType { get; set; }
    public int? ReplacementOrderID { get; set; }
    public int? ReplacementDeliveryID { get; set; }
    public int? SupplierReturnID { get; set; }
    public int? ResolvedByEmployeeID { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolutionNote { get; set; }

    public Client Client { get; set; } = null!;
    public Order Order { get; set; } = null!;
    public Invoice? Invoice { get; set; }
    public Delivery? Delivery { get; set; }
    public Complaint? Complaint { get; set; }
    public Order? ReplacementOrder { get; set; }
    public Delivery? ReplacementDelivery { get; set; }
    public SupplierReturn? SupplierReturn { get; set; }
    public Employee? ResolvedByEmployee { get; set; }
    public ICollection<ReturnItem> Items { get; set; } = new List<ReturnItem>();
    public CreditNote? CreditNote { get; set; }
}
