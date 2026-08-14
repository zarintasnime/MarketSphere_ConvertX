export interface SelectOption<T extends number = number> {
  value: T;
  label: string;
}

export enum ClientType {
  Outlet = 1,
  Dealer = 2,
  Distributor = 3,
  ModernTrade = 4,
  BusinessPartner = 5,
}
export enum SalesChannel {
  GeneralTrade = 1,
  ModernTrade = 2,
  BusinessPartner = 3,
  Institutional = 4,
  Online = 5,
}
export enum ClientLifecycleStatus {
  Prospect = 1,
  Active = 2,
  Inactive = 3,
  Churned = 4,
  ReactivationInProgress = 5,
}
export enum ClientRiskStatus {
  Normal = 1,
  Watch = 2,
  HighRisk = 3,
  Blocked = 4,
}
export enum LeadSource {
  Manual = 1,
  Referral = 2,
  Campaign = 3,
  FieldVisit = 4,
  Website = 5,
  Phone = 6,
  Other = 7,
}
export enum LeadTemperature {
  Cold = 1,
  Warm = 2,
  Hot = 3,
}
export enum LeadStatus {
  New = 1,
  Contacted = 2,
  Qualified = 3,
  Interested = 4,
  SampleGiven = 5,
  Negotiation = 6,
  Converted = 7,
  Lost = 8,
}
export enum LeadScoreConditionType {
  Source = 1,
  EstimatedValue = 2,
  HasPhone = 3,
  HasEmail = 4,
  ProductInterest = 5,
  Region = 6,
  NextFollowUp = 7,
}
export enum ComparisonOperator {
  Equals = 1,
  NotEquals = 2,
  GreaterThan = 3,
  GreaterThanOrEqual = 4,
  LessThan = 5,
  LessThanOrEqual = 6,
  Contains = 7,
  IsTrue = 8,
  IsFalse = 9,
}
export enum DuplicateReviewStatus {
  Open = 1,
  UnderReview = 2,
  Resolved = 3,
  Dismissed = 4,
}
export enum DuplicateResolutionType {
  NotDuplicate = 1,
  Linked = 2,
  Merged = 3,
  KeptSeparate = 4,
}
export enum CrmActivityType {
  Call = 1,
  Meeting = 2,
  Email = 3,
  Note = 4,
  Visit = 5,
  Demo = 6,
}
export enum CrmActivityStatus {
  Planned = 1,
  InProgress = 2,
  Completed = 3,
  Cancelled = 4,
  NoShow = 5,
}
export enum ParticipantRole {
  Organizer = 1,
  Attendee = 2,
  DecisionMaker = 3,
  Influencer = 4,
  Observer = 5,
}
export enum AttendanceStatus {
  Invited = 1,
  Accepted = 2,
  Declined = 3,
  Attended = 4,
  Absent = 5,
}
export enum TaskPriority {
  Low = 1,
  Normal = 2,
  High = 3,
  Urgent = 4,
}
export enum CrmTaskStatus {
  Open = 1,
  InProgress = 2,
  Completed = 3,
  Cancelled = 4,
}
export enum OpportunityStage {
  Qualified = 1,
  RequirementAnalysis = 2,
  Proposal = 3,
  Negotiation = 4,
  Commit = 5,
  Won = 6,
  Lost = 7,
  OnHold = 8,
}
export enum QuotationStatus {
  Draft = 1,
  Submitted = 2,
  Reviewed = 3,
  Accepted = 4,
  Rejected = 5,
  Expired = 6,
  Converted = 7,
}
export enum ClientSegmentType {
  Manual = 1,
  Lifecycle = 2,
  Channel = 3,
  Value = 4,
  Risk = 5,
}
export enum ComplaintCategory {
  ProductQuality = 1,
  Delivery = 2,
  Billing = 3,
  Service = 4,
  Pricing = 5,
  Other = 6,
}
export enum ComplaintPriority {
  Low = 1,
  Normal = 2,
  High = 3,
  Critical = 4,
}
export enum ComplaintStatus {
  Open = 1,
  Assigned = 2,
  InProgress = 3,
  WaitingForCustomer = 4,
  Resolved = 5,
  Closed = 6,
  Rejected = 7,
}
export enum ReactivationCaseStatus {
  Open = 1,
  Contacted = 2,
  InProgress = 3,
  Successful = 4,
  Unsuccessful = 5,
  Closed = 6,
}
export enum ReactivationResult {
  Reordered = 1,
  Interested = 2,
  NotInterested = 3,
  Unreachable = 4,
  CompetitorRetained = 5,
  BusinessClosed = 6,
}

