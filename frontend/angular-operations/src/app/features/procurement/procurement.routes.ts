import type { Routes } from '@angular/router';
import { permissionGuard } from '../../core/auth/permission.guard';

export const PROCUREMENT_ROUTES: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'suppliers' },
  {
    path: 'suppliers',
    canActivate: [permissionGuard],
    data: { permissions: ['procurement.suppliers.view'] },
    loadComponent: () =>
      import('./pages/suppliers-list-page/suppliers-list-page.component').then(
        (m) => m.SuppliersListPageComponent,
      ),
  },
  {
    path: 'suppliers/new',
    canActivate: [permissionGuard],
    data: { permissions: ['procurement.suppliers.manage'] },
    loadComponent: () =>
      import('./pages/supplier-form-page/supplier-form-page.component').then(
        (m) => m.SupplierFormPageComponent,
      ),
  },
  {
    path: 'suppliers/:id/edit',
    canActivate: [permissionGuard],
    data: { permissions: ['procurement.suppliers.manage'] },
    loadComponent: () =>
      import('./pages/supplier-form-page/supplier-form-page.component').then(
        (m) => m.SupplierFormPageComponent,
      ),
  },
  {
    path: 'purchase-requisitions',
    canActivate: [permissionGuard],
    data: { permissions: ['procurement.purchase_requisitions.view'] },
    loadComponent: () =>
      import('./pages/purchase-requisitions-page/purchase-requisitions-page.component').then(
        (m) => m.PurchaseRequisitionsPageComponent,
      ),
  },
  {
    path: 'purchase-requisitions/new',
    canActivate: [permissionGuard],
    data: { permissions: ['procurement.purchase_requisitions.manage'] },
    loadComponent: () =>
      import('./pages/purchase-requisition-form-page/purchase-requisition-form-page.component').then(
        (m) => m.PurchaseRequisitionFormPageComponent,
      ),
  },
  {
    path: 'purchase-requisitions/:id/edit',
    canActivate: [permissionGuard],
    data: { permissions: ['procurement.purchase_requisitions.manage'] },
    loadComponent: () =>
      import('./pages/purchase-requisition-form-page/purchase-requisition-form-page.component').then(
        (m) => m.PurchaseRequisitionFormPageComponent,
      ),
  },
  {
    path: 'purchase-orders',
    canActivate: [permissionGuard],
    data: { permissions: ['procurement.purchase_orders.view'] },
    loadComponent: () =>
      import('./pages/purchase-orders-page/purchase-orders-page.component').then(
        (m) => m.PurchaseOrdersPageComponent,
      ),
  },
  {
    path: 'purchase-orders/new',
    canActivate: [permissionGuard],
    data: { permissions: ['procurement.purchase_orders.manage'] },
    loadComponent: () =>
      import('./pages/purchase-order-form-page/purchase-order-form-page.component').then(
        (m) => m.PurchaseOrderFormPageComponent,
      ),
  },
  {
    path: 'purchase-orders/:id/edit',
    canActivate: [permissionGuard],
    data: { permissions: ['procurement.purchase_orders.manage'] },
    loadComponent: () =>
      import('./pages/purchase-order-form-page/purchase-order-form-page.component').then(
        (m) => m.PurchaseOrderFormPageComponent,
      ),
  },
  {
    path: 'goods-receipts',
    canActivate: [permissionGuard],
    data: { permissions: ['procurement.goods_receipts.view'] },
    loadComponent: () =>
      import('./pages/goods-receipts-page/goods-receipts-page.component').then(
        (m) => m.GoodsReceiptsPageComponent,
      ),
  },
  {
    path: 'goods-receipts/new',
    canActivate: [permissionGuard],
    data: { permissions: ['procurement.goods_receipts.manage'] },
    loadComponent: () =>
      import('./pages/goods-receipt-form-page/goods-receipt-form-page.component').then(
        (m) => m.GoodsReceiptFormPageComponent,
      ),
  },
  {
    path: 'goods-receipts/:id/edit',
    canActivate: [permissionGuard],
    data: { permissions: ['procurement.goods_receipts.manage'] },
    loadComponent: () =>
      import('./pages/goods-receipt-form-page/goods-receipt-form-page.component').then(
        (m) => m.GoodsReceiptFormPageComponent,
      ),
  },
  {
    path: 'purchase-invoices',
    canActivate: [permissionGuard],
    data: { permissions: ['procurement.purchase_invoices.view'] },
    loadComponent: () =>
      import('./pages/purchase-invoices-page/purchase-invoices-page.component').then(
        (m) => m.PurchaseInvoicesPageComponent,
      ),
  },
  {
    path: 'supplier-returns',
    canActivate: [permissionGuard],
    data: { permissions: ['procurement.supplier_returns.view'] },
    loadComponent: () =>
      import('./pages/supplier-returns-page/supplier-returns-page.component').then(
        (m) => m.SupplierReturnsPageComponent,
      ),
  },
];
