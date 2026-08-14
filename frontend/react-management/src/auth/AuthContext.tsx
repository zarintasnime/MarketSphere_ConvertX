import {
  type PropsWithChildren,
  useCallback,
  useEffect,
  useMemo,
  useState,
} from "react";

import { getCurrentUser, login as loginRequest } from "../api/authApi";
import type {
  AuthContextValue,
  AuthSession,
  CurrentUser,
  LoginCredentials,
  LoginRequest,
} from "../types/auth.types";
import {
  AUTH_SESSION_CHANGED_EVENT,
  AUTH_SESSION_CLEARED_EVENT,
  clearAuthSession,
  getDeviceIdentifier,
  getDeviceName,
  hasUsableRefreshToken,
  readAuthSession,
  writeAuthSession,
} from "../utils/storage";
import { AuthContext } from "./auth-context";

const SESSION_EXPIRED_EVENT = "marketsphere:session-expired";

function readUsableSession(): AuthSession | null {
  const storedSession = readAuthSession();

  return hasUsableRefreshToken(storedSession) ? storedSession : null;
}

export function AuthProvider({ children }: PropsWithChildren) {
  const [session, setSession] = useState<AuthSession | null>(readUsableSession);

  const [isInitializing, setIsInitializing] = useState<boolean>(
    () => readUsableSession() !== null,
  );

  const synchronizeSession = useCallback(() => {
    setSession(readUsableSession());
  }, []);

  useEffect(() => {
    window.addEventListener(AUTH_SESSION_CHANGED_EVENT, synchronizeSession);

    window.addEventListener(AUTH_SESSION_CLEARED_EVENT, synchronizeSession);

    window.addEventListener(SESSION_EXPIRED_EVENT, synchronizeSession);

    window.addEventListener("storage", synchronizeSession);

    return () => {
      window.removeEventListener(
        AUTH_SESSION_CHANGED_EVENT,
        synchronizeSession,
      );

      window.removeEventListener(
        AUTH_SESSION_CLEARED_EVENT,
        synchronizeSession,
      );

      window.removeEventListener(SESSION_EXPIRED_EVENT, synchronizeSession);

      window.removeEventListener("storage", synchronizeSession);
    };
  }, [synchronizeSession]);

  useEffect(() => {
    let cancelled = false;
    const storedSession = readUsableSession();

    if (!storedSession) {
      clearAuthSession();

      return () => {
        cancelled = true;
      };
    }

    void getCurrentUser()
      .then((user) => {
        if (cancelled) {
          return;
        }

        const latestSession = readUsableSession();

        if (!latestSession) {
          return;
        }

        const nextSession: AuthSession = {
          ...latestSession,
          user,
        };

        writeAuthSession(nextSession);
        setSession(nextSession);
      })
      .catch(() => {
        if (!cancelled) {
          clearAuthSession();
          setSession(null);
        }
      })
      .finally(() => {
        if (!cancelled) {
          setIsInitializing(false);
        }
      });

    return () => {
      cancelled = true;
    };
  }, []);

  const login = useCallback(
    async (credentials: LoginCredentials): Promise<AuthSession> => {
      const request: LoginRequest = {
        email: credentials.email.trim(),
        password: credentials.password,
        deviceIdentifier: getDeviceIdentifier(),
        deviceName: getDeviceName(),
      };

      const nextSession = await loginRequest(request);

      writeAuthSession(nextSession);
      setSession(nextSession);

      return nextSession;
    },
    [],
  );

  const logout = useCallback(() => {
    clearAuthSession();
    setSession(null);
  }, []);

  const refreshCurrentUser =
    useCallback(async (): Promise<CurrentUser | null> => {
      const currentSession = readUsableSession();

      if (!currentSession) {
        return null;
      }

      const user = await getCurrentUser();

      const nextSession: AuthSession = {
        ...currentSession,
        user,
      };

      writeAuthSession(nextSession);
      setSession(nextSession);

      return user;
    }, []);

  const hasPermission = useCallback(
    (permissionCode: string): boolean =>
      session?.user.permissionCodes?.includes(permissionCode) ?? false,
    [session],
  );

  const hasAnyPermission = useCallback(
    (permissionCodes: readonly string[]): boolean =>
      permissionCodes.some((permissionCode) => hasPermission(permissionCode)),
    [hasPermission],
  );

  const hasEveryPermission = useCallback(
    (permissionCodes: readonly string[]): boolean =>
      permissionCodes.every((permissionCode) => hasPermission(permissionCode)),
    [hasPermission],
  );

  const value = useMemo<AuthContextValue>(
    () => ({
      session,
      currentUser: session?.user ?? null,
      isAuthenticated: Boolean(session?.accessToken && session.user),
      isInitializing,
      login,
      logout,
      refreshCurrentUser,
      hasPermission,
      hasAnyPermission,
      hasEveryPermission,
    }),
    [
      session,
      isInitializing,
      login,
      logout,
      refreshCurrentUser,
      hasPermission,
      hasAnyPermission,
      hasEveryPermission,
    ],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
