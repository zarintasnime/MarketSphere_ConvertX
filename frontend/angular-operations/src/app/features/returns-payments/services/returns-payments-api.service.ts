import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import type { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../../../core/config/api-endpoints';
import { ApiClientService } from '../../../core/http/api-client.service';
import type { PagedRequest, PagedResult } from '../../../core/models/paged-result.model';
import type {
  ApproveReturnRequest,
  ConfirmPaymentRequest,
  CreatePaymentRequest,
  CreateReturnRequest,
  PaymentDetails,
  PaymentListItem,
  ResolveReturnRequest,
  ReturnDetails,
  ReturnListItem,
} from '../models/returns-payments.model';

@Injectable({ providedIn: 'root' })
export class ReturnsPaymentsApiService {
  private readonly api = inject(ApiClientService);

  getReturns(request: PagedRequest): Observable<PagedResult<ReturnListItem>> {
    return this.api.get(API_ENDPOINTS.returns.root, this.toPagedParams(request));
  }
  getReturn(id: number): Observable<ReturnDetails> {
    return this.api.get(API_ENDPOINTS.returns.byID(id));
  }
  createReturn(request: CreateReturnRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.returns.root, request);
  }
  approveReturn(id: number, request: ApproveReturnRequest): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.returns.approve(id), request);
  }
  resolveReturn(id: number, request: ResolveReturnRequest): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.returns.resolve(id), request);
  }

  getPayments(request: PagedRequest): Observable<PagedResult<PaymentListItem>> {
    return this.api.get(API_ENDPOINTS.payments.root, this.toPagedParams(request));
  }
  getPayment(id: number): Observable<PaymentDetails> {
    return this.api.get(API_ENDPOINTS.payments.byID(id));
  }
  createPayment(request: CreatePaymentRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.payments.root, request);
  }
  confirmPayment(id: number, request: ConfirmPaymentRequest): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.payments.confirm(id), request);
  }
  reverseAllocation(paymentAllocationID: number): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.payments.reverseAllocation, { paymentAllocationID });
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
