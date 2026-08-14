import type { Routes } from '@angular/router';

import { permissionGuard } from '../../core/auth/permission.guard';

export const ORGANIZATION_ROUTES: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'company' },
  {
    path: 'company',
    canActivate: [permissionGuard],
    data: { permissions: ['organization.view'] },
    loadComponent: () =>
      import('./pages/company-page/company-page.component').then((m) => m.CompanyPageComponent),
    title: 'Company',
  },
  {
    path: 'branches',
    canActivate: [permissionGuard],
    data: { permissions: ['organization.view'] },
    loadComponent: () =>
      import('./pages/branches-page/branches-page.component').then((m) => m.BranchesPageComponent),
    title: 'Branches',
  },
  {
    path: 'geography',
    canActivate: [permissionGuard],
    data: { permissions: ['geography.view'] },
    loadComponent: () =>
      import('./pages/geography-page/geography-page.component').then(
        (m) => m.GeographyPageComponent,
      ),
    title: 'Geography',
  },
  {
    path: 'routes',
    canActivate: [permissionGuard],
    data: { permissions: ['routes.view'] },
    loadComponent: () =>
      import('./pages/routes-page/routes-page.component').then((m) => m.RoutesPageComponent),
    title: 'Routes',
  },
];
