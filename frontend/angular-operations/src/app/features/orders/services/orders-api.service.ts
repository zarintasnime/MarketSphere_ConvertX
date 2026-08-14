import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import type { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../../../core/config/api-endpoints';
import { ApiClientService } from '../../../core/http/api-client.service';
import type { PagedRequest, PagedResult } from '../../../core/models/paged-result.model';
import type {
  AppliedOffer,
  ApplyOfferRequest,
  ApprovalActionRequest,
  ApprovalRequest,
  ApproveAndReserveOrderRequest,
  ConvertModernTradePurchaseOrderRequest,
  ConvertQuotationToOrderRequest,
  ModernTradePurchaseOrderDetails,
  ModernTradePurchaseOrderListItem,
  OrderDetails,
  OrderListItem,
  OrderStatus,
  SaveModernTradePurchaseOrderRequest,
  SaveRegularOrderRequest,
  VerifyModernTradePurchaseOrderRequest,
} from '../models/orders.model';

@Injectable({ providedIn: 'root' })
export class OrdersApiService {
  private readonly api = inject(ApiClientService);

  getModernTradePurchaseOrders(
    request: PagedRequest,
  ): Observable<PagedResult<ModernTradePurchaseOrderListItem>> {
    return this.api.get(API_ENDPOINTS.modernTradePurchaseOrders.root, this.toPagedParams(request));
  }
  getModernTradePurchaseOrder(id: number): Observable<ModernTradePurchaseOrderDetails> {
    return this.api.get(API_ENDPOINTS.modernTradePurchaseOrders.byID(id));
  }
  createModernTradePurchaseOrder(request: SaveModernTradePurchaseOrderRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.modernTradePurchaseOrders.root, request);
  }
  updateModernTradePurchaseOrder(
    id: number,
    request: SaveModernTradePurchaseOrderRequest,
  ): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.modernTradePurchaseOrders.byID(id), request);
  }
  mapModernTradePurchaseOrderItem(itemID: number, skuID: number): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.modernTradePurchaseOrders.itemMapping(itemID), { skuID });
  }
  submitModernTradePurchaseOrder(id: number): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.modernTradePurchaseOrders.submit(id), {});
  }
  verifyModernTradePurchaseOrder(
    id: number,
    request: VerifyModernTradePurchaseOrderRequest,
  ): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.modernTradePurchaseOrders.verify(id), request);
  }

  getOrders(request: PagedRequest): Observable<PagedResult<OrderListItem>> {
    return this.api.get(API_ENDPOINTS.orders.root, this.toPagedParams(request));
  }
  getOrder(id: number): Observable<OrderDetails> {
    return this.api.get(API_ENDPOINTS.orders.byID(id));
  }
  createRegularOrder(request: SaveRegularOrderRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.orders.regular, request);
  }
  convertQuotation(request: ConvertQuotationToOrderRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.orders.fromQuotation, request);
  }
  convertModernTradePurchaseOrder(
    request: ConvertModernTradePurchaseOrderRequest,
  ): Observable<number> {
    return this.api.post(API_ENDPOINTS.orders.fromModernTradePurchaseOrder, request);
  }
  submitOrder(id: number): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.orders.submit(id), {});
  }
  approveAndReserveOrder(id: number, request: ApproveAndReserveOrderRequest): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.orders.approveAndReserve(id), request);
  }
  changeOrderStatus(id: number, status: OrderStatus): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.orders.status(id), { status });
  }
  getAppliedOffers(orderID: number): Observable<readonly AppliedOffer[]> {
    return this.api.get(API_ENDPOINTS.orders.appliedOffers(orderID));
  }
  applyOffer(request: ApplyOfferRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.orders.applyOffer, request);
  }
  removeAppliedOffer(id: number): Observable<boolean> {
    return this.api.delete(API_ENDPOINTS.orders.removeOffer(id));
  }

  getApprovalQueue(request: PagedRequest): Observable<PagedResult<ApprovalRequest>> {
    return this.api.get(API_ENDPOINTS.approvals.root, this.toPagedParams(request));
  }
  getApproval(id: number): Observable<ApprovalRequest> {
    return this.api.get(API_ENDPOINTS.approvals.byID(id));
  }
  actOnApproval(id: number, request: ApprovalActionRequest): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.approvals.actions(id), request);
  }
  cancelApproval(id: number, note: string | null): Observable<boolean> {
    const params = note ? new HttpParams().set('note', note) : undefined;
    return this.api.post(
      `${API_ENDPOINTS.approvals.cancel(id)}${params ? `?${params.toString()}` : ''}`,
      {},
    );
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
