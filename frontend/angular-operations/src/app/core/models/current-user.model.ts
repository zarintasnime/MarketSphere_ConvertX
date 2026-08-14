export interface CurrentUser {
  userID: number;
  employeeID: number | null;
  fullName: string;
  email: string;
  mustChangePassword: boolean;
  roleCodes: readonly string[];
  permissionCodes: readonly string[];
}

export function hasRole(user: CurrentUser | null, roleCode: string): boolean {
  return user?.roleCodes.includes(roleCode) ?? false;
}

export function hasAnyRole(user: CurrentUser | null, roleCodes: readonly string[]): boolean {
  return roleCodes.some((roleCode) => hasRole(user, roleCode));
}

export function hasPermission(user: CurrentUser | null, permissionCode: string): boolean {
  return user?.permissionCodes.includes(permissionCode) ?? false;
}

export function hasAnyPermission(
  user: CurrentUser | null,
  permissionCodes: readonly string[],
): boolean {
  return permissionCodes.some((permissionCode) => hasPermission(user, permissionCode));
}

export function hasEveryPermission(
  user: CurrentUser | null,
  permissionCodes: readonly string[],
): boolean {
  return permissionCodes.every((permissionCode) => hasPermission(user, permissionCode));
}
