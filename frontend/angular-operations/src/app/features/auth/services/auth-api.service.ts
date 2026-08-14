import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_ENDPOINTS } from '../../../core/config/api-endpoints';
import { ApiClientService } from '../../../core/http/api-client.service';
import type { CurrentUser } from '../../../core/models/current-user.model';
import type {
  ActivateAccountRequest,
  AuthSession,
  ChangePasswordRequest,
  LoginRequest,
  RefreshSessionRequest,
  ResetPasswordRequest,
} from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private readonly api = inject(ApiClientService);

  login(request: LoginRequest): Observable<AuthSession> {
    return this.api.post<AuthSession, LoginRequest>(API_ENDPOINTS.auth.login, request);
  }

  refreshSession(request: RefreshSessionRequest): Observable<AuthSession> {
    return this.api.post<AuthSession, RefreshSessionRequest>(API_ENDPOINTS.auth.refresh, request);
  }

  getCurrentUser(): Observable<CurrentUser> {
    return this.api.get<CurrentUser>(API_ENDPOINTS.auth.currentUser);
  }

  changePassword(request: ChangePasswordRequest): Observable<boolean> {
    return this.api.post<boolean, ChangePasswordRequest>(
      API_ENDPOINTS.auth.changePassword,
      request,
    );
  }

  activateAccount(request: ActivateAccountRequest): Observable<boolean> {
    return this.api.post<boolean, ActivateAccountRequest>(
      API_ENDPOINTS.auth.activateAccount,
      request,
    );
  }

  resetPassword(request: ResetPasswordRequest): Observable<boolean> {
    return this.api.post<boolean, ResetPasswordRequest>(API_ENDPOINTS.auth.resetPassword, request);
  }

  revokeSession(userSessionID: number): Observable<boolean> {
    return this.api.post<boolean, Record<string, never>>(
      API_ENDPOINTS.auth.revokeSession(userSessionID),
      {},
    );
  }
}
