export enum SupplierStatus {
  Active = 1,
  Suspended = 2,
  Inactive = 3,
}
export enum PurchaseRequisitionStatus {
  Draft = 1,
  Submitted = 2,
  Approved = 3,
  Rejected = 4,
  Closed = 5,
  Cancelled = 6,
}
export enum PurchaseOrderStatus {
  Draft = 1,
  Submitted = 2,
  Approved = 3,
  PartiallyReceived = 4,
  Received = 5,
  Closed = 6,
  Cancelled = 7,
}
export enum GoodsReceiptStatus {
  Draft = 1,
  QualityCheck = 2,
  Approved = 3,
  Rejected = 4,
  Posted = 5,
}
export enum QualityCheckStatus {
  Pending = 1,
  Passed = 2,
  PartiallyAccepted = 3,
  Failed = 4,
}
export enum PurchaseInvoiceStatus {
  Draft = 1,
  Confirmed = 2,
  Cancelled = 3,
}
export enum SupplierInvoicePaymentStatus {
  Unpaid = 1,
  PartiallyPaid = 2,
  Paid = 3,
}
export enum SupplierPaymentStatus {
  Pending = 1,
  Confirmed = 2,
  Rejected = 3,
  Reversed = 4,
}
export enum SupplierReturnStatus {
  Draft = 1,
  Submitted = 2,
  Approved = 3,
  Posted = 4,
  Cancelled = 5,
}
export enum PaymentMethod {
  Cash = 1,
  BankTransfer = 2,
  Cheque = 3,
  MobileFinancialService = 4,
  Other = 5,
}

export interface SupplierListItem {
  supplierID: number;
  supplierCode: string;
  supplierName: string;
  phone: string | null;
  paymentTermsDays: number;
  status: SupplierStatus;
}
export interface SupplierProduct {
  supplierProductID: number;
  skuID: number;
  skuCode: string;
  skuName: string;
  supplierSKUCode: string | null;
  lastPurchasePrice: number | null;
  minimumOrderQuantity: number | null;
  leadTimeDays: number | null;
  isPreferredSupplier: boolean;
  isActive: boolean;
}
export interface SupplierDetails extends SupplierListItem {
  contactPerson: string | null;
  email: string | null;
  address: string | null;
  products: readonly SupplierProduct[];
}
export interface SaveSupplierRequest {
  supplierCode: string;
  supplierName: string;
  contactPerson: string | null;
  phone: string | null;
  email: string | null;
  address: string | null;
  paymentTermsDays: number;
}
export interface SaveSupplierProductRequest {
  skuID: number;
  supplierSKUCode: string | null;
  lastPurchasePrice: number | null;
  minimumOrderQuantity: number | null;
  leadTimeDays: number | null;
  isPreferredSupplier: boolean;
  isActive: boolean;
}

export interface PurchaseRequisitionItemInput {
  skuID: number;
  requestedQuantity: number;
  estimatedUnitCost: number | null;
  note: string | null;
}
export interface SavePurchaseRequisitionRequest {
  purchaseRequisitionNo: string;
  branchID: number;
  requestedByEmployeeID: number;
  requiredDate: string;
  reason: string | null;
  items: readonly PurchaseRequisitionItemInput[];
}
export interface PurchaseRequisitionItem extends PurchaseRequisitionItemInput {
  purchaseRequisitionItemID: number;
  skuCode: string;
  skuName: string;
}
export interface PurchaseRequisitionListItem {
  purchaseRequisitionID: number;
  purchaseRequisitionNo: string;
  branchName: string;
  requestedBy: string;
  requiredDate: string;
  status: PurchaseRequisitionStatus;
  estimatedAmount: number;
}
export interface PurchaseRequisitionDetails extends SavePurchaseRequisitionRequest {
  purchaseRequisitionID: number;
  status: PurchaseRequisitionStatus;
  items: readonly PurchaseRequisitionItem[];
}

