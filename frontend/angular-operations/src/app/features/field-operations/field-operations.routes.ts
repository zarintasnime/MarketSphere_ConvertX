import type { Routes } from '@angular/router';
import { permissionGuard } from '../../core/auth/permission.guard';

export const FIELD_OPERATIONS_ROUTES: Routes = [
  {
    path: 'home',
    canActivate: [permissionGuard],
    data: { permissions: ['marketing.visits.view'] },
    loadComponent: () =>
      import('./pages/field-home-page/field-home-page.component').then(
        (m) => m.FieldHomePageComponent,
      ),
    title: 'Field Home',
  },
  {
    path: 'clients',
    canActivate: [permissionGuard],
    data: { permissions: ['marketing.visits.view'] },
    loadComponent: () =>
      import('./pages/assigned-clients-page/assigned-clients-page.component').then(
        (m) => m.AssignedClientsPageComponent,
      ),
    title: 'Assigned Clients',
  },
  {
    path: 'visits',
    canActivate: [permissionGuard],
    data: { permissions: ['marketing.visits.view'] },
    loadComponent: () =>
      import('./pages/field-visit-list-page/field-visit-list-page.component').then(
        (m) => m.FieldVisitListPageComponent,
      ),
    title: 'My Visits',
  },
  {
    path: 'visit/check-in',
    canActivate: [permissionGuard],
    data: { permissions: ['marketing.visits.manage'] },
    loadComponent: () =>
      import('./pages/field-visit-check-in-page/field-visit-check-in-page.component').then(
        (m) => m.FieldVisitCheckInPageComponent,
      ),
    title: 'Visit Check-In',
  },
  {
    path: 'active-visit',
    canActivate: [permissionGuard],
    data: { permissions: ['marketing.visits.manage'] },
    loadComponent: () =>
      import('./pages/active-visit-page/active-visit-page.component').then(
        (m) => m.ActiveVisitPageComponent,
      ),
    title: 'Active Visit',
  },
  {
    path: 'mt-po',
    canActivate: [permissionGuard],
    data: { permissions: ['orders.mt_purchase_orders.manage'] },
    loadComponent: () =>
      import('./pages/mt-po-capture-page/mt-po-capture-page.component').then(
        (m) => m.MtPoCapturePageComponent,
      ),
    title: 'Capture MT PO',
  },
  {
    path: 'bp-sell-out',
    canActivate: [permissionGuard],
    data: { permissions: ['marketing.bp_sell_out.manage'] },
    loadComponent: () =>
      import('./pages/bp-sell-out-capture-page/bp-sell-out-capture-page.component').then(
        (m) => m.BpSellOutCapturePageComponent,
      ),
    title: 'Capture BP Sell-Out',
  },
  {
    path: 'feedback',
    canActivate: [permissionGuard],
    data: { permissions: ['marketing.feedback.manage'] },
    loadComponent: () =>
      import('./pages/feedback-capture-page/feedback-capture-page.component').then(
        (m) => m.FeedbackCapturePageComponent,
      ),
    title: 'Capture Feedback',
  },
  {
    path: 'observation',
    canActivate: [permissionGuard],
    data: { permissions: ['marketing.market_observations.manage'] },
    loadComponent: () =>
      import('./pages/market-observation-capture-page/market-observation-capture-page.component').then(
        (m) => m.MarketObservationCapturePageComponent,
      ),
    title: 'Capture Observation',
  },
  {
    path: 'notifications',
    canActivate: [permissionGuard],
    data: { permissions: ['infrastructure.notifications.view'] },
    loadComponent: () =>
      import('./pages/field-notifications-page/field-notifications-page.component').then(
        (m) => m.FieldNotificationsPageComponent,
      ),
    title: 'Field Notifications',
  },
];
