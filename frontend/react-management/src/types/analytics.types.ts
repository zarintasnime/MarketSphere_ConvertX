export interface AnalyticsFilter {
  from: string;
  to: string;
  branchID?: number | null;
  regionID?: number | null;
  employeeID?: number | null;
  campaignID?: number | null;
}

export interface DashboardKpi {
  code: string;
  label: string;
  value: number;
  unit: string | null;
  changePercent: number | null;
}

export interface FunnelPoint {
  stage: string;
  count: number;
  value: number;
}

export interface SeriesPoint {
  period: string;
  value: number;
  series: string | null;
}

export interface ExecutiveDashboard {
  kpis: readonly DashboardKpi[];
  leadToOrderFunnel: readonly FunnelPoint[];
  salesTrend: readonly SeriesPoint[];
  pendingApprovals: number;
  overdueTasks: number;
  nearExpiryBatches: number;
}

export type SalesChannel = 1 | 2 | 3 | 4 | 5;
export type BatchStatus = 1 | 2 | 3 | 4 | 5;
export type ClientType = 1 | 2 | 3 | 4 | 5;
export type ClientLifecycleStatus = 1 | 2 | 3 | 4 | 5;
export type ClientRiskStatus = 1 | 2 | 3 | 4;
export type OrderStatus = 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11 | 12 | 13;
export type PaymentMethod = 1 | 2 | 3 | 4 | 5;
export type CustomerPaymentStatus = 1 | 2 | 3 | 4;
export type ComplaintPriority = 1 | 2 | 3 | 4;
export type ComplaintStatus = 1 | 2 | 3 | 4 | 5 | 6 | 7;

export interface CampaignRoi {
  campaignID: number;
  campaignCode: string;
  campaignTitle: string;
  budget: number;
  expense: number;
  attributedValue: number;
  deliveredValue: number;
  roiPercent: number;
}

export interface ChannelSales {
  channel: SalesChannel;
  orderCount: number;
  grossAmount: number;
  netAmount: number;
  deliveredValue: number;
}

export interface SellInSellOutPoint {
  period: string;
  sellInQuantity: number;
  sellInValue: number;
  sellOutQuantity: number;
  sellOutValue: number;
}

export interface InventoryHealthItem {
  warehouseID: number;
  warehouseName: string;
  skuID: number;
  skuCode: string;
  skuName: string;
  batchID: number | null;
  batchNo: string | null;
  expiryDate: string | null;
  batchStatus: BatchStatus | null;
  onHandQuantity: number;
  reservedQuantity: number;
  quarantineQuantity: number;
  damagedQuantity: number;
  availableQuantity: number;
  isLowStock: boolean;
  isNearExpiry: boolean;
}

export interface InventoryHealth {
  onHandQuantity: number;
  availableQuantity: number;
  reservedQuantity: number;
  quarantineQuantity: number;
  damagedQuantity: number;
  nearExpiryBatchCount: number;
  expiredBatchCount: number;
  lowStockSkuCount: number;
  lowStockThreshold: number;
  items: readonly InventoryHealthItem[];
}

export interface DeliveryReturnPoint {
  period: string;
  plannedCount: number;
  deliveredCount: number;
  partialCount: number;
  failedCount: number;
  rescheduledCount: number;
  returnRequestCount: number;
  returnedQuantity: number;
}

export interface EmployeeKpi {
  employeeID: number;
  employeeCode: string;
  employeeName: string;
  targetValue: number;
  actualValue: number;
  achievementPercent: number;
  rewardAmount: number;
}

export interface Client360Header {
  clientID: number;
  clientCode: string;
  clientName: string;
  clientType: ClientType;
  channel: SalesChannel;
  phone: string | null;
  email: string | null;
  address: string;
  lifecycleStatus: ClientLifecycleStatus;
  riskStatus: ClientRiskStatus;
  creditLimit: number;
  currentDue: number;
  isCreditBlocked: boolean;
  orderCount: number;
  orderValue: number;
  paidAmount: number;
  openComplaintCount: number;
}

export interface Client360Order {
  orderID: number;
  orderNo: string;
  orderDate: string;
  channel: SalesChannel;
  status: OrderStatus;
  netAmount: number;
}

export interface Client360Payment {
  paymentID: number;
  paymentNo: string;
  paymentDate: string;
  paymentMethod: PaymentMethod;
  status: CustomerPaymentStatus;
  amount: number;
}

export interface Client360Complaint {
  complaintID: number;
  complaintNo: string;
  openedAt: string;
  priority: ComplaintPriority;
  status: ComplaintStatus;
  subject: string;
}

export interface Client360TimelineItem {
  occurredAt: string;
  type: string;
  title: string;
  status: string | null;
  amount: number | null;
  referenceID: number | null;
}

export interface Client360 {
  header: Client360Header;
  recentOrders: readonly Client360Order[];
  recentPayments: readonly Client360Payment[];
  recentComplaints: readonly Client360Complaint[];
  timeline: readonly Client360TimelineItem[];
}
