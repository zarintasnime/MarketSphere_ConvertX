using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.Inventory;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrderFulfilment;

public sealed class Delivery : AuditableEntity
{
    public int DeliveryID { get; set; }
    public string DeliveryNo { get; set; } = string.Empty;
    public int OrderID { get; set; }
    public int? InvoiceID { get; set; }
    public int? PickListID { get; set; }
    public int WarehouseID { get; set; }
    public DateTime? PlannedDeliveryDate { get; set; }
    public DateTime? DispatchDate { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
    public int? DeliveredByEmployeeID { get; set; }
    public string? ReceiverName { get; set; }
    public string? ReceiverPhone { get; set; }
    public string? FailureReason { get; set; }
    public DateTime? RescheduledDate { get; set; }

    public Order Order { get; set; } = null!;
    public Invoice? Invoice { get; set; }
    public PickList? PickList { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public Employee? DeliveredByEmployee { get; set; }
    public ICollection<DeliveryItem> Items { get; set; } = new List<DeliveryItem>();
}
