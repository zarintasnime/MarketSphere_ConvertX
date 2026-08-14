namespace MarketSphere.Domain.Enums;

public enum WarehouseType { Main = 1, Regional = 2, Transit = 3, Returns = 4, Quarantine = 5 }
public enum BatchStatus { Available = 1, Quarantine = 2, Expired = 3, Blocked = 4, Depleted = 5 }
public enum StockMovementType
{
    GoodsReceipt = 1,
    SupplierReturn = 2,
    TransferOut = 3,
    TransferIn = 4,
    AdjustmentIn = 5,
    AdjustmentOut = 6,
    Reservation = 7,
    ReservationRelease = 8,
    DeliveryIssue = 9,
    CustomerReturn = 10
}
public enum StockReservationStatus { Active = 1, Released = 2, Consumed = 3, Expired = 4, Cancelled = 5 }
public enum StockTransferStatus { Draft = 1, Submitted = 2, Approved = 3, Dispatched = 4, PartiallyReceived = 5, Received = 6, Cancelled = 7 }
public enum StockAdjustmentStatus { Draft = 1, Submitted = 2, Approved = 3, Posted = 4, Rejected = 5, Cancelled = 6 }
