export enum SalesChannel {
  GeneralTrade = 1,
  ModernTrade = 2,
  BusinessPartner = 3,
  Institutional = 4,
  Online = 5,
}
export enum ModernTradePurchaseOrderStatus {
  Draft = 1,
  Submitted = 2,
  Verified = 3,
  Rejected = 4,
  Converted = 5,
  Cancelled = 6,
}
export enum ModernTradeVerificationStatus {
  Pending = 1,
  Incomplete = 2,
  MappingRequired = 3,
  Verified = 4,
  Rejected = 5,
}
export enum ModernTradeCompletenessStatus {
  Incomplete = 1,
  Complete = 2,
}
export enum ItemMappingStatus {
  Unmapped = 1,
  Mapped = 2,
  Rejected = 3,
}
export enum OrderSource {
  Regular = 1,
  Quotation = 2,
  ModernTradePurchaseOrder = 3,
  Campaign = 4,
}
export enum OrderStatus {
  Draft = 1,
  Submitted = 2,
  UnderReview = 3,
  Approved = 4,
  StockAllocated = 5,
  Invoiced = 6,
  ReadyForDispatch = 7,
  PartiallyDelivered = 8,
  Delivered = 9,
  Returned = 10,
  Closed = 11,
  Rejected = 12,
  Cancelled = 13,
}
export enum CreditCheckStatus {
  NotRequired = 1,
  Pending = 2,
  Passed = 3,
  Failed = 4,
  OverrideRequired = 5,
}
export enum AppliedBenefitType {
  PercentageDiscount = 1,
  FixedDiscount = 2,
  FreeItem = 3,
  Bundle = 4,
  Cashback = 5,
  Other = 6,
}
export enum ApprovalRequestStatus {
  Pending = 1,
  InProgress = 2,
  Approved = 3,
  Rejected = 4,
  Cancelled = 5,
}
export enum ApprovalActionType {
  Submitted = 1,
  Approved = 2,
  Rejected = 3,
  Cancelled = 4,
  Delegated = 5,
  Commented = 6,
}
export enum ApprovalType {
  Discount = 1,
  CreditOverride = 2,
  Order = 3,
  PurchaseRequisition = 4,
  PurchaseOrder = 5,
  StockTransfer = 6,
  StockAdjustment = 7,
  Return = 8,
  SupplierReturn = 9,
  Reward = 10,
  Other = 99,
}

export interface ModernTradePurchaseOrderItem {
  modernTradePurchaseOrderItemID: number;
  externalItemCode: string | null;
  externalItemName: string | null;
  skuID: number | null;
  skuCode: string | null;
  mappingStatus: ItemMappingStatus;
  orderedQuantity: number;
  agreedUnitPrice: number | null;
  discount: number | null;
  note: string | null;
}
export interface ModernTradePurchaseOrderListItem {
  modernTradePurchaseOrderID: number;
  poNumber: string;
  clientID: number;
  clientName: string;
  poDate: string;
  receivedDate: string;
  status: ModernTradePurchaseOrderStatus;
  verificationStatus: ModernTradeVerificationStatus;
  completenessStatus: ModernTradeCompletenessStatus;
}
export interface ModernTradePurchaseOrderDetails {
  modernTradePurchaseOrderID: number;
  clientID: number;
  poNumber: string;
  poDate: string;
  receivedDate: string;
  uploadedByEmployeeID: number;
  status: ModernTradePurchaseOrderStatus;
  verificationStatus: ModernTradeVerificationStatus;
  completenessStatus: ModernTradeCompletenessStatus;
  verificationNote: string | null;
  rejectionReason: string | null;
  verifiedByEmployeeID: number | null;
  verifiedAt: string | null;
  duplicateHash: string | null;
  requestedDeliveryDate: string | null;
  items: readonly ModernTradePurchaseOrderItem[];
}
export interface SaveModernTradePurchaseOrderItemRequest {
  externalItemCode: string | null;
  externalItemName: string | null;
  skuID: number | null;
  orderedQuantity: number;
  agreedUnitPrice: number | null;
  discount: number | null;
  note: string | null;
}
export interface SaveModernTradePurchaseOrderRequest {
  clientID: number;
  poNumber: string;
  poDate: string;
  receivedDate: string;
  uploadedByEmployeeID: number;
  duplicateHash: string | null;
  requestedDeliveryDate: string | null;
  items: readonly SaveModernTradePurchaseOrderItemRequest[];
}
export interface VerifyModernTradePurchaseOrderRequest {
  approve: boolean;
  verifiedByEmployeeID: number;
  note: string | null;
  rejectionReason: string | null;
}

