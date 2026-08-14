import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, forkJoin, map, of } from 'rxjs';

import { AuthService } from '../../../core/auth/auth.service';
import { API_ENDPOINTS } from '../../../core/config/api-endpoints';
import { ApiClientService } from '../../../core/http/api-client.service';
import type { PagedResult } from '../../../core/models/paged-result.model';
import type { Branch } from '../../organization/models/organization.model';
import type {
  EmployeeListItem,
  RoleListItem,
  UserListItem,
} from '../../administration/models/administration.model';
import type { OperationsDashboardSummary } from '../models/dashboard.model';

@Injectable({ providedIn: 'root' })
export class DashboardApiService {
  private readonly api = inject(ApiClientService);
  private readonly auth = inject(AuthService);

  getSummary(): Observable<OperationsDashboardSummary> {
    const params = new HttpParams()
      .set('pageNumber', 1)
      .set('pageSize', 200)
      .set('sortDirection', 'asc');

    const users$ = this.auth.hasPermission('users.view')
      ? this.api.get<PagedResult<UserListItem>>(API_ENDPOINTS.users.root, params)
      : of<PagedResult<UserListItem>>({
          items: [],
          totalCount: 0,
          pageNumber: 1,
          pageSize: 200,
          totalPages: 0,
        });

    const roles$ = this.auth.hasPermission('roles.view')
      ? this.api.get<readonly RoleListItem[]>(API_ENDPOINTS.roles.root)
      : of<readonly RoleListItem[]>([]);

    const employees$ = this.auth.hasPermission('employees.view')
      ? this.api.get<PagedResult<EmployeeListItem>>(API_ENDPOINTS.employees.root, params)
      : of<PagedResult<EmployeeListItem>>({
          items: [],
          totalCount: 0,
          pageNumber: 1,
          pageSize: 200,
          totalPages: 0,
        });

    const branches$ = this.auth.hasPermission('organization.view')
      ? this.api.get<readonly Branch[]>(API_ENDPOINTS.organization.branches)
      : of<readonly Branch[]>([]);

    return forkJoin({
      users: users$,
      roles: roles$,
      employees: employees$,
      branches: branches$,
    }).pipe(
      map(({ users, roles, employees, branches }) => ({
        totalUsers: users.totalCount,
        activeUsers: users.items.filter((item) => item.status === 2).length,
        totalRoles: roles.length,
        activeRoles: roles.filter((item) => item.isActive).length,
        totalEmployees: employees.totalCount,
        activeEmployees: employees.items.filter((item) => item.status === 1).length,
        totalBranches: branches.length,
        activeBranches: branches.filter((item) => item.isActive).length,
      })),
      catchError(() =>
        of({
          totalUsers: 0,
          activeUsers: 0,
          totalRoles: 0,
          activeRoles: 0,
          totalEmployees: 0,
          activeEmployees: 0,
          totalBranches: 0,
          activeBranches: 0,
        }),
      ),
    );
  }
}
