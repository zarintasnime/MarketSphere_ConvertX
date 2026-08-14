export enum InvoiceStatus {
  Draft = 1,
  Issued = 2,
  PartiallyPaid = 3,
  Paid = 4,
  PartiallyCredited = 5,
  Credited = 6,
  Cancelled = 7,
}
export enum PickListStatus {
  Draft = 1,
  Released = 2,
  Picking = 3,
  PartiallyPicked = 4,
  Picked = 5,
  Verified = 6,
  Cancelled = 7,
}
export enum DeliveryStatus {
  Pending = 1,
  ReadyForDispatch = 2,
  Dispatched = 3,
  PartiallyDelivered = 4,
  Delivered = 5,
  Failed = 6,
  Rescheduled = 7,
  Cancelled = 8,
}

export interface InvoiceItem {
  invoiceItemID: number;
  orderItemID: number;
  skuID: number;
  skuCode: string;
  quantity: number;
  unitPrice: number;
  discountAmount: number;
  taxAmount: number;
  lineTotal: number;
}
export interface InvoiceListItem {
  invoiceID: number;
  invoiceNo: string;
  orderID: number;
  clientID: number;
  invoiceDate: string;
  dueDate: string | null;
  totalAmount: number;
  paidAmount: number;
  dueAmount: number;
  status: InvoiceStatus;
}
export interface InvoiceDetails {
  invoiceID: number;
  invoiceNo: string;
  orderID: number;
  clientID: number;
  invoiceDate: string;
  dueDate: string | null;
  grossAmount: number;
  discountAmount: number;
  taxAmount: number;
  totalAmount: number;
  paidAmount: number;
  dueAmount: number;
  status: InvoiceStatus;
  items: readonly InvoiceItem[];
}
export interface CreateInvoiceItemRequest {
  orderItemID: number;
  quantity: number;
}
export interface CreateInvoiceRequest {
  invoiceNo: string;
  orderID: number;
  invoiceDate: string;
  dueDate: string | null;
  items: readonly CreateInvoiceItemRequest[];
}

export interface PickListItem {
  pickListItemID: number;
  orderItemID: number;
  stockReservationID: number | null;
  skuID: number;
  skuCode: string;
  batchID: number | null;
  requestedQuantity: number;
  pickedQuantity: number;
  shortQuantity: number;
  pickedByEmployeeID: number | null;
  pickedAt: string | null;
  verificationNote: string | null;
}
export interface PickListListItem {
  pickListID: number;
  pickListNo: string;
  orderID: number;
  invoiceID: number | null;
  warehouseID: number;
  waveNo: string | null;
  status: PickListStatus;
  releasedAt: string | null;
  verifiedAt: string | null;
}
export interface PickListDetails {
  pickListID: number;
  pickListNo: string;
  orderID: number;
  invoiceID: number | null;
  warehouseID: number;
  waveNo: string | null;
  status: PickListStatus;
  releasedAt: string | null;
  releasedByEmployeeID: number | null;
  verifiedByEmployeeID: number | null;
  verifiedAt: string | null;
  note: string | null;
  items: readonly PickListItem[];
}
export interface CreatePickListRequest {
  pickListNo: string;
  orderID: number;
  invoiceID: number | null;
  warehouseID: number;
  waveNo: string | null;
  note: string | null;
}
export interface RecordPickRequest {
  pickListItemID: number;
  pickedQuantity: number;
  shortQuantity: number;
  pickedByEmployeeID: number;
  verificationNote: string | null;
}

export interface DeliveryItem {
  deliveryItemID: number;
  pickListItemID: number | null;
  orderItemID: number;
  invoiceItemID: number | null;
  skuID: number;
  skuCode: string;
  batchID: number | null;
  quantityDispatched: number;
  quantityDelivered: number;
  quantityRejectedAtDelivery: number;
}
export interface DeliveryListItem {
  deliveryID: number;
  deliveryNo: string;
  orderID: number;
  invoiceID: number | null;
  pickListID: number | null;
  warehouseID: number;
  plannedDeliveryDate: string | null;
  dispatchDate: string | null;
  deliveredAt: string | null;
  status: DeliveryStatus;
}
export interface DeliveryDetails {
  deliveryID: number;
  deliveryNo: string;
  orderID: number;
  invoiceID: number | null;
  pickListID: number | null;
  warehouseID: number;
  plannedDeliveryDate: string | null;
  dispatchDate: string | null;
  deliveredAt: string | null;
  status: DeliveryStatus;
  deliveredByEmployeeID: number | null;
  receiverName: string | null;
  receiverPhone: string | null;
  failureReason: string | null;
  rescheduledDate: string | null;
  items: readonly DeliveryItem[];
}
export interface CreateDeliveryRequest {
  deliveryNo: string;
  orderID: number;
  invoiceID: number | null;
  pickListID: number;
  warehouseID: number;
  plannedDeliveryDate: string | null;
}
export interface CompleteDeliveryItemRequest {
  deliveryItemID: number;
  quantityDelivered: number;
  quantityRejectedAtDelivery: number;
}
export interface CompleteDeliveryRequest {
  status: DeliveryStatus;
  receiverName: string | null;
  receiverPhone: string | null;
  failureReason: string | null;
  rescheduledDate: string | null;
  items: readonly CompleteDeliveryItemRequest[];
}