export const CLIENT_TYPE_OPTIONS: readonly SelectOption<ClientType>[] = [
  { value: ClientType.Outlet, label: 'Outlet' },
  { value: ClientType.Dealer, label: 'Dealer' },
  { value: ClientType.Distributor, label: 'Distributor' },
  { value: ClientType.ModernTrade, label: 'Modern Trade' },
  { value: ClientType.BusinessPartner, label: 'Business Partner' },
];
export const SALES_CHANNEL_OPTIONS: readonly SelectOption<SalesChannel>[] = [
  { value: SalesChannel.GeneralTrade, label: 'General Trade' },
  { value: SalesChannel.ModernTrade, label: 'Modern Trade' },
  { value: SalesChannel.BusinessPartner, label: 'Business Partner' },
  { value: SalesChannel.Institutional, label: 'Institutional' },
  { value: SalesChannel.Online, label: 'Online' },
];
export const CLIENT_LIFECYCLE_OPTIONS: readonly SelectOption<ClientLifecycleStatus>[] = [
  { value: ClientLifecycleStatus.Prospect, label: 'Prospect' },
  { value: ClientLifecycleStatus.Active, label: 'Active' },
  { value: ClientLifecycleStatus.Inactive, label: 'Inactive' },
  { value: ClientLifecycleStatus.Churned, label: 'Churned' },
  { value: ClientLifecycleStatus.ReactivationInProgress, label: 'Reactivation In Progress' },
];
export const CLIENT_RISK_OPTIONS: readonly SelectOption<ClientRiskStatus>[] = [
  { value: ClientRiskStatus.Normal, label: 'Normal' },
  { value: ClientRiskStatus.Watch, label: 'Watch' },
  { value: ClientRiskStatus.HighRisk, label: 'High Risk' },
  { value: ClientRiskStatus.Blocked, label: 'Blocked' },
];
export const LEAD_SOURCE_OPTIONS: readonly SelectOption<LeadSource>[] = [
  { value: LeadSource.Manual, label: 'Manual' },
  { value: LeadSource.Referral, label: 'Referral' },
  { value: LeadSource.Campaign, label: 'Campaign' },
  { value: LeadSource.FieldVisit, label: 'Field Visit' },
  { value: LeadSource.Website, label: 'Website' },
  { value: LeadSource.Phone, label: 'Phone' },
  { value: LeadSource.Other, label: 'Other' },
];
export const LEAD_STATUS_OPTIONS: readonly SelectOption<LeadStatus>[] = [
  { value: LeadStatus.New, label: 'New' },
  { value: LeadStatus.Contacted, label: 'Contacted' },
  { value: LeadStatus.Qualified, label: 'Qualified' },
  { value: LeadStatus.Interested, label: 'Interested' },
  { value: LeadStatus.SampleGiven, label: 'Sample Given' },
  { value: LeadStatus.Negotiation, label: 'Negotiation' },
  { value: LeadStatus.Converted, label: 'Converted' },
  { value: LeadStatus.Lost, label: 'Lost' },
];
export const LEAD_TEMPERATURE_OPTIONS: readonly SelectOption<LeadTemperature>[] = [
  { value: LeadTemperature.Cold, label: 'Cold' },
  { value: LeadTemperature.Warm, label: 'Warm' },
  { value: LeadTemperature.Hot, label: 'Hot' },
];
export const ACTIVITY_TYPE_OPTIONS: readonly SelectOption<CrmActivityType>[] = [
  { value: CrmActivityType.Call, label: 'Call' },
  { value: CrmActivityType.Meeting, label: 'Meeting' },
  { value: CrmActivityType.Email, label: 'Email' },
  { value: CrmActivityType.Note, label: 'Note' },
  { value: CrmActivityType.Visit, label: 'Visit' },
  { value: CrmActivityType.Demo, label: 'Demo' },
];
export const ACTIVITY_STATUS_OPTIONS: readonly SelectOption<CrmActivityStatus>[] = [
  { value: CrmActivityStatus.Planned, label: 'Planned' },
  { value: CrmActivityStatus.InProgress, label: 'In Progress' },
  { value: CrmActivityStatus.Completed, label: 'Completed' },
  { value: CrmActivityStatus.Cancelled, label: 'Cancelled' },
  { value: CrmActivityStatus.NoShow, label: 'No Show' },
];
export const TASK_PRIORITY_OPTIONS: readonly SelectOption<TaskPriority>[] = [
  { value: TaskPriority.Low, label: 'Low' },
  { value: TaskPriority.Normal, label: 'Normal' },
  { value: TaskPriority.High, label: 'High' },
  { value: TaskPriority.Urgent, label: 'Urgent' },
];
export const TASK_STATUS_OPTIONS: readonly SelectOption<CrmTaskStatus>[] = [
  { value: CrmTaskStatus.Open, label: 'Open' },
  { value: CrmTaskStatus.InProgress, label: 'In Progress' },
  { value: CrmTaskStatus.Completed, label: 'Completed' },
  { value: CrmTaskStatus.Cancelled, label: 'Cancelled' },
];
export const OPPORTUNITY_STAGE_OPTIONS: readonly SelectOption<OpportunityStage>[] = [
  { value: OpportunityStage.Qualified, label: 'Qualified' },
  { value: OpportunityStage.RequirementAnalysis, label: 'Requirement Analysis' },
  { value: OpportunityStage.Proposal, label: 'Proposal' },
  { value: OpportunityStage.Negotiation, label: 'Negotiation' },
  { value: OpportunityStage.Commit, label: 'Commit' },
  { value: OpportunityStage.Won, label: 'Won' },
  { value: OpportunityStage.Lost, label: 'Lost' },
  { value: OpportunityStage.OnHold, label: 'On Hold' },
];
export const QUOTATION_STATUS_OPTIONS: readonly SelectOption<QuotationStatus>[] = [
  { value: QuotationStatus.Draft, label: 'Draft' },
  { value: QuotationStatus.Submitted, label: 'Submitted' },
  { value: QuotationStatus.Reviewed, label: 'Reviewed' },
  { value: QuotationStatus.Accepted, label: 'Accepted' },
  { value: QuotationStatus.Rejected, label: 'Rejected' },
  { value: QuotationStatus.Expired, label: 'Expired' },
  { value: QuotationStatus.Converted, label: 'Converted' },
];
export const COMPLAINT_CATEGORY_OPTIONS: readonly SelectOption<ComplaintCategory>[] = [
  { value: ComplaintCategory.ProductQuality, label: 'Product Quality' },
  { value: ComplaintCategory.Delivery, label: 'Delivery' },
  { value: ComplaintCategory.Billing, label: 'Billing' },
  { value: ComplaintCategory.Service, label: 'Service' },
  { value: ComplaintCategory.Pricing, label: 'Pricing' },
  { value: ComplaintCategory.Other, label: 'Other' },
];
export const COMPLAINT_PRIORITY_OPTIONS: readonly SelectOption<ComplaintPriority>[] = [
  { value: ComplaintPriority.Low, label: 'Low' },
  { value: ComplaintPriority.Normal, label: 'Normal' },
  { value: ComplaintPriority.High, label: 'High' },
  { value: ComplaintPriority.Critical, label: 'Critical' },
];
export const COMPLAINT_STATUS_OPTIONS: readonly SelectOption<ComplaintStatus>[] = [
  { value: ComplaintStatus.Open, label: 'Open' },
  { value: ComplaintStatus.Assigned, label: 'Assigned' },
  { value: ComplaintStatus.InProgress, label: 'In Progress' },
  { value: ComplaintStatus.WaitingForCustomer, label: 'Waiting for Customer' },
  { value: ComplaintStatus.Resolved, label: 'Resolved' },
  { value: ComplaintStatus.Closed, label: 'Closed' },
  { value: ComplaintStatus.Rejected, label: 'Rejected' },
];
export const REACTIVATION_STATUS_OPTIONS: readonly SelectOption<ReactivationCaseStatus>[] = [
  { value: ReactivationCaseStatus.Open, label: 'Open' },
  { value: ReactivationCaseStatus.Contacted, label: 'Contacted' },
  { value: ReactivationCaseStatus.InProgress, label: 'In Progress' },
  { value: ReactivationCaseStatus.Successful, label: 'Successful' },
  { value: ReactivationCaseStatus.Unsuccessful, label: 'Unsuccessful' },
  { value: ReactivationCaseStatus.Closed, label: 'Closed' },
];
export const REACTIVATION_RESULT_OPTIONS: readonly SelectOption<ReactivationResult>[] = [
  { value: ReactivationResult.Reordered, label: 'Reordered' },
  { value: ReactivationResult.Interested, label: 'Interested' },
  { value: ReactivationResult.NotInterested, label: 'Not Interested' },
  { value: ReactivationResult.Unreachable, label: 'Unreachable' },
  { value: ReactivationResult.CompetitorRetained, label: 'Competitor Retained' },
  { value: ReactivationResult.BusinessClosed, label: 'Business Closed' },
];

