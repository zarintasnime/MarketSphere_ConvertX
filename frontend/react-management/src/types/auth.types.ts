export interface CurrentUser {
  userID: number;
  employeeID: number | null;
  fullName: string;
  email: string;
  mustChangePassword: boolean;
  roleCodes: readonly string[];
  permissionCodes: readonly string[];
}

export interface AuthSession {
  user: CurrentUser;
  accessToken: string;
  accessTokenExpiresAt: string;
  refreshToken: string;
  refreshTokenExpiresAt: string;
}

export interface LoginCredentials {
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
  deviceIdentifier: string;
  deviceName: string | null;
}

export interface RefreshSessionRequest {
  refreshToken: string;
  deviceIdentifier: string;
  deviceName: string | null;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

export interface AuthContextValue {
  session: AuthSession | null;
  currentUser: CurrentUser | null;
  isAuthenticated: boolean;
  isInitializing: boolean;
  login: (credentials: LoginCredentials) => Promise<AuthSession>;
  logout: () => void;
  refreshCurrentUser: () => Promise<CurrentUser | null>;
  hasPermission: (permissionCode: string) => boolean;
  hasAnyPermission: (permissionCodes: readonly string[]) => boolean;
  hasEveryPermission: (permissionCodes: readonly string[]) => boolean;
}
