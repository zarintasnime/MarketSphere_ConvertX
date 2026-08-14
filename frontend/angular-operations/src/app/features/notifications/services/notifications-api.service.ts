import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import type { Observable } from 'rxjs';

import { API_ENDPOINTS } from '../../../core/config/api-endpoints';
import { ApiClientService } from '../../../core/http/api-client.service';
import type { PagedRequest, PagedResult } from '../../../core/models/paged-result.model';
import type {
  CreateNotificationRequest,
  NotificationItem,
  SystemCheckRun,
} from '../models/notifications.model';

@Injectable({ providedIn: 'root' })
export class NotificationsApiService {
  private readonly api = inject(ApiClientService);

  getMine(request: PagedRequest): Observable<PagedResult<NotificationItem>> {
    return this.api.get(API_ENDPOINTS.notifications.root, this.toPagedParams(request));
  }

  create(request: CreateNotificationRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.notifications.root, request);
  }

  markRead(notificationID: number): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.notifications.markRead(notificationID), {});
  }

  markAllRead(): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.notifications.markAllRead, {});
  }

  runSystemChecks(): Observable<SystemCheckRun> {
    return this.api.post(API_ENDPOINTS.systemChecks.run, {});
  }

  private toPagedParams(request: PagedRequest): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', request.pageNumber)
      .set('pageSize', request.pageSize)
      .set('sortDescending', request.sortDirection === 'desc');

    if (request.search?.trim()) {
      params = params.set('search', request.search.trim());
    }

    if (request.sortBy?.trim()) {
      params = params.set('sortBy', request.sortBy.trim());
    }

    return params;
  }
}