export interface PurchaseOrderItemInput {
  skuID: number;
  orderedQuantity: number;
  unitCost: number;
  discountAmount: number;
  taxAmount: number;
}
export interface SavePurchaseOrderRequest {
  purchaseOrderNo: string;
  supplierID: number;
  purchaseRequisitionID: number | null;
  branchID: number;
  orderDate: string;
  expectedDeliveryDate: string | null;
  items: readonly PurchaseOrderItemInput[];
}
export interface PurchaseOrderItem extends PurchaseOrderItemInput {
  purchaseOrderItemID: number;
  skuCode: string;
  skuName: string;
  receivedQuantity: number;
  lineTotal: number;
}
export interface PurchaseOrderListItem {
  purchaseOrderID: number;
  purchaseOrderNo: string;
  supplierName: string;
  orderDate: string;
  expectedDeliveryDate: string | null;
  status: PurchaseOrderStatus;
  netAmount: number;
}
export interface PurchaseOrderDetails extends SavePurchaseOrderRequest {
  purchaseOrderID: number;
  status: PurchaseOrderStatus;
  grossAmount: number;
  discountAmount: number;
  taxAmount: number;
  netAmount: number;
  items: readonly PurchaseOrderItem[];
}

export interface GoodsReceiptItemInput {
  purchaseOrderItemID: number;
  skuID: number;
  acceptedQuantity: number;
  rejectedQuantity: number;
  batchNo: string | null;
  manufacturingDate: string | null;
  expiryDate: string | null;
  unitCost: number;
  rejectionReason: string | null;
}
export interface SaveGoodsReceiptRequest {
  goodsReceiptNo: string;
  purchaseOrderID: number;
  warehouseID: number;
  receivedDate: string;
  receivedByEmployeeID: number;
  supplierChallanNo: string | null;
  items: readonly GoodsReceiptItemInput[];
}
export interface GoodsReceiptItem extends GoodsReceiptItemInput {
  goodsReceiptItemID: number;
  skuCode: string;
  batchID: number | null;
}
export interface GoodsReceiptListItem {
  goodsReceiptID: number;
  goodsReceiptNo: string;
  purchaseOrderNo: string;
  warehouseName: string;
  receivedDate: string;
  status: GoodsReceiptStatus;
  qualityCheckStatus: QualityCheckStatus;
}
export interface GoodsReceiptDetails extends SaveGoodsReceiptRequest {
  goodsReceiptID: number;
  status: GoodsReceiptStatus;
  qualityCheckStatus: QualityCheckStatus;
  items: readonly GoodsReceiptItem[];
}

export interface SavePurchaseInvoiceRequest {
  purchaseInvoiceNo: string;
  supplierID: number;
  purchaseOrderID: number | null;
  goodsReceiptID: number | null;
  invoiceDate: string;
  dueDate: string | null;
  grossAmount: number;
  discountAmount: number;
  taxAmount: number;
}
export interface PurchaseInvoice {
  purchaseInvoiceID: number;
  purchaseInvoiceNo: string;
  supplierID: number;
  supplierName: string;
  purchaseOrderID: number | null;
  goodsReceiptID: number | null;
  invoiceDate: string;
  dueDate: string | null;
  totalAmount: number;
  paidAmount: number;
  dueAmount: number;
  paymentStatus: SupplierInvoicePaymentStatus;
  status: PurchaseInvoiceStatus;
}
export interface CreateSupplierPaymentRequest {
  purchaseInvoiceID: number;
  paymentNo: string;
  paymentDate: string;
  paymentMethod: PaymentMethod;
  amount: number;
  referenceNo: string | null;
}
export interface SupplierPayment {
  supplierPaymentID: number;
  paymentNo: string;
  purchaseInvoiceID: number;
  paymentDate: string;
  paymentMethod: PaymentMethod;
  amount: number;
  referenceNo: string | null;
  status: SupplierPaymentStatus;
}

export interface SupplierReturnItemInput {
  skuID: number;
  batchID: number | null;
  quantity: number;
  unitCost: number;
  reason: string;
}
export interface SaveSupplierReturnRequest {
  supplierReturnNo: string;
  supplierID: number;
  goodsReceiptID: number | null;
  warehouseID: number;
  returnDate: string;
  reason: string;
  items: readonly SupplierReturnItemInput[];
}
export interface SupplierReturnItem extends SupplierReturnItemInput {
  supplierReturnItemID: number;
  skuCode: string;
  stockMovementID: number | null;
}
export interface SupplierReturnListItem {
  supplierReturnID: number;
  supplierReturnNo: string;
  supplierName: string;
  warehouseName: string;
  returnDate: string;
  status: SupplierReturnStatus;
}
export interface SupplierReturnDetails extends SaveSupplierReturnRequest {
  supplierReturnID: number;
  status: SupplierReturnStatus;
  items: readonly SupplierReturnItem[];
}
