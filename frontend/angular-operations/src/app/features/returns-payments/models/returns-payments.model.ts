export enum ReturnRequestStatus {
  Requested = 1,
  UnderReview = 2,
  Approved = 3,
  Rejected = 4,
  Received = 5,
  Inspected = 6,
  Resolved = 7,
  Closed = 8,
}
export enum ReturnResolutionType {
  Restock = 1,
  Quarantine = 2,
  Damage = 3,
  Replacement = 4,
  Credit = 5,
  SupplierClaim = 6,
  Mixed = 7,
}
export enum ReturnConditionStatus {
  Unopened = 1,
  Saleable = 2,
  Damaged = 3,
  Expired = 4,
  Defective = 5,
  WrongItem = 6,
  Other = 7,
}
export enum ReturnDisposition {
  Pending = 1,
  Restock = 2,
  Quarantine = 3,
  Damage = 4,
  Replace = 5,
  Credit = 6,
  SupplierReturn = 7,
}
export enum PaymentMethod {
  Cash = 1,
  BankTransfer = 2,
  Cheque = 3,
  MobileFinancialService = 4,
  Other = 5,
}
export enum CustomerPaymentStatus {
  Pending = 1,
  Confirmed = 2,
  Rejected = 3,
  Reversed = 4,
}
export enum PaymentAllocationType {
  Allocation = 1,
  Reversal = 2,
}

export interface ReturnItem {
  returnItemID: number;
  deliveryItemID: number | null;
  skuID: number;
  skuCode: string;
  batchID: number | null;
  requestedQuantity: number;
  approvedQuantity: number;
  receivedQuantity: number;
  conditionStatus: ReturnConditionStatus | null;
  inspectionResult: string | null;
  disposition: ReturnDisposition;
  restockQuantity: number;
  quarantineQuantity: number;
  damageQuantity: number;
  replacementQuantity: number;
  creditAmount: number;
}
export interface ReturnListItem {
  returnRequestID: number;
  returnNo: string;
  clientID: number;
  orderID: number;
  invoiceID: number | null;
  deliveryID: number | null;
  requestDate: string;
  returnReason: string;
  status: ReturnRequestStatus;
  resolutionType: ReturnResolutionType | null;
}
export interface ReturnDetails {
  returnRequestID: number;
  returnNo: string;
  clientID: number;
  orderID: number;
  invoiceID: number | null;
  deliveryID: number | null;
  complaintID: number | null;
  requestDate: string;
  returnReason: string;
  description: string | null;
  status: ReturnRequestStatus;
  receivedAtWarehouseAt: string | null;
  resolutionType: ReturnResolutionType | null;
  replacementOrderID: number | null;
  replacementDeliveryID: number | null;
  supplierReturnID: number | null;
  resolvedByEmployeeID: number | null;
  resolvedAt: string | null;
  resolutionNote: string | null;
  items: readonly ReturnItem[];
}
export interface CreateReturnRequest {
  returnNo: string;
  clientID: number;
  orderID: number;
  invoiceID: number | null;
  deliveryID: number | null;
  complaintID: number | null;
  requestDate: string;
  returnReason: string;
  description: string | null;
  items: readonly { deliveryItemID: number; requestedQuantity: number }[];
}
export interface ApproveReturnRequest {
  items: readonly { returnItemID: number; approvedQuantity: number }[];
}
export interface ResolveReturnRequest {
  warehouseID: number;
  resolvedByEmployeeID: number;
  resolutionType: ReturnResolutionType;
  resolutionNote: string;
  creditNoteNo: string | null;
  items: readonly {
    returnItemID: number;
    receivedQuantity: number;
    conditionStatus: ReturnConditionStatus;
    inspectionResult: string | null;
    disposition: ReturnDisposition;
    restockQuantity: number;
    quarantineQuantity: number;
    damageQuantity: number;
    replacementQuantity: number;
    creditAmount: number;
  }[];
}
export interface CreditResolutionRow {
  returnRequestID: number;
  returnNo: string;
  clientID: number;
  invoiceID: number | null;
  resolvedAt: string | null;
  resolutionType: ReturnResolutionType | null;
  totalCreditAmount: number;
  status: ReturnRequestStatus;
}

export interface PaymentAllocation {
  paymentAllocationID: number;
  paymentID: number;
  invoiceID: number;
  allocationType: PaymentAllocationType;
  allocatedAmount: number;
  reversalOfPaymentAllocationID: number | null;
  allocatedAt: string;
  allocatedByUserID: number;
}
export interface PaymentListItem {
  paymentID: number;
  paymentNo: string;
  clientID: number;
  paymentDate: string;
  paymentMethod: PaymentMethod;
  amount: number;
  status: CustomerPaymentStatus;
  allocatedAmount: number;
  availableAmount: number;
}
export interface PaymentDetails {
  paymentID: number;
  paymentNo: string;
  clientID: number;
  paymentDate: string;
  paymentMethod: PaymentMethod;
  amount: number;
  referenceNo: string | null;
  proofFileAttachmentID: number | null;
  status: CustomerPaymentStatus;
  receivedByUserID: number;
  allocations: readonly PaymentAllocation[];
}
export interface CreatePaymentRequest {
  paymentNo: string;
  clientID: number;
  paymentDate: string;
  paymentMethod: PaymentMethod;
  amount: number;
  referenceNo: string | null;
  proofFileAttachmentID: number | null;
}
export interface ConfirmPaymentRequest {
  allocations: readonly { invoiceID: number; amount: number }[];
}