export function optionLabel<T extends number>(
  options: readonly SelectOption<T>[],
  value: T,
): string {
  return options.find((option) => option.value === value)?.label ?? `Value ${value}`;
}

export interface CrmDashboardSummary {
  activeClients: number;
  openLeads: number;
  hotLeads: number;
  openTasks: number;
  overdueTasks: number;
  openOpportunities: number;
  opportunityPipelineValue: number;
  activeQuotations: number;
  activeQuotationValue: number;
  openComplaints: number;
  slaBreachedComplaints: number;
  openReactivationCases: number;
}
export interface FunnelStage {
  stage: string;
  count: number;
  value: number;
}
export interface CrmDashboard {
  summary: CrmDashboardSummary;
  leadFunnel: readonly FunnelStage[];
  opportunityFunnel: readonly FunnelStage[];
}

export interface ClientListItem {
  clientID: number;
  clientCode: string;
  clientName: string;
  clientType: ClientType;
  channel: SalesChannel;
  phone: string | null;
  lifecycleStatus: ClientLifecycleStatus;
  riskStatus: ClientRiskStatus;
  isActive: boolean;
}
export interface ClientContact {
  clientContactID: number;
  contactName: string;
  designation: string | null;
  phone: string | null;
  email: string | null;
  isPrimary: boolean;
  isActive: boolean;
}
export interface ClientCreditProfile {
  clientCreditProfileID: number;
  creditLimit: number;
  creditDays: number;
  currentDue: number;
  isBlocked: boolean;
  blockReason: string | null;
  lastReviewedAt: string | null;
}
export interface ClientSegmentAssignment {
  clientSegmentAssignmentID: number;
  clientSegmentID: number;
  segmentCode: string;
  segmentName: string;
  assignedAt: string;
  effectiveTo: string | null;
}
export interface ClientDetails extends ClientListItem {
  email: string | null;
  address: string;
  gpsLat: number | null;
  gpsLng: number | null;
  regionID: number | null;
  areaID: number | null;
  territoryID: number | null;
  lastOrderAt: string | null;
  contacts: readonly ClientContact[];
  creditProfile: ClientCreditProfile | null;
  segments: readonly ClientSegmentAssignment[];
}
export interface SaveClientRequest {
  clientCode: string;
  clientName: string;
  clientType: ClientType;
  channel: SalesChannel;
  phone: string | null;
  email: string | null;
  address: string;
  gpsLat: number | null;
  gpsLng: number | null;
  regionID: number | null;
  areaID: number | null;
  territoryID: number | null;
  lifecycleStatus: ClientLifecycleStatus;
  riskStatus: ClientRiskStatus;
  isActive: boolean;
}
export interface SaveClientContactRequest {
  contactName: string;
  designation: string | null;
  phone: string | null;
  email: string | null;
  isPrimary: boolean;
  isActive: boolean;
}
export interface SaveClientCreditProfileRequest {
  creditLimit: number;
  creditDays: number;
  currentDue: number;
  isBlocked: boolean;
  blockReason: string | null;
}
export interface ChangeClientLifecycleRequest {
  lifecycleStatus: ClientLifecycleStatus;
  reason: string | null;
}
export interface AssignClientSegmentRequest {
  clientSegmentID: number;
  effectiveTo: string | null;
}
export interface SaveClientSegmentRequest {
  segmentCode: string;
  segmentName: string;
  segmentType: ClientSegmentType;
  description: string | null;
  isSystemSegment: boolean;
  isActive: boolean;
}

