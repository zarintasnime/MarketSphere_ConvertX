import type { Routes } from '@angular/router';
import { permissionGuard } from '../../core/auth/permission.guard';

export const ORDERS_ROUTES: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'list' },
  {
    path: 'mt-purchase-orders',
    canActivate: [permissionGuard],
    data: { permissions: ['orders.mt_purchase_orders.view'] },
    loadComponent: () =>
      import('./pages/mt-purchase-orders-page/mt-purchase-orders-page.component').then(
        (m) => m.MtPurchaseOrdersPageComponent,
      ),
  },
  {
    path: 'mt-purchase-orders/:id',
    canActivate: [permissionGuard],
    data: { permissions: ['orders.mt_purchase_orders.view'] },
    loadComponent: () =>
      import('./pages/mt-po-details-page/mt-po-details-page.component').then(
        (m) => m.MtPoDetailsPageComponent,
      ),
  },
  {
    path: 'list',
    canActivate: [permissionGuard],
    data: { permissions: ['orders.orders.view'] },
    loadComponent: () =>
      import('./pages/orders-list-page/orders-list-page.component').then(
        (m) => m.OrdersListPageComponent,
      ),
  },
  {
    path: 'new',
    canActivate: [permissionGuard],
    data: { permissions: ['orders.orders.manage'] },
    loadComponent: () =>
      import('./pages/order-form-page/order-form-page.component').then(
        (m) => m.OrderFormPageComponent,
      ),
  },
  {
    path: 'approval-queue/all',
    canActivate: [permissionGuard],
    data: { permissions: ['infrastructure.approvals.view'] },
    loadComponent: () =>
      import('./pages/approval-queue-page/approval-queue-page.component').then(
        (m) => m.ApprovalQueuePageComponent,
      ),
  },
  { path: 'approval-queue', pathMatch: 'full', redirectTo: 'approval-queue/all' },
  {
    path: ':id',
    canActivate: [permissionGuard],
    data: { permissions: ['orders.orders.view'] },
    loadComponent: () =>
      import('./pages/order-details-page/order-details-page.component').then(
        (m) => m.OrderDetailsPageComponent,
      ),
  },
];
