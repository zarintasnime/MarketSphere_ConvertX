import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_ENDPOINTS } from '../../../core/config/api-endpoints';
import { ApiClientService } from '../../../core/http/api-client.service';
import type { PagedRequest, PagedResult } from '../../../core/models/paged-result.model';
import type {
  Batch,
  DispatchStockTransferRequest,
  ReceiveStockTransferRequest,
  SaveStockAdjustmentRequest,
  SaveStockTransferRequest,
  SaveWarehouseRequest,
  StockAdjustmentDetails,
  StockAdjustmentListItem,
  StockAdjustmentStatus,
  StockBalance,
  StockMovement,
  StockReservation,
  StockSearchRequest,
  StockTransferDetails,
  StockTransferListItem,
  Warehouse,
} from '../models/inventory.model';

@Injectable({ providedIn: 'root' })
export class InventoryApiService {
  private readonly api = inject(ApiClientService);

  getWarehouses(): Observable<readonly Warehouse[]> {
    return this.api.get(API_ENDPOINTS.warehouses.root);
  }
  createWarehouse(request: SaveWarehouseRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.warehouses.root, request);
  }
  updateWarehouse(id: number, request: SaveWarehouseRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.warehouses.byID(id), request);
  }
  changeWarehouseStatus(id: number, isActive: boolean): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.warehouses.status(id), { isActive });
  }

  getStockBalances(request: StockSearchRequest): Observable<readonly StockBalance[]> {
    return this.api.get(API_ENDPOINTS.stock.balances, this.stockSearchParams(request));
  }
  getBatches(skuID: number | null, includeExpired: boolean): Observable<readonly Batch[]> {
    let params = new HttpParams().set('includeExpired', includeExpired);
    if (skuID) {
      params = params.set('skuID', skuID);
    }
    return this.api.get(API_ENDPOINTS.stock.batches, params);
  }
  getMovements(
    warehouseID: number | null,
    skuID: number | null,
    from: string | null,
    to: string | null,
  ): Observable<readonly StockMovement[]> {
    let params = new HttpParams();
    if (warehouseID) {
      params = params.set('warehouseID', warehouseID);
    }
    if (skuID) {
      params = params.set('skuID', skuID);
    }
    if (from) {
      params = params.set('from', from);
    }
    if (to) {
      params = params.set('to', to);
    }
    return this.api.get(API_ENDPOINTS.stock.movements, params);
  }
  getReservations(orderItemID: number | null): Observable<readonly StockReservation[]> {
    const params = orderItemID ? new HttpParams().set('orderItemID', orderItemID) : undefined;
    return this.api.get(API_ENDPOINTS.stock.reservations, params);
  }

  getStockTransfers(request: PagedRequest): Observable<PagedResult<StockTransferListItem>> {
    return this.api.get(API_ENDPOINTS.stockTransfers.root, this.toPagedParams(request));
  }
  getStockTransfer(id: number): Observable<StockTransferDetails> {
    return this.api.get(API_ENDPOINTS.stockTransfers.byID(id));
  }
  createStockTransfer(request: SaveStockTransferRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.stockTransfers.root, request);
  }
  updateStockTransfer(id: number, request: SaveStockTransferRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.stockTransfers.byID(id), request);
  }
  submitStockTransfer(id: number): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.stockTransfers.submit(id), {});
  }
  approveStockTransfer(id: number): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.stockTransfers.approve(id), {});
  }
  dispatchStockTransfer(id: number, request: DispatchStockTransferRequest): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.stockTransfers.dispatch(id), request);
  }
  receiveStockTransfer(id: number, request: ReceiveStockTransferRequest): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.stockTransfers.receive(id), request);
  }

  getStockAdjustments(request: PagedRequest): Observable<PagedResult<StockAdjustmentListItem>> {
    return this.api.get(API_ENDPOINTS.stockAdjustments.root, this.toPagedParams(request));
  }
  getStockAdjustment(id: number): Observable<StockAdjustmentDetails> {
    return this.api.get(API_ENDPOINTS.stockAdjustments.byID(id));
  }
  createStockAdjustment(request: SaveStockAdjustmentRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.stockAdjustments.root, request);
  }
  updateStockAdjustment(id: number, request: SaveStockAdjustmentRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.stockAdjustments.byID(id), request);
  }
  changeStockAdjustmentStatus(id: number, status: StockAdjustmentStatus): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.stockAdjustments.status(id), { status });
  }
  postStockAdjustment(id: number, note: string | null): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.stockAdjustments.post(id), { note });
  }

  private stockSearchParams(request: StockSearchRequest): HttpParams {
    let params = new HttpParams()
      .set('includeZero', request.includeZero)
      .set('includeExpired', request.includeExpired);
    if (request.warehouseID) {
      params = params.set('warehouseID', request.warehouseID);
    }
    if (request.skuID) {
      params = params.set('skuID', request.skuID);
    }
    if (request.batchID) {
      params = params.set('batchID', request.batchID);
    }
    return params;
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
