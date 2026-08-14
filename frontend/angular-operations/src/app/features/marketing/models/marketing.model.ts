export interface PagedRequest {
  pageNumber: number;
  pageSize: number;
  search?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}

export interface SelectOption<T extends string | number = number> {
  value: T;
  label: string;
}

export interface CampaignListItem {
  campaignID: number;
  campaignCode: string;
  campaignTitle: string;
  budget: number;
  actualExpense: number;
  startDate: string;
  endDate: string;
  channel: number;
  status: number;
  createdByEmployeeID: number;
}
export interface CampaignTarget {
  campaignTargetID: number;
  campaignID: number;
  targetType: number;
  regionID: number | null;
  areaID: number | null;
  clientSegmentID: number | null;
  clientID: number | null;
  productCategoryID: number | null;
  skuID: number | null;
  targetValue: number | null;
}
export interface CampaignOffer {
  campaignOfferID: number;
  campaignID: number;
  offerCode: string;
  offerType: number;
  ruleJson: string;
  discountValue: number | null;
  freeSKUID: number | null;
  priority: number;
  usageLimit: number | null;
  perClientLimit: number | null;
  isStackable: boolean;
  isActive: boolean;
}
export interface CampaignExpense {
  campaignExpenseID: number;
  campaignID: number;
  expenseDate: string;
  expenseCategory: string;
  amount: number;
  vendorName: string | null;
  description: string | null;
  status: number;
}
export interface CampaignAttribution {
  campaignAttributionID: number;
  campaignID: number;
  leadID: number | null;
  opportunityID: number | null;
  quotationID: number | null;
  orderID: number | null;
  attributionType: number;
  weightPercent: number;
  attributedAmount: number | null;
}
export interface CampaignDetails extends CampaignListItem {
  objective: string;
  targets: readonly CampaignTarget[];
  offers: readonly CampaignOffer[];
  expenses: readonly CampaignExpense[];
  attributions: readonly CampaignAttribution[];
}
export interface SaveCampaignRequest {
  campaignCode: string;
  campaignTitle: string;
  objective: string;
  budget: number;
  startDate: string;
  endDate: string;
  channel: number;
  createdByEmployeeID: number;
}
export interface CampaignRoi {
  campaignID: number;
  budget: number;
  actualExpense: number;
  attributedAmount: number;
  roiAmount: number;
  roiPercent: number | null;
}

export interface VisitListItem {
  visitID: number;
  employeeID: number;
  clientID: number;
  routeID: number | null;
  campaignID: number | null;
  visitType: number;
  checkInAt: string;
  checkOutAt: string | null;
  status: number;
  isSuspiciousLocation: boolean;
}
export interface VisitDetails extends VisitListItem {
  checkInGPSLat: number;
  checkInGPSLng: number;
  checkOutGPSLat: number | null;
  checkOutGPSLng: number | null;
  accuracyMeters: number | null;
  note: string | null;
}
export interface CheckInVisitRequest {
  employeeID: number;
  clientID: number;
  routeID: number | null;
  campaignID: number | null;
  visitType: number;
  checkInAt: string | null;
  checkInGPSLat: number;
  checkInGPSLng: number;
  accuracyMeters: number | null;
  note: string | null;
}
export interface CheckOutVisitRequest {
  checkOutAt: string | null;
  checkOutGPSLat: number;
  checkOutGPSLng: number;
  note: string | null;
}

export interface SamplingLogListItem {
  samplingLogID: number;
  visitID: number | null;
  campaignID: number | null;
  clientID: number | null;
  leadID: number | null;
  employeeID: number;
  skuID: number;
  issuedQuantity: number;
  consumedQuantity: number;
  returnedQuantity: number;
  damagedQuantity: number;
  sampleDate: string;
  outcome: number;
  followUpRequired: boolean;
}
export interface SaveSamplingLogRequest {
  visitID: number | null;
  campaignID: number | null;
  clientID: number | null;
  leadID: number | null;
  employeeID: number;
  skuID: number;
  issuedQuantity: number;
  consumedQuantity: number;
  returnedQuantity: number;
  damagedQuantity: number;
  sampleDate: string;
  feedbackSummary: string | null;
  outcome: number;
  followUpRequired: boolean;
}

