import type { Routes } from '@angular/router';

import { authGuard } from './core/auth/auth.guard';
import { FieldShellComponent } from './layout/field-shell.component';
import { OperationsShellComponent } from './layout/operations-shell.component';

export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then((module) => module.AUTH_ROUTES),
  },
  {
    path: 'field',
    component: FieldShellComponent,
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'home' },
      {
        path: '',
        loadChildren: () =>
          import('./features/field-operations/field-operations.routes').then(
            (module) => module.FIELD_OPERATIONS_ROUTES,
          ),
      },
    ],
  },
  {
    path: '',
    component: OperationsShellComponent,
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      {
        path: 'dashboard',
        loadChildren: () =>
          import('./features/dashboard/dashboard.routes').then((module) => module.DASHBOARD_ROUTES),
      },
      {
        path: 'administration',
        loadChildren: () =>
          import('./features/administration/administration.routes').then(
            (module) => module.ADMINISTRATION_ROUTES,
          ),
      },
      {
        path: 'organization',
        loadChildren: () =>
          import('./features/organization/organization.routes').then(
            (module) => module.ORGANIZATION_ROUTES,
          ),
      },
      {
        path: 'products',
        loadChildren: () =>
          import('./features/products/products.routes').then((module) => module.PRODUCTS_ROUTES),
      },
      {
        path: 'crm',
        loadChildren: () => import('./features/crm/crm.routes').then((module) => module.CRM_ROUTES),
      },
      {
        path: 'marketing',
        loadChildren: () =>
          import('./features/marketing/marketing.routes').then((module) => module.MARKETING_ROUTES),
      },
      {
        path: 'procurement',
        loadChildren: () =>
          import('./features/procurement/procurement.routes').then(
            (module) => module.PROCUREMENT_ROUTES,
          ),
      },
      {
        path: 'inventory',
        loadChildren: () =>
          import('./features/inventory/inventory.routes').then((module) => module.INVENTORY_ROUTES),
      },
      {
        path: 'orders',
        loadChildren: () =>
          import('./features/orders/orders.routes').then((module) => module.ORDERS_ROUTES),
      },
      {
        path: 'fulfilment',
        loadChildren: () =>
          import('./features/fulfilment/fulfilment.routes').then(
            (module) => module.FULFILMENT_ROUTES,
          ),
      },
      {
        path: 'returns-payments',
        loadChildren: () =>
          import('./features/returns-payments/returns-payments.routes').then(
            (module) => module.RETURNS_PAYMENTS_ROUTES,
          ),
      },
      {
        path: 'notifications',
        loadChildren: () =>
          import('./features/notifications/notifications.routes').then(
            (module) => module.NOTIFICATIONS_ROUTES,
          ),
      },
    ],
  },
  { path: '**', redirectTo: 'dashboard' },
];
