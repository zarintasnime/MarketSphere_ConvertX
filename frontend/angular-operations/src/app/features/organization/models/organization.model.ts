export enum BranchType {
  HeadOffice = 1,
  RegionalOffice = 2,
  Depot = 3,
}

export enum VisitFrequency {
  Daily = 1,
  Weekly = 2,
  BiWeekly = 3,
  Monthly = 4,
  Custom = 5,
}

export enum GeographyScopeType {
  Region = 1,
  Area = 2,
  Territory = 3,
}

export interface Company {
  companyID: number;
  companyCode: string;
  companyName: string;
  tradeLicenseNo: string | null;
  phone: string | null;
  email: string | null;
  address: string | null;
  isActive: boolean;
}

export interface UpdateCompanyRequest {
  companyName: string;
  tradeLicenseNo: string | null;
  phone: string | null;
  email: string | null;
  address: string | null;
  isActive: boolean;
}

export interface Branch {
  branchID: number;
  companyID: number;
  branchCode: string;
  branchName: string;
  branchType: BranchType;
  address: string | null;
  phone: string | null;
  isActive: boolean;
}

export interface SaveBranchRequest {
  companyID?: number;
  branchCode?: string;
  branchName: string;
  branchType: BranchType;
  address: string | null;
  phone: string | null;
  isActive?: boolean;
}

export interface Region {
  regionID: number;
  companyID: number;
  regionCode: string;
  regionName: string;
  isActive: boolean;
}

export interface Area {
  areaID: number;
  regionID: number;
  areaCode: string;
  areaName: string;
  isActive: boolean;
}

export interface Territory {
  territoryID: number;
  areaID: number;
  territoryCode: string;
  territoryName: string;
  isActive: boolean;
}

export interface RouteItem {
  routeID: number;
  territoryID: number;
  routeCode: string;
  routeName: string;
  dayOfWeek: number | null;
  visitFrequency: VisitFrequency;
  isActive: boolean;
}

export interface CreateRouteRequest {
  territoryID: number;
  routeCode: string;
  routeName: string;
  dayOfWeek: number | null;
  visitFrequency: VisitFrequency;
}

export interface UpdateRouteRequest {
  routeName: string;
  dayOfWeek: number | null;
  visitFrequency: VisitFrequency;
  isActive: boolean;
}

export interface CreateEmployeeRouteAssignmentRequest {
  employeeID: number;
  routeID: number;
  effectiveFrom: string;
  effectiveTo: string | null;
  dayOfWeek: number | null;
  isPrimary: boolean;
}

export interface CreateEmployeeTerritoryAssignmentRequest {
  employeeID: number;
  scopeType: GeographyScopeType;
  regionID: number | null;
  areaID: number | null;
  territoryID: number | null;
  effectiveFrom: string;
  effectiveTo: string | null;
  isPrimary: boolean;
}
