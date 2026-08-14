import type { Routes } from '@angular/router';

import { permissionGuard } from '../../core/auth/permission.guard';

export const ADMINISTRATION_ROUTES: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'users' },
  {
    path: 'users',
    canActivate: [permissionGuard],
    data: { permissions: ['users.view'] },
    loadComponent: () =>
      import('./pages/users-list-page/users-list-page.component').then(
        (m) => m.UsersListPageComponent,
      ),
    title: 'Users',
  },
  {
    path: 'users/new',
    canActivate: [permissionGuard],
    data: { permissions: ['users.create'] },
    loadComponent: () =>
      import('./pages/user-form-page/user-form-page.component').then(
        (m) => m.UserFormPageComponent,
      ),
    title: 'Create User',
  },
  {
    path: 'users/:userID/edit',
    canActivate: [permissionGuard],
    data: { permissions: ['users.update'] },
    loadComponent: () =>
      import('./pages/user-form-page/user-form-page.component').then(
        (m) => m.UserFormPageComponent,
      ),
    title: 'Edit User',
  },
  {
    path: 'roles',
    canActivate: [permissionGuard],
    data: { permissions: ['roles.view'] },
    loadComponent: () =>
      import('./pages/roles-list-page/roles-list-page.component').then(
        (m) => m.RolesListPageComponent,
      ),
    title: 'Roles',
  },
  {
    path: 'roles/:roleID/permissions',
    canActivate: [permissionGuard],
    data: { permissions: ['roles.manage_permissions'] },
    loadComponent: () =>
      import('./pages/role-permission-page/role-permission-page.component').then(
        (m) => m.RolePermissionPageComponent,
      ),
    title: 'Role Permissions',
  },
  {
    path: 'employees',
    canActivate: [permissionGuard],
    data: { permissions: ['employees.view'] },
    loadComponent: () =>
      import('./pages/employees-list-page/employees-list-page.component').then(
        (m) => m.EmployeesListPageComponent,
      ),
    title: 'Employees',
  },
  {
    path: 'employees/new',
    canActivate: [permissionGuard],
    data: { permissions: ['employees.create'] },
    loadComponent: () =>
      import('./pages/employee-form-page/employee-form-page.component').then(
        (m) => m.EmployeeFormPageComponent,
      ),
    title: 'Create Employee',
  },
  {
    path: 'employees/:employeeID/edit',
    canActivate: [permissionGuard],
    data: { permissions: ['employees.update'] },
    loadComponent: () =>
      import('./pages/employee-form-page/employee-form-page.component').then(
        (m) => m.EmployeeFormPageComponent,
      ),
    title: 'Edit Employee',
  },
  {
    path: 'settings',
    canActivate: [permissionGuard],
    data: { permissions: ['infrastructure.settings.view'] },
    loadComponent: () =>
      import('./pages/settings-page/settings-page.component').then((m) => m.SettingsPageComponent),
    title: 'System Settings',
  },
  {
    path: 'audit-log',
    canActivate: [permissionGuard],
    data: { permissions: ['infrastructure.audit_logs.view'] },
    loadComponent: () =>
      import('./pages/audit-log-page/audit-log-page.component').then(
        (m) => m.AuditLogPageComponent,
      ),
    title: 'Audit Log',
  },
];
