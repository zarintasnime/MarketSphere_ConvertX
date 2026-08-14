export enum UserStatus {
  Invited = 1,
  Active = 2,
  Locked = 3,
  Disabled = 4,
}

export enum EmployeeStatus {
  Active = 1,
  Inactive = 2,
  Suspended = 3,
  Terminated = 4,
}

export interface UserListItem {
  userID: number;
  fullName: string;
  email: string;
  phone: string | null;
  status: UserStatus;
  mustChangePassword: boolean;
  roleCodes: readonly string[];
}

export interface UserDetails extends UserListItem {
  accountActivatedAt: string | null;
  failedLoginCount: number;
  lockoutEndAt: string | null;
  roleIDs: readonly number[];
}

export interface CreateUserRequest {
  fullName: string;
  email: string;
  phone: string | null;
  temporaryPassword: string;
  activateImmediately: boolean;
  roleIDs: readonly number[];
}

export interface UpdateUserRequest {
  fullName: string;
  email: string;
  phone: string | null;
}

export interface PermissionItem {
  permissionID: number;
  moduleName: string;
  actionName: string;
  permissionCode: string;
  description: string | null;
  isAllowed: boolean;
}

export interface RoleListItem {
  roleID: number;
  roleCode: string;
  roleName: string;
  roleLevel: number;
  isActive: boolean;
}

export interface RoleDetails extends RoleListItem {
  description: string | null;
  permissions: readonly PermissionItem[];
}

export interface CreateRoleRequest {
  roleCode: string;
  roleName: string;
  description: string | null;
  roleLevel: number;
}

export interface UpdateRoleRequest {
  roleName: string;
  description: string | null;
  roleLevel: number;
  isActive: boolean;
}

export interface EmployeeListItem {
  employeeID: number;
  employeeCode: string;
  userID: number | null;
  userFullName: string | null;
  designationID: number;
  designationName: string;
  branchID: number;
  branchName: string;
  status: EmployeeStatus;
}

export interface EmployeeDetails {
  employeeID: number;
  employeeCode: string;
  userID: number | null;
  designationID: number;
  managerEmployeeID: number | null;
  branchID: number;
  regionID: number | null;
  areaID: number | null;
  territoryID: number | null;
  joiningDate: string;
  endDate: string | null;
  status: EmployeeStatus;
  phone: string | null;
  email: string | null;
}

export interface SaveEmployeeRequest {
  employeeCode?: string;
  userID: number | null;
  designationID: number;
  managerEmployeeID: number | null;
  branchID: number;
  regionID: number | null;
  areaID: number | null;
  territoryID: number | null;
  joiningDate: string;
  endDate: string | null;
  status: EmployeeStatus;
  phone: string | null;
  email: string | null;
}

export enum SettingDataType {
  String = 1,
  Integer = 2,
  Decimal = 3,
  Boolean = 4,
  DateTime = 5,
  Json = 6,
}

export enum SettingScopeType {
  Global = 1,
  Company = 2,
  Branch = 3,
  User = 4,
}

export interface SystemSetting {
  systemSettingID: number;
  settingKey: string;
  settingValue: string;
  dataType: SettingDataType;
  scopeType: SettingScopeType;
  scopeID: number | null;
  description: string | null;
  isEncrypted: boolean;
  updatedByUserID: number | null;
  updatedAt: string | null;
}

export interface SaveSystemSettingRequest {
  settingKey: string;
  settingValue: string;
  dataType: SettingDataType;
  scopeType: SettingScopeType;
  scopeID: number | null;
  description: string | null;
  isEncrypted: boolean;
}

export interface AuditLogItem {
  auditLogID: number;
  userID: number | null;
  actionName: string;
  entityType: string;
  entityID: number | null;
  oldValuesJson: string | null;
  newValuesJson: string | null;
  ipAddress: string | null;
  deviceIdentifier: string | null;
  createdAt: string;
}

export function userStatusLabel(status: UserStatus): string {
  return UserStatus[status] ?? 'Unknown';
}

export function employeeStatusLabel(status: EmployeeStatus): string {
  return EmployeeStatus[status] ?? 'Unknown';
}
