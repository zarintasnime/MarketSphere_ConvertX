import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_ENDPOINTS } from '../../../core/config/api-endpoints';
import { ApiClientService } from '../../../core/http/api-client.service';
import type { PagedRequest, PagedResult } from '../../../core/models/paged-result.model';
import type {
  AuditLogItem,
  CreateRoleRequest,
  CreateUserRequest,
  EmployeeDetails,
  EmployeeListItem,
  RoleDetails,
  RoleListItem,
  SaveEmployeeRequest,
  SaveSystemSettingRequest,
  SystemSetting,
  UpdateRoleRequest,
  UpdateUserRequest,
  UserDetails,
  UserListItem,
  UserStatus,
} from '../models/administration.model';

@Injectable({ providedIn: 'root' })
export class AdministrationApiService {
  private readonly api = inject(ApiClientService);

  getUsers(request: PagedRequest): Observable<PagedResult<UserListItem>> {
    return this.api.get(API_ENDPOINTS.users.root, this.toPagedParams(request));
  }

  getUser(userID: number): Observable<UserDetails> {
    return this.api.get(API_ENDPOINTS.users.byID(userID));
  }

  createUser(request: CreateUserRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.users.root, request);
  }

  updateUser(userID: number, request: UpdateUserRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.users.byID(userID), request);
  }

  changeUserStatus(userID: number, status: UserStatus): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.users.status(userID), { status });
  }

  assignUserRoles(userID: number, roleIDs: readonly number[]): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.users.roles(userID), { roleIDs });
  }

  getRoles(): Observable<readonly RoleListItem[]> {
    return this.api.get(API_ENDPOINTS.roles.root);
  }

  getRole(roleID: number): Observable<RoleDetails> {
    return this.api.get(API_ENDPOINTS.roles.byID(roleID));
  }

  createRole(request: CreateRoleRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.roles.root, request);
  }

  updateRole(roleID: number, request: UpdateRoleRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.roles.byID(roleID), request);
  }

  updateRolePermissions(
    roleID: number,
    allowedPermissionIDs: readonly number[],
  ): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.roles.permissions(roleID), { allowedPermissionIDs });
  }

  getEmployees(request: PagedRequest): Observable<PagedResult<EmployeeListItem>> {
    return this.api.get(API_ENDPOINTS.employees.root, this.toPagedParams(request));
  }

  getEmployee(employeeID: number): Observable<EmployeeDetails> {
    return this.api.get(API_ENDPOINTS.employees.byID(employeeID));
  }

  createEmployee(request: SaveEmployeeRequest & { employeeCode: string }): Observable<number> {
    return this.api.post(API_ENDPOINTS.employees.root, request);
  }

  updateEmployee(employeeID: number, request: SaveEmployeeRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.employees.byID(employeeID), request);
  }

  getSettings(): Observable<readonly SystemSetting[]> {
    return this.api.get(API_ENDPOINTS.settings.root);
  }

  createSetting(request: SaveSystemSettingRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.settings.root, request);
  }

  updateSetting(settingID: number, request: SaveSystemSettingRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.settings.byID(settingID), request);
  }

  getAuditLogs(request: PagedRequest): Observable<PagedResult<AuditLogItem>> {
    return this.api.get(API_ENDPOINTS.audit.logs, this.toPagedParams(request));
  }

  private toPagedParams(request: PagedRequest): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', request.pageNumber)
      .set('pageSize', request.pageSize);

    if (request.search?.trim()) {
      params = params.set('search', request.search.trim());
    }

    if (request.sortBy) {
      params = params.set('sortBy', request.sortBy);
    }

    if (request.sortDirection) {
      params = params.set('sortDirection', request.sortDirection);
    }

    return params;
  }
}
