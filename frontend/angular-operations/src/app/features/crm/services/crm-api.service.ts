import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_ENDPOINTS } from '../../../core/config/api-endpoints';
import { ApiClientService } from '../../../core/http/api-client.service';
import type { PagedRequest, PagedResult } from '../../../core/models/paged-result.model';
import type {
  AssignClientSegmentRequest,
  ChangeClientLifecycleRequest,
  ClientDetails,
  ClientListItem,
  ComplaintDetails,
  ComplaintListItem,
  ComplaintStatus,
  ConvertLeadToClientRequest,
  CreateReactivationCaseRequest,
  CrmActivity,
  CrmDashboard,
  CrmTask,
  CrmTaskStatus,
  DuplicateCandidate,
  DuplicateResolutionType,
  DuplicateReview,
  LeadDetails,
  LeadListItem,
  LeadScoreResult,
  LeadStatus,
  OpportunityDetails,
  OpportunityListItem,
  OpportunityStage,
  QuotationDetails,
  QuotationListItem,
  QuotationStatus,
  ReactivationCase,
  ResolveReactivationCaseRequest,
  SaveClientContactRequest,
  SaveClientCreditProfileRequest,
  SaveClientRequest,
  SaveClientSegmentRequest,
  SaveComplaintRequest,
  SaveCrmActivityRequest,
  SaveCrmTaskRequest,
  SaveLeadRequest,
  SaveLeadScoreRuleRequest,
  SaveOpportunityRequest,
  SaveQuotationRequest,
} from '../models/crm.model';

@Injectable({ providedIn: 'root' })
export class CrmApiService {
  private readonly api = inject(ApiClientService);

