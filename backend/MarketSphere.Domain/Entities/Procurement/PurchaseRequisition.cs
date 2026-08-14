using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Procurement;

public sealed class PurchaseRequisition : AuditableEntity
{
    public int PurchaseRequisitionID { get; set; }
    public string PurchaseRequisitionNo { get; set; } = string.Empty;
    public int BranchID { get; set; }
    public int RequestedByEmployeeID { get; set; }
    public DateTime RequiredDate { get; set; }
    public string? Reason { get; set; }
    public PurchaseRequisitionStatus Status { get; set; } = PurchaseRequisitionStatus.Draft;

    public Branch Branch { get; set; } = null!;
    public Employee RequestedByEmployee { get; set; } = null!;
    public ICollection<PurchaseRequisitionItem> Items { get; set; } = new List<PurchaseRequisitionItem>();
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}
