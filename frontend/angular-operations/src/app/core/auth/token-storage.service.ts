import { Injectable } from '@angular/core';

import type { AuthSession } from '../../features/auth/models/auth.model';

const SESSION_STORAGE_KEY = 'marketsphere.auth.session';

@Injectable({ providedIn: 'root' })
export class TokenStorageService {
  readSession(): AuthSession | null {
    try {
      const rawValue = localStorage.getItem(SESSION_STORAGE_KEY);

      if (!rawValue) {
        return null;
      }

      const parsed = JSON.parse(rawValue) as Partial<AuthSession>;

      if (
        !parsed.user ||
        typeof parsed.accessToken !== 'string' ||
        typeof parsed.refreshToken !== 'string' ||
        typeof parsed.accessTokenExpiresAt !== 'string' ||
        typeof parsed.refreshTokenExpiresAt !== 'string'
      ) {
        this.clearSession();
        return null;
      }

      return parsed as AuthSession;
    } catch {
      this.clearSession();
      return null;
    }
  }

  writeSession(session: AuthSession): void {
    localStorage.setItem(SESSION_STORAGE_KEY, JSON.stringify(session));
  }

  clearSession(): void {
    localStorage.removeItem(SESSION_STORAGE_KEY);
  }

  hasUsableRefreshToken(session: AuthSession | null): boolean {
    if (!session?.refreshToken) {
      return false;
    }

    const expiresAt = Date.parse(session.refreshTokenExpiresAt);
    return Number.isFinite(expiresAt) && expiresAt > Date.now();
  }
}
