import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_ENDPOINTS } from '../../../core/config/api-endpoints';
import { ApiClientService } from '../../../core/http/api-client.service';
import type { PagedResult } from '../../../core/models/paged-result.model';
import type {
  CheckInVisitRequest,
  CheckOutVisitRequest,
  SaveBpSellOutRequest,
  SaveFeedbackRequest,
  SaveMarketObservationRequest,
} from '../../marketing/models/marketing.model';
import type {
  FieldActiveVisit,
  FieldAssignedClient,
  FieldVisitListItem,
  FieldWorkspaceSummary,
  NotificationItem,
  SaveModernTradePurchaseOrderRequest,
} from '../models/field-operations.model';

@Injectable({ providedIn: 'root' })
export class FieldOperationsApiService {
  private readonly api = inject(ApiClientService);

  getSummary(): Observable<FieldWorkspaceSummary> {
    return this.api.get(API_ENDPOINTS.fieldWorkspace.summary);
  }
  getAssignedClients(
    pageNumber = 1,
    pageSize = 20,
    search = '',
  ): Observable<PagedResult<FieldAssignedClient>> {
    return this.api.get(
      API_ENDPOINTS.fieldWorkspace.assignedClients,
      this.params(pageNumber, pageSize, search),
    );
  }
  getMyVisits(
    pageNumber = 1,
    pageSize = 20,
    search = '',
  ): Observable<PagedResult<FieldVisitListItem>> {
    return this.api.get(
      API_ENDPOINTS.fieldWorkspace.myVisits,
      this.params(pageNumber, pageSize, search),
    );
  }
  getActiveVisit(): Observable<FieldActiveVisit | null> {
    return this.api.get(API_ENDPOINTS.fieldWorkspace.activeVisit);
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
  createModernTradePurchaseOrder(request: SaveModernTradePurchaseOrderRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.modernTradePurchaseOrders.root, request);
  }
  submitModernTradePurchaseOrder(id: number): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.modernTradePurchaseOrders.submit(id), {});
  }
  createBpSellOut(request: SaveBpSellOutRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.bpSellOut.root, request);
  }
  createFeedback(request: SaveFeedbackRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.feedback.root, request);
  }
  createMarketObservation(request: SaveMarketObservationRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.marketObservations.root, request);
  }
  getNotifications(pageNumber = 1, pageSize = 20): Observable<PagedResult<NotificationItem>> {
    return this.api.get(API_ENDPOINTS.notifications.root, this.params(pageNumber, pageSize));
  }
  markNotificationRead(notificationID: number): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.notifications.markRead(notificationID), {});
  }
  markAllNotificationsRead(): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.notifications.markAllRead, {});
  }

  private params(pageNumber: number, pageSize: number, search = ''): HttpParams {
    let params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', pageSize);
    if (search.trim()) params = params.set('search', search.trim());
    return params;
  }
}
