import type { Routes } from '@angular/router';

export const DASHBOARD_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/operations-dashboard-page/operations-dashboard-page.component').then(
        (module) => module.OperationsDashboardPageComponent,
      ),
    title: 'Operations Dashboard',
  },
];