export interface LeadListItem {
  leadID: number;
  leadCode: string;
  leadName: string;
  businessName: string | null;
  phone: string | null;
  source: LeadSource;
  currentScore: number;
  temperature: LeadTemperature;
  status: LeadStatus;
  nextFollowUpAt: string | null;
  assignedEmployeeID: number | null;
}
export interface LeadDetails extends LeadListItem {
  email: string | null;
  sourceCampaignID: number | null;
  regionID: number | null;
  productInterest: string | null;
  estimatedValue: number | null;
  lostReason: string | null;
  reactivationAt: string | null;
  convertedClientID: number | null;
}
export interface SaveLeadRequest {
  leadCode: string;
  leadName: string;
  businessName: string | null;
  phone: string | null;
  email: string | null;
  source: LeadSource;
  sourceCampaignID: number | null;
  assignedEmployeeID: number | null;
  regionID: number | null;
  productInterest: string | null;
  estimatedValue: number | null;
  nextFollowUpAt: string | null;
}
export interface LeadScoreResult {
  leadID: number;
  previousScore: number;
  currentScore: number;
  temperature: LeadTemperature;
  appliedRules: readonly string[];
}
export interface DuplicateCandidate {
  entityType: string;
  entityID: number;
  displayName: string;
  matchScore: number;
  reasons: readonly string[];
}
export interface DuplicateReview {
  duplicateReviewCaseID: number;
  sourceEntityType: string;
  sourceEntityID: number;
  matchedEntityType: string;
  matchedEntityID: number;
  matchScore: number | null;
  matchReasonsJson: string | null;
  status: DuplicateReviewStatus;
  resolutionType: DuplicateResolutionType | null;
  survivorEntityID: number | null;
  resolvedAt: string | null;
}
export interface SaveLeadScoreRuleRequest {
  ruleName: string;
  conditionType: LeadScoreConditionType;
  operator: ComparisonOperator;
  comparisonValue: string | null;
  scoreValue: number;
  effectiveFrom: string;
  effectiveTo: string | null;
  isActive: boolean;
}
export interface ConvertLeadToClientRequest {
  clientCode: string;
  clientType: ClientType;
  channel: SalesChannel;
  address: string;
}

