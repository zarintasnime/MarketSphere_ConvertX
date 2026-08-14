export interface FieldActiveVisit {
  visitID: number;
  employeeID: number;
  clientID: number;
  clientCode: string;
  clientName: string;
  routeID: number | null;
  routeName: string | null;
  campaignID: number | null;
  visitType: number;
  checkInAt: string;
  checkInGPSLat: number;
  checkInGPSLng: number;
  accuracyMeters: number | null;
  isSuspiciousLocation: boolean;
  note: string | null;
}
export interface FieldAssignedClient {
  clientID: number;
  clientCode: string;
  clientName: string;
  clientType: number;
  channel: number;
  phone: string | null;
  address: string;
  gpsLat: number | null;
  gpsLng: number | null;
  regionID: number | null;
  areaID: number | null;
  territoryID: number | null;
  routeID: number | null;
  routeCode: string | null;
  routeName: string | null;
  sequenceNo: number | null;
}
export interface FieldVisitListItem {
  visitID: number;
  clientID: number;
  clientCode: string;
  clientName: string;
  routeID: number | null;
  routeName: string | null;
  campaignID: number | null;
  visitType: number;
  checkInAt: string;
  checkOutAt: string | null;
  status: number;
  isSuspiciousLocation: boolean;
}
export interface FieldWorkspaceSummary {
  employeeID: number;
  employeeCode: string;
  employeeName: string;
  designationName: string;
  branchID: number;
  branchName: string;
  assignedClientCount: number;
  todayVisitCount: number;
  completedVisitCount: number;
  unreadNotificationCount: number;
  activeVisit: FieldActiveVisit | null;
}
export interface NotificationItem {
  notificationID: number;
  notificationType: number;
  title: string;
  message: string;
  priority: number;
  referenceType: string | null;
  referenceID: number | null;
  isRead: boolean;
  createdAt: string;
  expiresAt: string | null;
  readAt: string | null;
}
export interface MtPoItemDraft {
  externalItemCode: string;
  externalItemName: string;
  skuID: number | null;
  orderedQuantity: number;
  agreedUnitPrice: number | null;
  discount: number | null;
  note: string;
}
export interface SaveModernTradePurchaseOrderRequest {
  clientID: number;
  poNumber: string;
  poDate: string;
  receivedDate: string;
  uploadedByEmployeeID: number;
  duplicateHash: string | null;
  requestedDeliveryDate: string | null;
  items: readonly MtPoItemDraft[];
}