export interface FeedbackListItem {
  feedbackID: number;
  clientID: number | null;
  leadID: number | null;
  campaignID: number | null;
  visitID: number | null;
  submittedByEmployeeID: number | null;
  feedbackType: number;
  rating: number | null;
  submittedAt: string;
  isFollowUpRequired: boolean;
}
export interface SaveFeedbackRequest {
  clientID: number | null;
  leadID: number | null;
  campaignID: number | null;
  visitID: number | null;
  submittedByEmployeeID: number | null;
  feedbackType: number;
  rating: number | null;
  comments: string | null;
  submittedAt: string | null;
  isFollowUpRequired: boolean;
}

export interface MarketObservationListItem {
  marketObservationID: number;
  visitID: number;
  clientID: number;
  employeeID: number;
  observationType: number;
  skuID: number | null;
  availabilityStatus: number | null;
  competitorBrand: string | null;
  competitorPrice: number | null;
}
export interface SaveMarketObservationRequest {
  visitID: number;
  clientID: number;
  employeeID: number;
  observationType: number;
  skuID: number | null;
  availabilityStatus: number | null;
  facingCount: number | null;
  planogramScore: number | null;
  displayScore: number | null;
  competitorBrand: string | null;
  competitorProduct: string | null;
  competitorPrice: number | null;
  competitorOffer: string | null;
  note: string | null;
}

export interface BpSellOutListItem {
  bpSellOutID: number;
  employeeID: number;
  clientID: number;
  visitID: number | null;
  campaignID: number | null;
  sellOutDate: string;
  totalQuantity: number;
  totalValue: number;
  verificationStatus: number;
  verifiedByEmployeeID: number | null;
  verifiedAt: string | null;
}
export interface BpSellOutItemRequest {
  skuID: number;
  quantitySold: number;
  unitSellingPrice: number | null;
}
export interface SaveBpSellOutRequest {
  employeeID: number;
  clientID: number;
  visitID: number | null;
  campaignID: number | null;
  sellOutDate: string;
  gpsLat: number | null;
  gpsLng: number | null;
  items: readonly BpSellOutItemRequest[];
}

export const CAMPAIGN_STATUS_OPTIONS: readonly SelectOption[] = [
  { value: 0, label: 'Draft' },
  { value: 1, label: 'Pending approval' },
  { value: 2, label: 'Approved' },
  { value: 3, label: 'Active' },
  { value: 4, label: 'Completed' },
  { value: 5, label: 'Cancelled' },
];
export const SALES_CHANNEL_OPTIONS: readonly SelectOption[] = [
  { value: 0, label: 'General trade' },
  { value: 1, label: 'Modern trade' },
  { value: 2, label: 'Business promoter' },
  { value: 3, label: 'All channels' },
];
export const VISIT_TYPE_OPTIONS: readonly SelectOption[] = [
  { value: 0, label: 'Sales' },
  { value: 1, label: 'Merchandising' },
  { value: 2, label: 'Collection' },
  { value: 3, label: 'Support' },
  { value: 4, label: 'Survey' },
];
export const FEEDBACK_TYPE_OPTIONS: readonly SelectOption[] = [
  { value: 0, label: 'Product' },
  { value: 1, label: 'Service' },
  { value: 2, label: 'Campaign' },
  { value: 3, label: 'Complaint' },
  { value: 4, label: 'General' },
];
export const OBSERVATION_TYPE_OPTIONS: readonly SelectOption[] = [
  { value: 0, label: 'Availability' },
  { value: 1, label: 'Display' },
  { value: 2, label: 'Price' },
  { value: 3, label: 'Competitor' },
  { value: 4, label: 'Planogram' },
];
export const optionLabel = (
  options: readonly SelectOption[],
  value: number | null | undefined,
): string => options.find((item) => item.value === value)?.label ?? String(value ?? 'Not set');