export interface CrmActivityParticipant {
  crmActivityParticipantID: number;
  employeeID: number | null;
  clientContactID: number | null;
  externalName: string | null;
  externalEmail: string | null;
  participantRole: ParticipantRole | null;
  attendanceStatus: AttendanceStatus | null;
}
export interface CrmActivity {
  crmActivityID: number;
  leadID: number | null;
  clientID: number | null;
  opportunityID: number | null;
  activityType: CrmActivityType;
  subject: string;
  details: string | null;
  activityAt: string;
  scheduledStartAt: string | null;
  scheduledEndAt: string | null;
  locationOrMeetingLink: string | null;
  agenda: string | null;
  activityStatus: CrmActivityStatus;
  outcome: string | null;
  nextActionAt: string | null;
  performedByEmployeeID: number | null;
  participants: readonly CrmActivityParticipant[];
}
export interface SaveCrmActivityRequest {
  leadID: number | null;
  clientID: number | null;
  opportunityID: number | null;
  activityType: CrmActivityType;
  subject: string;
  details: string | null;
  activityAt: string;
  scheduledStartAt: string | null;
  scheduledEndAt: string | null;
  locationOrMeetingLink: string | null;
  agenda: string | null;
  activityStatus: CrmActivityStatus;
  outcome: string | null;
  nextActionAt: string | null;
  performedByEmployeeID: number | null;
  participants: readonly CrmActivityParticipant[];
}

export interface CrmTask {
  crmTaskID: number;
  leadID: number | null;
  clientID: number | null;
  opportunityID: number | null;
  complaintID: number | null;
  reactivationCaseID: number | null;
  assignedEmployeeID: number;
  title: string;
  description: string | null;
  priority: TaskPriority;
  dueAt: string;
  reminderAt: string | null;
  recurrenceRule: string | null;
  status: CrmTaskStatus;
  completedAt: string | null;
  escalatedAt: string | null;
}
export interface SaveCrmTaskRequest {
  leadID: number | null;
  clientID: number | null;
  opportunityID: number | null;
  complaintID: number | null;
  reactivationCaseID: number | null;
  assignedEmployeeID: number;
  title: string;
  description: string | null;
  priority: TaskPriority;
  dueAt: string;
  reminderAt: string | null;
  recurrenceRule: string | null;
}

