import type { AuthSession } from "../types/auth.types";

const SESSION_STORAGE_KEY = "marketsphere.management.auth.session";
const DEVICE_IDENTIFIER_KEY = "marketsphere.management.device.identifier";
const SESSION_CLOCK_SKEW_MILLISECONDS = 30000;

export const AUTH_SESSION_CHANGED_EVENT = "marketsphere:auth-session-changed";
export const AUTH_SESSION_CLEARED_EVENT = "marketsphere:auth-session-cleared";

export function readAuthSession(): AuthSession | null {
  try {
    const rawValue = localStorage.getItem(SESSION_STORAGE_KEY);

    if (!rawValue) {
      return null;
    }

    const parsed = JSON.parse(rawValue) as Partial<AuthSession>;

    if (
      !parsed.user ||
      typeof parsed.accessToken !== "string" ||
      typeof parsed.refreshToken !== "string" ||
      typeof parsed.accessTokenExpiresAt !== "string" ||
      typeof parsed.refreshTokenExpiresAt !== "string"
    ) {
      clearAuthSession();
      return null;
    }

    return parsed as AuthSession;
  } catch {
    clearAuthSession();
    return null;
  }
}

export function writeAuthSession(session: AuthSession): void {
  localStorage.setItem(SESSION_STORAGE_KEY, JSON.stringify(session));
  window.dispatchEvent(new CustomEvent(AUTH_SESSION_CHANGED_EVENT));
}

export function clearAuthSession(): void {
  localStorage.removeItem(SESSION_STORAGE_KEY);
  window.dispatchEvent(new CustomEvent(AUTH_SESSION_CLEARED_EVENT));
}

export function hasUsableRefreshToken(session: AuthSession | null): boolean {
  if (!session?.refreshToken) {
    return false;
  }

  const expiresAt = Date.parse(session.refreshTokenExpiresAt);
  return (
    Number.isFinite(expiresAt) &&
    expiresAt - SESSION_CLOCK_SKEW_MILLISECONDS > Date.now()
  );
}

export function getDeviceIdentifier(): string {
  const existing = localStorage.getItem(DEVICE_IDENTIFIER_KEY);

  if (existing) {
    return existing;
  }

  const identifier =
    typeof crypto.randomUUID === "function"
      ? crypto.randomUUID()
      : `web-${Date.now()}-${Math.random().toString(36).slice(2, 14)}`;

  localStorage.setItem(DEVICE_IDENTIFIER_KEY, identifier);
  return identifier;
}

export function getDeviceName(): string {
  const userAgentData = (
    navigator as Navigator & {
      userAgentData?: { platform?: string };
    }
  ).userAgentData;
  const platform = userAgentData?.platform || navigator.platform;
  return platform ? `Web - ${platform}` : "Web Browser";
}
