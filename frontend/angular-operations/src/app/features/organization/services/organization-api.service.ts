import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_ENDPOINTS } from '../../../core/config/api-endpoints';
import { ApiClientService } from '../../../core/http/api-client.service';
import type {
  Area,
  Branch,
  Company,
  CreateEmployeeRouteAssignmentRequest,
  CreateEmployeeTerritoryAssignmentRequest,
  CreateRouteRequest,
  Region,
  RouteItem,
  SaveBranchRequest,
  Territory,
  UpdateCompanyRequest,
  UpdateRouteRequest,
} from '../models/organization.model';

@Injectable({ providedIn: 'root' })
export class OrganizationApiService {
  private readonly api = inject(ApiClientService);

  getCompanies(): Observable<readonly Company[]> {
    return this.api.get(API_ENDPOINTS.organization.companies);
  }
  updateCompany(companyID: number, request: UpdateCompanyRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.organization.company(companyID), request);
  }

  getBranches(companyID?: number | null): Observable<readonly Branch[]> {
    const params = companyID ? new HttpParams().set('companyID', companyID) : undefined;
    return this.api.get(API_ENDPOINTS.organization.branches, params);
  }
  createBranch(
    request: SaveBranchRequest & { companyID: number; branchCode: string },
  ): Observable<number> {
    return this.api.post(API_ENDPOINTS.organization.branches, request);
  }
  updateBranch(
    branchID: number,
    request: SaveBranchRequest & { isActive: boolean },
  ): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.organization.branch(branchID), request);
  }

  getRegions(companyID?: number | null): Observable<readonly Region[]> {
    const params = companyID ? new HttpParams().set('companyID', companyID) : undefined;
    return this.api.get(API_ENDPOINTS.geography.regions, params);
  }
  createRegion(request: {
    companyID: number;
    regionCode: string;
    regionName: string;
  }): Observable<number> {
    return this.api.post(API_ENDPOINTS.geography.regions, request);
  }
  updateRegion(
    regionID: number,
    request: { regionName: string; isActive: boolean },
  ): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.geography.region(regionID), request);
  }

  getAreas(regionID?: number | null): Observable<readonly Area[]> {
    const params = regionID ? new HttpParams().set('regionID', regionID) : undefined;
    return this.api.get(API_ENDPOINTS.geography.areas, params);
  }
  createArea(request: {
    regionID: number;
    areaCode: string;
    areaName: string;
  }): Observable<number> {
    return this.api.post(API_ENDPOINTS.geography.areas, request);
  }
  updateArea(
    areaID: number,
    request: { areaName: string; isActive: boolean },
  ): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.geography.area(areaID), request);
  }

  getTerritories(areaID?: number | null): Observable<readonly Territory[]> {
    const params = areaID ? new HttpParams().set('areaID', areaID) : undefined;
    return this.api.get(API_ENDPOINTS.geography.territories, params);
  }
  createTerritory(request: {
    areaID: number;
    territoryCode: string;
    territoryName: string;
  }): Observable<number> {
    return this.api.post(API_ENDPOINTS.geography.territories, request);
  }
  updateTerritory(
    territoryID: number,
    request: { territoryName: string; isActive: boolean },
  ): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.geography.territory(territoryID), request);
  }

  getRoutes(territoryID?: number | null): Observable<readonly RouteItem[]> {
    const params = territoryID ? new HttpParams().set('territoryID', territoryID) : undefined;
    return this.api.get(API_ENDPOINTS.routes.root, params);
  }
  createRoute(request: CreateRouteRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.routes.root, request);
  }
  updateRoute(routeID: number, request: UpdateRouteRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.routes.byID(routeID), request);
  }
  assignEmployeeRoute(request: CreateEmployeeRouteAssignmentRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.routes.employeeRouteAssignments, request);
  }
  assignEmployeeTerritory(request: CreateEmployeeTerritoryAssignmentRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.routes.employeeTerritoryAssignments, request);
  }
}