export interface OrderItem {
  orderItemID: number;
  skuID: number;
  skuCode: string;
  skuName: string;
  orderedQuantity: number;
  freeQuantity: number;
  unitPrice: number;
  discountPercent: number;
  discountAmount: number;
  taxAmount: number;
  lineTotal: number;
  approvedQuantity: number;
  deliveredQuantity: number;
  returnedQuantity: number;
  backorderQuantity: number;
}
export interface OrderListItem {
  orderID: number;
  orderNo: string;
  clientID: number;
  clientName: string;
  channel: SalesChannel;
  orderSource: OrderSource;
  orderDate: string;
  status: OrderStatus;
  creditCheckStatus: CreditCheckStatus;
  netAmount: number;
}
export interface OrderDetails {
  orderID: number;
  orderNo: string;
  clientID: number;
  employeeID: number | null;
  channel: SalesChannel;
  orderSource: OrderSource;
  campaignID: number | null;
  quotationID: number | null;
  modernTradePurchaseOrderID: number | null;
  priceListID: number | null;
  orderDate: string;
  requestedDeliveryDate: string | null;
  deliveryAddressSnapshot: string;
  status: OrderStatus;
  grossAmount: number;
  discountAmount: number;
  taxAmount: number;
  netAmount: number;
  creditCheckStatus: CreditCheckStatus;
  approvalRequestID: number | null;
  items: readonly OrderItem[];
}
export interface SaveOrderItemRequest {
  skuID: number;
  orderedQuantity: number;
  freeQuantity: number;
  unitPrice: number;
  discountPercent: number;
  taxAmount: number;
}
export interface SaveRegularOrderRequest {
  orderNo: string;
  clientID: number;
  employeeID: number | null;
  channel: SalesChannel;
  campaignID: number | null;
  priceListID: number | null;
  orderDate: string;
  requestedDeliveryDate: string | null;
  deliveryAddressSnapshot: string;
  items: readonly SaveOrderItemRequest[];
}
export interface ConvertQuotationToOrderRequest {
  orderNo: string;
  quotationID: number;
  employeeID: number | null;
  orderDate: string;
  requestedDeliveryDate: string | null;
  deliveryAddressSnapshot: string;
}
export interface ConvertModernTradePurchaseOrderRequest {
  orderNo: string;
  modernTradePurchaseOrderID: number;
  employeeID: number | null;
  priceListID: number | null;
  orderDate: string;
  deliveryAddressSnapshot: string;
}
export interface ApproveAndReserveOrderRequest {
  warehouseID: number;
  approvalRequestID: number | null;
  reservationExpiresAt: string | null;
}

export interface AppliedOffer {
  appliedOfferID: number;
  quotationID: number | null;
  quotationItemID: number | null;
  orderID: number | null;
  orderItemID: number | null;
  campaignOfferID: number;
  benefitType: AppliedBenefitType;
  benefitAmount: number | null;
  freeSKUID: number | null;
  freeQuantity: number | null;
  ruleSnapshotJson: string;
  usageCount: number;
  appliedAt: string;
  appliedByUserID: number | null;
}
export interface ApplyOfferRequest {
  quotationID: number | null;
  quotationItemID: number | null;
  orderID: number | null;
  orderItemID: number | null;
  campaignOfferID: number;
  benefitType: AppliedBenefitType;
  benefitAmount: number | null;
  freeSKUID: number | null;
  freeQuantity: number | null;
  ruleSnapshotJson: string;
  usageCount: number;
}

export interface ApprovalAction {
  approvalActionID: number;
  stepNo: number;
  actionByUserID: number;
  actionByName: string;
  action: ApprovalActionType;
  actionAt: string;
  note: string | null;
  delegatedFromUserID: number | null;
}
export interface ApprovalRequest {
  approvalRequestID: number;
  referenceType: string;
  referenceID: number;
  approvalType: ApprovalType;
  approvalPolicyID: number;
  requestedByUserID: number;
  requestedAt: string;
  currentStepNo: number;
  status: ApprovalRequestStatus;
  completedAt: string | null;
  actions: readonly ApprovalAction[];
}
export interface ApprovalActionRequest {
  action: ApprovalActionType;
  note: string | null;
  delegateToUserID: number | null;
}
