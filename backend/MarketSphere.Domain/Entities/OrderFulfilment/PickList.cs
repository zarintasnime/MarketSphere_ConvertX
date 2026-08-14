using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.Inventory;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrderFulfilment;

public sealed class PickList : AuditableEntity, IHasRowVersion
{
    public int PickListID { get; set; }
    public string PickListNo { get; set; } = string.Empty;
    public int OrderID { get; set; }
    public int? InvoiceID { get; set; }
    public int WarehouseID { get; set; }
    public string? WaveNo { get; set; }
    public PickListStatus Status { get; set; } = PickListStatus.Draft;
    public DateTime? ReleasedAt { get; set; }
    public int? ReleasedByEmployeeID { get; set; }
    public int? VerifiedByEmployeeID { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? Note { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Order Order { get; set; } = null!;
    public Invoice? Invoice { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public Employee? ReleasedByEmployee { get; set; }
    public Employee? VerifiedByEmployee { get; set; }
    public ICollection<PickListItem> Items { get; set; } = new List<PickListItem>();
    public ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
}
