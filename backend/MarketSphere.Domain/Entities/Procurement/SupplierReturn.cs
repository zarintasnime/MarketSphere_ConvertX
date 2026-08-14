using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.Inventory;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Procurement;

public sealed class SupplierReturn : AuditableEntity
{
    public int SupplierReturnID { get; set; }
    public string SupplierReturnNo { get; set; } = string.Empty;
    public int SupplierID { get; set; }
    public int? GoodsReceiptID { get; set; }
    public int WarehouseID { get; set; }
    public DateTime ReturnDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public SupplierReturnStatus Status { get; set; } = SupplierReturnStatus.Draft;

    public Supplier Supplier { get; set; } = null!;
    public GoodsReceipt? GoodsReceipt { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public ICollection<SupplierReturnItem> Items { get; set; } = new List<SupplierReturnItem>();
}
