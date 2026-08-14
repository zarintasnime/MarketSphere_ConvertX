import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_ENDPOINTS } from '../../../core/config/api-endpoints';
import { ApiClientService } from '../../../core/http/api-client.service';
import type { PagedRequest, PagedResult } from '../../../core/models/paged-result.model';
import type {
  CreateSupplierPaymentRequest,
  GoodsReceiptDetails,
  GoodsReceiptListItem,
  PaymentMethod,
  PurchaseInvoice,
  PurchaseOrderDetails,
  PurchaseOrderListItem,
  PurchaseOrderStatus,
  PurchaseRequisitionDetails,
  PurchaseRequisitionListItem,
  PurchaseRequisitionStatus,
  QualityCheckStatus,
  SaveGoodsReceiptRequest,
  SavePurchaseInvoiceRequest,
  SavePurchaseOrderRequest,
  SavePurchaseRequisitionRequest,
  SaveSupplierProductRequest,
  SaveSupplierRequest,
  SaveSupplierReturnRequest,
  SupplierDetails,
  SupplierListItem,
  SupplierPayment,
  SupplierPaymentStatus,
  SupplierReturnDetails,
  SupplierReturnListItem,
  SupplierReturnStatus,
  SupplierStatus,
} from '../models/procurement.model';

@Injectable({ providedIn: 'root' })
export class ProcurementApiService {
  private readonly api = inject(ApiClientService);

  getSuppliers(request: PagedRequest): Observable<PagedResult<SupplierListItem>> {
    return this.api.get(API_ENDPOINTS.suppliers.root, this.toPagedParams(request));
  }
  getSupplier(supplierID: number): Observable<SupplierDetails> {
    return this.api.get(API_ENDPOINTS.suppliers.byID(supplierID));
  }
  createSupplier(request: SaveSupplierRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.suppliers.root, request);
  }
  updateSupplier(supplierID: number, request: SaveSupplierRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.suppliers.byID(supplierID), request);
  }
  changeSupplierStatus(supplierID: number, status: SupplierStatus): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.suppliers.status(supplierID), { status });
  }
  saveSupplierProduct(supplierID: number, request: SaveSupplierProductRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.suppliers.products(supplierID), request);
  }

  getPurchaseRequisitions(
    request: PagedRequest,
  ): Observable<PagedResult<PurchaseRequisitionListItem>> {
    return this.api.get(API_ENDPOINTS.purchaseRequisitions.root, this.toPagedParams(request));
  }
  getPurchaseRequisition(id: number): Observable<PurchaseRequisitionDetails> {
    return this.api.get(API_ENDPOINTS.purchaseRequisitions.byID(id));
  }
  createPurchaseRequisition(request: SavePurchaseRequisitionRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.purchaseRequisitions.root, request);
  }
  updatePurchaseRequisition(
    id: number,
    request: SavePurchaseRequisitionRequest,
  ): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.purchaseRequisitions.byID(id), request);
  }
  changePurchaseRequisitionStatus(
    id: number,
    status: PurchaseRequisitionStatus,
    note: string | null,
  ): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.purchaseRequisitions.status(id), { status, note });
  }

  getPurchaseOrders(request: PagedRequest): Observable<PagedResult<PurchaseOrderListItem>> {
    return this.api.get(API_ENDPOINTS.purchaseOrders.root, this.toPagedParams(request));
  }
  getPurchaseOrder(id: number): Observable<PurchaseOrderDetails> {
    return this.api.get(API_ENDPOINTS.purchaseOrders.byID(id));
  }
  createPurchaseOrder(request: SavePurchaseOrderRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.purchaseOrders.root, request);
  }
  updatePurchaseOrder(id: number, request: SavePurchaseOrderRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.purchaseOrders.byID(id), request);
  }
  changePurchaseOrderStatus(
    id: number,
    status: PurchaseOrderStatus,
    note: string | null,
  ): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.purchaseOrders.status(id), { status, note });
  }

  getGoodsReceipts(request: PagedRequest): Observable<PagedResult<GoodsReceiptListItem>> {
    return this.api.get(API_ENDPOINTS.goodsReceipts.root, this.toPagedParams(request));
  }
  getGoodsReceipt(id: number): Observable<GoodsReceiptDetails> {
    return this.api.get(API_ENDPOINTS.goodsReceipts.byID(id));
  }
  createGoodsReceipt(request: SaveGoodsReceiptRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.goodsReceipts.root, request);
  }
  updateGoodsReceipt(id: number, request: SaveGoodsReceiptRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.goodsReceipts.byID(id), request);
  }
  completeGoodsReceiptQualityCheck(
    id: number,
    qualityCheckStatus: QualityCheckStatus,
  ): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.goodsReceipts.qualityCheck(id), { qualityCheckStatus });
  }
  postGoodsReceipt(id: number, note: string | null): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.goodsReceipts.post(id), { note });
  }

  getPurchaseInvoices(request: PagedRequest): Observable<PagedResult<PurchaseInvoice>> {
    return this.api.get(API_ENDPOINTS.purchaseInvoices.root, this.toPagedParams(request));
  }
  createPurchaseInvoice(request: SavePurchaseInvoiceRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.purchaseInvoices.root, request);
  }
  confirmPurchaseInvoice(id: number): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.purchaseInvoices.confirm(id), {});
  }
  createSupplierPayment(request: CreateSupplierPaymentRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.purchaseInvoices.payments, request);
  }
  changeSupplierPaymentStatus(
    supplierPaymentID: number,
    status: SupplierPaymentStatus,
  ): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.purchaseInvoices.paymentStatus(supplierPaymentID), {
      status,
    });
  }
  getSupplierPayments(purchaseInvoiceID: number): Observable<readonly SupplierPayment[]> {
    return this.api.get(API_ENDPOINTS.purchaseInvoices.invoicePayments(purchaseInvoiceID));
  }

  getSupplierReturns(request: PagedRequest): Observable<PagedResult<SupplierReturnListItem>> {
    return this.api.get(API_ENDPOINTS.supplierReturns.root, this.toPagedParams(request));
  }
  getSupplierReturn(id: number): Observable<SupplierReturnDetails> {
    return this.api.get(API_ENDPOINTS.supplierReturns.byID(id));
  }
  createSupplierReturn(request: SaveSupplierReturnRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.supplierReturns.root, request);
  }
  updateSupplierReturn(id: number, request: SaveSupplierReturnRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.supplierReturns.byID(id), request);
  }
  changeSupplierReturnStatus(id: number, status: SupplierReturnStatus): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.supplierReturns.status(id), { status });
  }
  postSupplierReturn(id: number, note: string | null): Observable<boolean> {
    return this.api.post(API_ENDPOINTS.supplierReturns.post(id), { note });
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