export interface OpportunityListItem {
  opportunityID: number;
  opportunityCode: string;
  opportunityName: string;
  leadID: number | null;
  clientID: number | null;
  ownerEmployeeID: number;
  stage: OpportunityStage;
  expectedValue: number;
  probabilityPercent: number;
  expectedCloseDate: string | null;
}
export interface OpportunityDetails extends OpportunityListItem {
  campaignID: number | null;
  competitor: string | null;
  lostReason: string | null;
  wonAt: string | null;
}
export interface SaveOpportunityRequest {
  opportunityCode: string;
  leadID: number | null;
  clientID: number | null;
  campaignID: number | null;
  ownerEmployeeID: number;
  opportunityName: string;
  expectedValue: number;
  probabilityPercent: number;
  expectedCloseDate: string | null;
  competitor: string | null;
}

export interface QuotationItem {
  quotationItemID: number;
  skuID: number;
  quantity: number;
  unitPrice: number;
  discountPercent: number;
  discountAmount: number;
  taxAmount: number;
  lineTotal: number;
  note: string | null;
}
export interface QuotationListItem {
  quotationID: number;
  quotationNo: string;
  versionNo: number;
  clientID: number;
  opportunityID: number | null;
  validFrom: string;
  validUntil: string;
  status: QuotationStatus;
  netAmount: number;
}
export interface QuotationDetails extends QuotationListItem {
  rootQuotationID: number | null;
  campaignID: number | null;
  priceListID: number | null;
  grossAmount: number;
  discountAmount: number;
  taxAmount: number;
  terms: string | null;
  acceptedAt: string | null;
  items: readonly QuotationItem[];
}
export interface SaveQuotationItemRequest {
  skuID: number;
  quantity: number;
  unitPrice: number;
  discountPercent: number;
  taxAmount: number;
  note: string | null;
}
export interface SaveQuotationRequest {
  quotationNo: string;
  opportunityID: number | null;
  clientID: number;
  campaignID: number | null;
  priceListID: number | null;
  validFrom: string;
  validUntil: string;
  terms: string | null;
  items: readonly SaveQuotationItemRequest[];
}

export interface ComplaintListItem {
  complaintID: number;
  complaintNo: string;
  clientID: number;
  complaintCategory: ComplaintCategory;
  priority: ComplaintPriority;
  subject: string;
  status: ComplaintStatus;
  openedAt: string;
  slaDueAt: string | null;
  assignedEmployeeID: number | null;
}
export interface ComplaintDetails extends ComplaintListItem {
  orderID: number | null;
  invoiceID: number | null;
  deliveryID: number | null;
  details: string;
  resolvedAt: string | null;
  resolutionNote: string | null;
  satisfactionScore: number | null;
}
export interface SaveComplaintRequest {
  complaintNo: string;
  clientID: number;
  orderID: number | null;
  invoiceID: number | null;
  deliveryID: number | null;
  complaintCategory: ComplaintCategory;
  priority: ComplaintPriority;
  subject: string;
  details: string;
  assignedEmployeeID: number | null;
  slaDueAt: string | null;
}

export interface ReactivationCase {
  reactivationCaseID: number;
  clientID: number;
  inactiveAt: string;
  churnReason: string | null;
  campaignID: number | null;
  assignedEmployeeID: number;
  openedAt: string;
  status: ReactivationCaseStatus;
  reactivationResult: ReactivationResult | null;
  reactivatedAt: string | null;
  repeatOrderID: number | null;
}
export interface CreateReactivationCaseRequest {
  clientID: number;
  inactiveAt: string;
  churnReason: string | null;
  campaignID: number | null;
  assignedEmployeeID: number;
}
export interface ResolveReactivationCaseRequest {
  status: ReactivationCaseStatus;
  reactivationResult: ReactivationResult | null;
  repeatOrderID: number | null;
}
