import type { Routes } from '@angular/router';
import { permissionGuard } from '../../core/auth/permission.guard';

export const INVENTORY_ROUTES: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'stock-balance' },
  {
    path: 'warehouses',
    canActivate: [permissionGuard],
    data: { permissions: ['inventory.warehouses.view'] },
    loadComponent: () =>
      import('./pages/warehouses-page/warehouses-page.component').then(
        (m) => m.WarehousesPageComponent,
      ),
  },
  {
    path: 'stock-balance',
    canActivate: [permissionGuard],
    data: { permissions: ['inventory.stock.view'] },
    loadComponent: () =>
      import('./pages/stock-balance-page/stock-balance-page.component').then(
        (m) => m.StockBalancePageComponent,
      ),
  },
  {
    path: 'stock-movements',
    canActivate: [permissionGuard],
    data: { permissions: ['inventory.stock_movements.view'] },
    loadComponent: () =>
      import('./pages/stock-movement-page/stock-movement-page.component').then(
        (m) => m.StockMovementPageComponent,
      ),
  },
  {
    path: 'stock-reservations',
    canActivate: [permissionGuard],
    data: { permissions: ['inventory.stock.view'] },
    loadComponent: () =>
      import('./pages/stock-reservations-page/stock-reservations-page.component').then(
        (m) => m.StockReservationsPageComponent,
      ),
  },
  {
    path: 'stock-transfers',
    canActivate: [permissionGuard],
    data: { permissions: ['inventory.stock_transfers.view'] },
    loadComponent: () =>
      import('./pages/stock-transfers-page/stock-transfers-page.component').then(
        (m) => m.StockTransfersPageComponent,
      ),
  },
  {
    path: 'stock-adjustments',
    canActivate: [permissionGuard],
    data: { permissions: ['inventory.stock_adjustments.view'] },
    loadComponent: () =>
      import('./pages/stock-adjustments-page/stock-adjustments-page.component').then(
        (m) => m.StockAdjustmentsPageComponent,
      ),
  },
  {
    path: 'batch-expiry',
    canActivate: [permissionGuard],
    data: { permissions: ['inventory.stock.view'] },
    loadComponent: () =>
      import('./pages/batch-expiry-page/batch-expiry-page.component').then(
        (m) => m.BatchExpiryPageComponent,
      ),
  },
];
