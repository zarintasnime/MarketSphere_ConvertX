import type { Routes } from '@angular/router';
import { permissionGuard } from '../../core/auth/permission.guard';

export const FULFILMENT_ROUTES: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'invoices' },
  {
    path: 'invoices',
    canActivate: [permissionGuard],
    data: { permissions: ['fulfilment.invoices.view'] },
    loadComponent: () =>
      import('./pages/invoices-page/invoices-page.component').then((m) => m.InvoicesPageComponent),
  },
  {
    path: 'invoices/:id',
    canActivate: [permissionGuard],
    data: { permissions: ['fulfilment.invoices.view'] },
    loadComponent: () =>
      import('./pages/invoice-details-page/invoice-details-page.component').then(
        (m) => m.InvoiceDetailsPageComponent,
      ),
  },
  {
    path: 'pick-lists',
    canActivate: [permissionGuard],
    data: { permissions: ['fulfilment.pick_lists.view'] },
    loadComponent: () =>
      import('./pages/pick-lists-page/pick-lists-page.component').then(
        (m) => m.PickListsPageComponent,
      ),
  },
  {
    path: 'pick-lists/:id',
    canActivate: [permissionGuard],
    data: { permissions: ['fulfilment.pick_lists.view'] },
    loadComponent: () =>
      import('./pages/pick-list-details-page/pick-list-details-page.component').then(
        (m) => m.PickListDetailsPageComponent,
      ),
  },
  {
    path: 'deliveries',
    canActivate: [permissionGuard],
    data: { permissions: ['fulfilment.deliveries.view'] },
    loadComponent: () =>
      import('./pages/deliveries-page/deliveries-page.component').then(
        (m) => m.DeliveriesPageComponent,
      ),
  },
  {
    path: 'deliveries/:id',
    canActivate: [permissionGuard],
    data: { permissions: ['fulfilment.deliveries.view'] },
    loadComponent: () =>
      import('./pages/delivery-details-page/delivery-details-page.component').then(
        (m) => m.DeliveryDetailsPageComponent,
      ),
  },
];
