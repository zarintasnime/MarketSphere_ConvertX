import type { Routes } from '@angular/router';

import { permissionGuard } from '../../core/auth/permission.guard';

export const NOTIFICATIONS_ROUTES: Routes = [
  {
    path: '',
    canActivate: [permissionGuard],
    data: { permissions: ['infrastructure.notifications.view'] },
    loadComponent: () =>
      import('./pages/notifications-page/notifications-page.component').then(
        (module) => module.NotificationsPageComponent,
      ),
    title: 'Notifications',
  },
  {
    path: 'system-checks',
    canActivate: [permissionGuard],
    data: { permissions: ['infrastructure.system_checks.run'] },
    loadComponent: () =>
      import('./pages/system-checks-page/system-checks-page.component').then(
        (module) => module.SystemChecksPageComponent,
      ),
    title: 'System Checks',
  },
];
