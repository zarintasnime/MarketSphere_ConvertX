import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_ENDPOINTS } from '../../../core/config/api-endpoints';
import { ApiClientService } from '../../../core/http/api-client.service';
import type { PagedResult } from '../../../core/models/paged-result.model';
import type {
  BpSellOutListItem,
  CampaignDetails,
  CampaignListItem,
  CampaignRoi,
  CheckInVisitRequest,
  CheckOutVisitRequest,
  FeedbackListItem,
  MarketObservationListItem,
  PagedRequest,
  SamplingLogListItem,
  SaveBpSellOutRequest,
  SaveCampaignRequest,
  SaveFeedbackRequest,
  SaveMarketObservationRequest,
  SaveSamplingLogRequest,
  VisitDetails,
  VisitListItem,
} from '../models/marketing.model';

@Injectable({ providedIn: 'root' })
export class MarketingApiService {
  private readonly api = inject(ApiClientService);

  getCampaigns(request: PagedRequest): Observable<PagedResult<CampaignListItem>> {
    return this.api.get(API_ENDPOINTS.campaigns.root, this.toPagedParams(request));
  }
  getCampaign(campaignID: number): Observable<CampaignDetails> {
    return this.api.get(API_ENDPOINTS.campaigns.byID(campaignID));
  }
  createCampaign(request: SaveCampaignRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.campaigns.root, request);
  }
  updateCampaign(campaignID: number, request: SaveCampaignRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.campaigns.byID(campaignID), request);
  }
  changeCampaignStatus(
    campaignID: number,
    status: number,
    reason: string | null,
  ): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.campaigns.status(campaignID), { status, reason });
  }
  addCampaignTarget(campaignID: number, request: unknown): Observable<number> {
    return this.api.post(API_ENDPOINTS.campaigns.targets(campaignID), request);
  }
  updateCampaignTarget(targetID: number, request: unknown): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.campaigns.target(targetID), request);
  }
  deleteCampaignTarget(targetID: number): Observable<boolean> {
    return this.api.delete(API_ENDPOINTS.campaigns.target(targetID));
  }
  addCampaignOffer(campaignID: number, request: unknown): Observable<number> {
    return this.api.post(API_ENDPOINTS.campaigns.offers(campaignID), request);
  }
  updateCampaignOffer(offerID: number, request: unknown): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.campaigns.offer(offerID), request);
  }
  deleteCampaignOffer(offerID: number): Observable<boolean> {
    return this.api.delete(API_ENDPOINTS.campaigns.offer(offerID));
  }
  addCampaignExpense(campaignID: number, request: unknown): Observable<number> {
    return this.api.post(API_ENDPOINTS.campaigns.expenses(campaignID), request);
  }
  updateCampaignExpense(expenseID: number, request: unknown): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.campaigns.expense(expenseID), request);
  }
  changeCampaignExpenseStatus(expenseID: number, status: number): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.campaigns.expenseStatus(expenseID), { status });
  }
  addCampaignAttribution(campaignID: number, request: unknown): Observable<number> {
    return this.api.post(API_ENDPOINTS.campaigns.attributions(campaignID), request);
  }
  getCampaignRoi(campaignID: number): Observable<CampaignRoi> {
    return this.api.get(API_ENDPOINTS.campaigns.roi(campaignID));
  }

  getVisits(request: PagedRequest): Observable<PagedResult<VisitListItem>> {
    return this.api.get(API_ENDPOINTS.visits.root, this.toPagedParams(request));
  }
  getVisit(visitID: number): Observable<VisitDetails> {
    return this.api.get(API_ENDPOINTS.visits.byID(visitID));
  }
  checkIn(request: CheckInVisitRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.visits.checkIn, request);
  }
  checkOut(visitID: number, request: CheckOutVisitRequest): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.visits.checkOut(visitID), request);
  }
  cancelVisit(visitID: number, reason: string): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.visits.cancel(visitID), { reason });
  }

  getSamplingLogs(request: PagedRequest): Observable<PagedResult<SamplingLogListItem>> {
    return this.api.get(API_ENDPOINTS.sampling.root, this.toPagedParams(request));
  }
  createSamplingLog(request: SaveSamplingLogRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.sampling.root, request);
  }
  updateSamplingLog(id: number, request: SaveSamplingLogRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.sampling.byID(id), request);
  }
  deleteSamplingLog(id: number): Observable<boolean> {
    return this.api.delete(API_ENDPOINTS.sampling.byID(id));
  }

  getFeedback(request: PagedRequest): Observable<PagedResult<FeedbackListItem>> {
    return this.api.get(API_ENDPOINTS.feedback.root, this.toPagedParams(request));
  }
  createFeedback(request: SaveFeedbackRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.feedback.root, request);
  }
  updateFeedback(id: number, request: SaveFeedbackRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.feedback.byID(id), request);
  }
  deleteFeedback(id: number): Observable<boolean> {
    return this.api.delete(API_ENDPOINTS.feedback.byID(id));
  }

  getMarketObservations(request: PagedRequest): Observable<PagedResult<MarketObservationListItem>> {
    return this.api.get(API_ENDPOINTS.marketObservations.root, this.toPagedParams(request));
  }
  createMarketObservation(request: SaveMarketObservationRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.marketObservations.root, request);
  }
  updateMarketObservation(id: number, request: SaveMarketObservationRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.marketObservations.byID(id), request);
  }
  deleteMarketObservation(id: number): Observable<boolean> {
    return this.api.delete(API_ENDPOINTS.marketObservations.byID(id));
  }

  getBpSellOut(request: PagedRequest): Observable<PagedResult<BpSellOutListItem>> {
    return this.api.get(API_ENDPOINTS.bpSellOut.root, this.toPagedParams(request));
  }
  createBpSellOut(request: SaveBpSellOutRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.bpSellOut.root, request);
  }
  updateBpSellOut(id: number, request: SaveBpSellOutRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.bpSellOut.byID(id), request);
  }
  verifyBpSellOut(
    id: number,
    verifiedByEmployeeID: number,
    verificationStatus: number,
    note: string | null,
  ): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.bpSellOut.verify(id), {
      verifiedByEmployeeID,
      verificationStatus,
      note,
    });
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
