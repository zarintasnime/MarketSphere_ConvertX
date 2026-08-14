export enum WarehouseType {
  Main = 1,
  Regional = 2,
  Transit = 3,
  Returns = 4,
  Quarantine = 5,
}
export enum BatchStatus {
  Available = 1,
  Quarantine = 2,
  Expired = 3,
  Blocked = 4,
  Depleted = 5,
}
export enum StockMovementType {
  GoodsReceipt = 1,
  SalesIssue = 2,
  SalesReturn = 3,
  SupplierReturn = 4,
  TransferOut = 5,
  TransferIn = 6,
  AdjustmentIn = 7,
  AdjustmentOut = 8,
  Reservation = 9,
  ReservationRelease = 10,
}
export enum StockReservationStatus {
  Active = 1,
  Released = 2,
  Consumed = 3,
  Expired = 4,
  Cancelled = 5,
}
export enum StockTransferStatus {
  Draft = 1,
  Submitted = 2,
  Approved = 3,
  Dispatched = 4,
  PartiallyReceived = 5,
  Received = 6,
  Cancelled = 7,
}
export enum StockAdjustmentStatus {
  Draft = 1,
  Submitted = 2,
  Approved = 3,
  Posted = 4,
  Rejected = 5,
  Cancelled = 6,
}

export interface Warehouse {
  warehouseID: number;
  branchID: number;
  branchName: string;
  warehouseCode: string;
  warehouseName: string;
  warehouseType: WarehouseType;
  address: string | null;
  isActive: boolean;
}
export interface SaveWarehouseRequest {
  branchID: number;
  warehouseCode: string;
  warehouseName: string;
  warehouseType: WarehouseType;
  address: string | null;
}
export interface Batch {
  batchID: number;
  skuID: number;
  skuCode: string;
  batchNo: string;
  manufacturingDate: string | null;
  expiryDate: string | null;
  bestBeforeDate: string | null;
  costPrice: number;
  status: BatchStatus;
}
export interface StockBalance {
  stockBalanceID: number;
  warehouseID: number;
  warehouseName: string;
  skuID: number;
  skuCode: string;
  skuName: string;
  batchID: number | null;
  batchNo: string | null;
  expiryDate: string | null;
  onHandQuantity: number;
  reservedQuantity: number;
  quarantineQuantity: number;
  damagedQuantity: number;
  availableQuantity: number;
  rowVersion: string;
}
export interface StockMovement {
  stockMovementID: number;
  warehouseID: number;
  warehouseName: string;
  skuID: number;
  skuCode: string;
  skuName: string;
  batchID: number | null;
  batchNo: string | null;
  movementType: StockMovementType;
  quantityIn: number;
  quantityOut: number;
  balanceAfter: number;
  referenceType: string;
  referenceID: number;
  movementAt: string;
  note: string | null;
}
export interface StockReservation {
  stockReservationID: number;
  orderItemID: number;
  warehouseID: number;
  warehouseName: string;
  skuID: number;
  skuCode: string;
  skuName: string;
  batchID: number | null;
  batchNo: string | null;
  reservedQuantity: number;
  reservationStatus: StockReservationStatus;
  reservedAt: string;
  expiresAt: string | null;
  releasedAt: string | null;
}
export interface StockSearchRequest {
  warehouseID: number | null;
  skuID: number | null;
  batchID: number | null;
  includeZero: boolean;
  includeExpired: boolean;
}

export interface StockTransferItemInput {
  skuID: number;
  batchID: number | null;
  requestedQuantity: number;
}
export interface SaveStockTransferRequest {
  stockTransferNo: string;
  fromWarehouseID: number;
  toWarehouseID: number;
  requestedAt: string;
  items: readonly StockTransferItemInput[];
}
export interface StockTransferItem extends StockTransferItemInput {
  stockTransferItemID: number;
  skuCode: string;
  batchNo: string | null;
  dispatchedQuantity: number;
  receivedQuantity: number;
}
export interface StockTransferListItem {
  stockTransferID: number;
  stockTransferNo: string;
  fromWarehouse: string;
  toWarehouse: string;
  requestedAt: string;
  status: StockTransferStatus;
}
export interface StockTransferDetails extends SaveStockTransferRequest {
  stockTransferID: number;
  dispatchedAt: string | null;
  receivedAt: string | null;
  status: StockTransferStatus;
  approvalRequestID: number | null;
  items: readonly StockTransferItem[];
}
export interface DispatchStockTransferRequest {
  items: readonly { stockTransferItemID: number; dispatchedQuantity: number }[];
  note: string | null;
}
export interface ReceiveStockTransferRequest {
  items: readonly { stockTransferItemID: number; receivedQuantity: number }[];
  note: string | null;
}

export interface StockAdjustmentItemInput {
  skuID: number;
  batchID: number | null;
  adjustmentQuantity: number;
  unitCost: number | null;
  note: string | null;
}
export interface SaveStockAdjustmentRequest {
  stockAdjustmentNo: string;
  warehouseID: number;
  adjustmentDate: string;
  reason: string;
  performedByEmployeeID: number;
  items: readonly StockAdjustmentItemInput[];
}
export interface StockAdjustmentItem extends StockAdjustmentItemInput {
  stockAdjustmentItemID: number;
  skuCode: string;
  stockMovementID: number | null;
}
export interface StockAdjustmentListItem {
  stockAdjustmentID: number;
  stockAdjustmentNo: string;
  warehouseName: string;
  adjustmentDate: string;
  reason: string;
  status: StockAdjustmentStatus;
}
export interface StockAdjustmentDetails extends SaveStockAdjustmentRequest {
  stockAdjustmentID: number;
  status: StockAdjustmentStatus;
  items: readonly StockAdjustmentItem[];
}