  getDashboard(): Observable<CrmDashboard> {
    return this.api.get(API_ENDPOINTS.clients.dashboard);
  }
  getClients(request: PagedRequest): Observable<PagedResult<ClientListItem>> {
    return this.api.get(API_ENDPOINTS.clients.root, this.toPagedParams(request));
  }
  getClient(clientID: number): Observable<ClientDetails> {
    return this.api.get(API_ENDPOINTS.clients.byID(clientID));
  }
  createClient(request: SaveClientRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.clients.root, request);
  }
  updateClient(clientID: number, request: SaveClientRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.clients.byID(clientID), request);
  }
  addClientContact(clientID: number, request: SaveClientContactRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.clients.contacts(clientID), request);
  }
  updateClientContact(
    clientContactID: number,
    request: SaveClientContactRequest,
  ): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.clients.contact(clientContactID), request);
  }
  setClientCreditProfile(
    clientID: number,
    request: SaveClientCreditProfileRequest,
  ): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.clients.creditProfile(clientID), request);
  }
  changeClientLifecycle(
    clientID: number,
    request: ChangeClientLifecycleRequest,
  ): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.clients.lifecycle(clientID), request);
  }
  createClientSegment(request: SaveClientSegmentRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.clients.segments, request);
  }
  assignClientSegment(clientID: number, request: AssignClientSegmentRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.clients.assignSegment(clientID), request);
  }
  endClientSegmentAssignment(
    clientSegmentAssignmentID: number,
    effectiveTo: string,
  ): Observable<boolean> {
    return this.api.post(
      `${API_ENDPOINTS.clients.endSegmentAssignment(clientSegmentAssignmentID)}?effectiveTo=${encodeURIComponent(effectiveTo)}`,
      null,
    );
  }

  getLeads(request: PagedRequest): Observable<PagedResult<LeadListItem>> {
    return this.api.get(API_ENDPOINTS.leads.root, this.toPagedParams(request));
  }
  getLead(leadID: number): Observable<LeadDetails> {
    return this.api.get(API_ENDPOINTS.leads.byID(leadID));
  }
  createLead(request: SaveLeadRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.leads.root, request);
  }
  updateLead(leadID: number, request: SaveLeadRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.leads.byID(leadID), request);
  }
  changeLeadStatus(
    leadID: number,
    status: LeadStatus,
    lostReason: string | null,
  ): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.leads.status(leadID), { status, lostReason });
  }
  recalculateLeadScore(leadID: number): Observable<LeadScoreResult> {
    return this.api.post(API_ENDPOINTS.leads.recalculateScore(leadID), null);
  }
  findLeadDuplicates(leadID: number): Observable<readonly DuplicateCandidate[]> {
    return this.api.get(API_ENDPOINTS.leads.duplicates(leadID));
  }
  getDuplicateReviews(): Observable<readonly DuplicateReview[]> {
    return this.api.get(API_ENDPOINTS.leads.duplicateReviews);
  }
  resolveDuplicateReview(
    duplicateReviewCaseID: number,
    resolutionType: DuplicateResolutionType,
    survivorEntityID: number | null,
  ): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.leads.resolveDuplicateReview(duplicateReviewCaseID), {
      resolutionType,
      survivorEntityID,
    });
  }
  createLeadScoreRule(request: SaveLeadScoreRuleRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.leads.scoreRules, request);
  }
  convertLeadToClient(leadID: number, request: ConvertLeadToClientRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.leads.convertToClient(leadID), request);
  }

  getActivities(filters: {
    leadID?: number;
    clientID?: number;
    opportunityID?: number;
  }): Observable<readonly CrmActivity[]> {
    let params = new HttpParams();
    if (filters.leadID) params = params.set('leadID', filters.leadID);
    if (filters.clientID) params = params.set('clientID', filters.clientID);
    if (filters.opportunityID) params = params.set('opportunityID', filters.opportunityID);
    return this.api.get(API_ENDPOINTS.crmActivities.root, params);
  }
  getActivity(activityID: number): Observable<CrmActivity> {
    return this.api.get(API_ENDPOINTS.crmActivities.byID(activityID));
  }
  createActivity(request: SaveCrmActivityRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.crmActivities.root, request);
  }
  updateActivity(activityID: number, request: SaveCrmActivityRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.crmActivities.byID(activityID), request);
  }

  getTasks(
    request: PagedRequest,
    assignedEmployeeID: number | null,
    overdueOnly: boolean,
  ): Observable<PagedResult<CrmTask>> {
    let params = this.toPagedParams(request).set('overdueOnly', overdueOnly);
    if (assignedEmployeeID) params = params.set('assignedEmployeeID', assignedEmployeeID);
    return this.api.get(API_ENDPOINTS.crmTasks.root, params);
  }
  getTask(taskID: number): Observable<CrmTask> {
    return this.api.get(API_ENDPOINTS.crmTasks.byID(taskID));
  }
  createTask(request: SaveCrmTaskRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.crmTasks.root, request);
  }
  updateTask(taskID: number, request: SaveCrmTaskRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.crmTasks.byID(taskID), request);
  }
  changeTaskStatus(taskID: number, status: CrmTaskStatus): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.crmTasks.status(taskID), { status });
  }

  getOpportunities(request: PagedRequest): Observable<PagedResult<OpportunityListItem>> {
    return this.api.get(API_ENDPOINTS.opportunities.root, this.toPagedParams(request));
  }
  getOpportunity(opportunityID: number): Observable<OpportunityDetails> {
    return this.api.get(API_ENDPOINTS.opportunities.byID(opportunityID));
  }
  createOpportunity(request: SaveOpportunityRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.opportunities.root, request);
  }
  updateOpportunity(opportunityID: number, request: SaveOpportunityRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.opportunities.byID(opportunityID), request);
  }
  changeOpportunityStage(
    opportunityID: number,
    stage: OpportunityStage,
    lostReason: string | null,
  ): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.opportunities.stage(opportunityID), { stage, lostReason });
  }

  getQuotations(request: PagedRequest): Observable<PagedResult<QuotationListItem>> {
    return this.api.get(API_ENDPOINTS.quotations.root, this.toPagedParams(request));
  }
  getQuotation(quotationID: number): Observable<QuotationDetails> {
    return this.api.get(API_ENDPOINTS.quotations.byID(quotationID));
  }
  createQuotation(request: SaveQuotationRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.quotations.root, request);
  }
  updateQuotation(quotationID: number, request: SaveQuotationRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.quotations.byID(quotationID), request);
  }
  createQuotationVersion(quotationID: number): Observable<number> {
    return this.api.post(API_ENDPOINTS.quotations.versions(quotationID), null);
  }
  changeQuotationStatus(quotationID: number, status: QuotationStatus): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.quotations.status(quotationID), { status });
  }

  getComplaints(
    request: PagedRequest,
    slaBreachedOnly: boolean,
  ): Observable<PagedResult<ComplaintListItem>> {
    return this.api.get(
      API_ENDPOINTS.complaints.root,
      this.toPagedParams(request).set('slaBreachedOnly', slaBreachedOnly),
    );
  }
  getComplaint(complaintID: number): Observable<ComplaintDetails> {
    return this.api.get(API_ENDPOINTS.complaints.byID(complaintID));
  }
  createComplaint(request: SaveComplaintRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.complaints.root, request);
  }
  updateComplaint(complaintID: number, request: SaveComplaintRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.complaints.byID(complaintID), request);
  }
  changeComplaintStatus(
    complaintID: number,
    status: ComplaintStatus,
    resolutionNote: string | null,
    satisfactionScore: number | null,
  ): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.complaints.status(complaintID), {
      status,
      resolutionNote,
      satisfactionScore,
    });
  }

  getReactivationCases(request: PagedRequest): Observable<PagedResult<ReactivationCase>> {
    return this.api.get(API_ENDPOINTS.reactivation.root, this.toPagedParams(request));
  }
  getReactivationCase(reactivationCaseID: number): Observable<ReactivationCase> {
    return this.api.get(API_ENDPOINTS.reactivation.byID(reactivationCaseID));
  }
  createReactivationCase(request: CreateReactivationCaseRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.reactivation.root, request);
  }
  resolveReactivationCase(
    reactivationCaseID: number,
    request: ResolveReactivationCaseRequest,
  ): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.reactivation.resolve(reactivationCaseID), request);
  }

  private toPagedParams(request: PagedRequest): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', request.pageNumber)
      .set('pageSize', request.pageSize);
    if (request.search?.trim()) params = params.set('search', request.search.trim());
    if (request.sortBy) params = params.set('sortBy', request.sortBy);
    if (request.sortDirection)
      params = params.set('sortDescending', request.sortDirection === 'desc');
    return params;
  }
}
