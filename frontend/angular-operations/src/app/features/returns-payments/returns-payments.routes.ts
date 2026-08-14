import type { Routes } from '@angular/router';
import { permissionGuard } from '../../core/auth/permission.guard';

export const RETURNS_PAYMENTS_ROUTES: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'returns' },
  {
    path: 'returns',
    canActivate: [permissionGuard],
    data: { permissions: ['fulfilment.returns.view'] },
    loadComponent: () =>
      import('./pages/return-requests-page/return-requests-page.component').then(
        (m) => m.ReturnRequestsPageComponent,
      ),
  },
  {
    path: 'returns/:id',
    canActivate: [permissionGuard],
    data: { permissions: ['fulfilment.returns.view'] },
    loadComponent: () =>
      import('./pages/return-details-page/return-details-page.component').then(
        (m) => m.ReturnDetailsPageComponent,
      ),
  },
  {
    path: 'credit-notes',
    canActivate: [permissionGuard],
    data: { permissions: ['fulfilment.credit_notes.view'] },
    loadComponent: () =>
      import('./pages/credit-notes-page/credit-notes-page.component').then(
        (m) => m.CreditNotesPageComponent,
      ),
  },
  {
    path: 'payments',
    canActivate: [permissionGuard],
    data: { permissions: ['payments.payments.view'] },
    loadComponent: () =>
      import('./pages/payments-page/payments-page.component').then((m) => m.PaymentsPageComponent),
  },
  {
    path: 'payments/:id/allocations',
    canActivate: [permissionGuard],
    data: { permissions: ['payments.payments.view'] },
    loadComponent: () =>
      import('./pages/payment-allocation-page/payment-allocation-page.component').then(
        (m) => m.PaymentAllocationPageComponent,
      ),
  },
];
