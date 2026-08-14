import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import type { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../../../core/config/api-endpoints';
import { ApiClientService } from '../../../core/http/api-client.service';
import type { PagedRequest, PagedResult } from '../../../core/models/paged-result.model';
import type {
  CompleteDeliveryRequest,
  CreateDeliveryRequest,
  CreateInvoiceRequest,
  CreatePickListRequest,
  DeliveryDetails,
  DeliveryListItem,
  InvoiceDetails,
  InvoiceListItem,
  InvoiceStatus,
  PickListDetails,
  PickListListItem,
  RecordPickRequest,
} from '../models/fulfilment.model';

@Injectable({ providedIn: 'root' })
export class FulfilmentApiService {
  private readonly api = inject(ApiClientService);

  getInvoices(request: PagedRequest): Observable<PagedResult<InvoiceListItem>> {
    return this.api.get(API_ENDPOINTS.invoices.root, this.toPagedParams(request));
  }
  getInvoice(id: number): Observable<InvoiceDetails> {
    return this.api.get(API_ENDPOINTS.invoices.byID(id));
  }
  createInvoice(request: CreateInvoiceRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.invoices.root, request);
  }
  changeInvoiceStatus(id: number, status: InvoiceStatus): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.invoices.status(id), { status });
  }

  getPickLists(request: PagedRequest): Observable<PagedResult<PickListListItem>> {
    return this.api.get(API_ENDPOINTS.pickLists.root, this.toPagedParams(request));
  }
  getPickList(id: number): Observable<PickListDetails> {
    return this.api.get(API_ENDPOINTS.pickLists.byID(id));
  }
  createPickList(request: CreatePickListRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.pickLists.root, request);
  }
  releasePickList(id: number, releasedByEmployeeID: number): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.pickLists.release(id), { releasedByEmployeeID });
  }
  recordPick(id: number, request: RecordPickRequest): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.pickLists.recordPick(id), request);
  }
  verifyPickList(
    id: number,
    verifiedByEmployeeID: number,
    note: string | null,
  ): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.pickLists.verify(id), { verifiedByEmployeeID, note });
  }

  getDeliveries(request: PagedRequest): Observable<PagedResult<DeliveryListItem>> {
    return this.api.get(API_ENDPOINTS.deliveries.root, this.toPagedParams(request));
  }
  getDelivery(id: number): Observable<DeliveryDetails> {
    return this.api.get(API_ENDPOINTS.deliveries.byID(id));
  }
  createDelivery(request: CreateDeliveryRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.deliveries.root, request);
  }
  dispatchDelivery(id: number, deliveredByEmployeeID: number): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.deliveries.dispatch(id), { deliveredByEmployeeID });
  }
  completeDelivery(id: number, request: CompleteDeliveryRequest): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.deliveries.complete(id), request);
  }

  private toPagedParams(request: PagedRequest): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', request.pageNumber)
      .set('pageSize', request.pageSize);
    if (request.search?.trim()) {
      params = params.set('search', request.search.trim());
    }
    if (request.sortBy) {
      params = params.set('sortBy', request.sortBy);
    }
    if (request.sortDirection) {
      params = params.set('sortDescending', request.sortDirection === 'desc');
    }
    return params;
  }
}
