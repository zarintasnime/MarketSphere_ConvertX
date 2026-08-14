import type { CurrentUser } from '../../../core/models/current-user.model';

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

export interface ActivateAccountRequest {
  token: string;
  newPassword: string;
}

export interface ResetPasswordRequest {
  token: string;
  newPassword: string;
}

export interface AuthSession {
  user: CurrentUser;
  accessToken: string;
  accessTokenExpiresAt: string;
  refreshToken: string;
  refreshTokenExpiresAt: string;
}

export interface AuthRouteData {
  permissions?: readonly string[];
  permissionMatch?: 'all' | 'any';
}
