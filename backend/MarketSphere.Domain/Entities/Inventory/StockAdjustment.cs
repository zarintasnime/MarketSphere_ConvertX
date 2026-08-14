using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Inventory;

public sealed class StockAdjustment : AuditableEntity
{
    public int StockAdjustmentID { get; set; }
    public string StockAdjustmentNo { get; set; } = string.Empty;
    public int WarehouseID { get; set; }
    public DateTime AdjustmentDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public StockAdjustmentStatus Status { get; set; } = StockAdjustmentStatus.Draft;
    public int PerformedByEmployeeID { get; set; }

    public Warehouse Warehouse { get; set; } = null!;
    public Employee PerformedByEmployee { get; set; } = null!;
    public ICollection<StockAdjustmentItem> Items { get; set; } = new List<StockAdjustmentItem>();
}
