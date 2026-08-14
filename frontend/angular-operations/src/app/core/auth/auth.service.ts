import { Injectable, computed, inject, signal } from '@angular/core';
import {
  Observable,
  catchError,
  finalize,
  firstValueFrom,
  map,
  of,
  shareReplay,
  tap,
  throwError,
} from 'rxjs';

import { AuthApiService } from '../../features/auth/services/auth-api.service';
import type {
  AuthSession,
  ChangePasswordRequest,
  LoginCredentials,
  LoginRequest,
  RefreshSessionRequest,
} from '../../features/auth/models/auth.model';
import { DeviceIdentifierService } from './device-identifier.service';
import { TokenStorageService } from './token-storage.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly authApi = inject(AuthApiService);
  private readonly tokenStorage = inject(TokenStorageService);
  private readonly deviceIdentifier = inject(DeviceIdentifierService);

  private readonly sessionState = signal<AuthSession | null>(this.tokenStorage.readSession());

  private readonly initializingState = signal(true);
  private refreshRequest$: Observable<AuthSession> | null = null;

  readonly session = this.sessionState.asReadonly();
  readonly isInitializing = this.initializingState.asReadonly();
  readonly currentUser = computed(() => this.sessionState()?.user ?? null);
  readonly accessToken = computed(() => this.sessionState()?.accessToken ?? null);
  readonly isAuthenticated = computed(
    () => this.currentUser() !== null && this.accessToken() !== null,
  );

  async initialize(): Promise<void> {
    const session = this.sessionState();

    if (!this.tokenStorage.hasUsableRefreshToken(session)) {
      this.clearSession();
      this.initializingState.set(false);
      return;
    }

    await firstValueFrom(
      this.authApi.getCurrentUser().pipe(
        tap((user) => {
          const currentSession = this.sessionState();

          if (currentSession) {
            this.saveSession({ ...currentSession, user });
          }
        }),
        catchError(() => {
          this.clearSession();
          return of(null);
        }),
        finalize(() => this.initializingState.set(false)),
      ),
    );
  }

  login(credentials: LoginCredentials): Observable<AuthSession> {
    const request: LoginRequest = {
      email: credentials.email.trim(),
      password: credentials.password,
      deviceIdentifier: this.deviceIdentifier.getIdentifier(),
      deviceName: this.deviceIdentifier.getDeviceName(),
    };

    return this.authApi.login(request).pipe(tap((session) => this.saveSession(session)));
  }

  refreshSession(): Observable<AuthSession> {
    if (this.refreshRequest$) {
      return this.refreshRequest$;
    }

    const session = this.sessionState();

    if (!this.tokenStorage.hasUsableRefreshToken(session) || !session) {
      return throwError(() => new Error('A refresh token is not available.'));
    }

    const request: RefreshSessionRequest = {
      refreshToken: session.refreshToken,
      deviceIdentifier: this.deviceIdentifier.getIdentifier(),
      deviceName: this.deviceIdentifier.getDeviceName(),
    };

    const refreshRequest$ = this.authApi.refreshSession(request).pipe(
      tap((refreshedSession) => this.saveSession(refreshedSession)),
      finalize(() => {
        this.refreshRequest$ = null;
      }),
      shareReplay({ bufferSize: 1, refCount: false }),
    );

    this.refreshRequest$ = refreshRequest$;
    return refreshRequest$;
  }

  changePassword(request: ChangePasswordRequest): Observable<boolean> {
    return this.authApi.changePassword(request).pipe(
      tap(() => {
        const session = this.sessionState();

        if (session) {
          this.saveSession({
            ...session,
            user: {
              ...session.user,
              mustChangePassword: false,
            },
          });
        }
      }),
    );
  }

  refreshCurrentUser(): Observable<void> {
    return this.authApi.getCurrentUser().pipe(
      tap((user) => {
        const session = this.sessionState();

        if (session) {
          this.saveSession({ ...session, user });
        }
      }),
      map(() => undefined),
    );
  }

  canRefreshSession(): boolean {
    return this.tokenStorage.hasUsableRefreshToken(this.sessionState());
  }

  hasPermission(permissionCode: string): boolean {
    return this.currentUser()?.permissionCodes.includes(permissionCode) ?? false;
  }

  hasAnyPermission(permissionCodes: readonly string[]): boolean {
    return permissionCodes.some((permissionCode) => this.hasPermission(permissionCode));
  }

  hasEveryPermission(permissionCodes: readonly string[]): boolean {
    return permissionCodes.every((permissionCode) => this.hasPermission(permissionCode));
  }

  logout(): void {
    this.clearSession();
  }

  clearSession(): void {
    this.tokenStorage.clearSession();
    this.sessionState.set(null);
  }

  private saveSession(session: AuthSession): void {
    this.tokenStorage.writeSession(session);
    this.sessionState.set(session);
  }
}
