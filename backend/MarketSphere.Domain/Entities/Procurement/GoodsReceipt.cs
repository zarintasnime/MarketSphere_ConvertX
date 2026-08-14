using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.Inventory;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Procurement;

public sealed class GoodsReceipt : AuditableEntity
{
    public int GoodsReceiptID { get; set; }
    public string GoodsReceiptNo { get; set; } = string.Empty;
    public int PurchaseOrderID { get; set; }
    public int WarehouseID { get; set; }
    public DateTime ReceivedDate { get; set; }
    public int ReceivedByEmployeeID { get; set; }
    public string? SupplierChallanNo { get; set; }
    public GoodsReceiptStatus Status { get; set; } = GoodsReceiptStatus.Draft;
    public QualityCheckStatus QualityCheckStatus { get; set; } = QualityCheckStatus.Pending;

    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public Employee ReceivedByEmployee { get; set; } = null!;
    public ICollection<GoodsReceiptItem> Items { get; set; } = new List<GoodsReceiptItem>();
}
