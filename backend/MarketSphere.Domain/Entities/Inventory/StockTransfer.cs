using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.Infrastructure;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Inventory;

public sealed class StockTransfer : AuditableEntity
{
    public int StockTransferID { get; set; }
    public string StockTransferNo { get; set; } = string.Empty;
    public int FromWarehouseID { get; set; }
    public int ToWarehouseID { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? DispatchedAt { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public StockTransferStatus Status { get; set; } = StockTransferStatus.Draft;
    public int? ApprovalRequestID { get; set; }

    public Warehouse FromWarehouse { get; set; } = null!;
    public Warehouse ToWarehouse { get; set; } = null!;
    public ApprovalRequest? ApprovalRequest { get; set; }
    public ICollection<StockTransferItem> Items { get; set; } = new List<StockTransferItem>();
}
