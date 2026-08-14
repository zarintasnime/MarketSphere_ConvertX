import axios, { AxiosError, type InternalAxiosRequestConfig } from "axios";

import type {
  ApiProblemDetails,
  ApiResponse,
  ApiValidationErrors,
} from "../types/common.types";
import type { AuthSession, RefreshSessionRequest } from "../types/auth.types";
import {
  clearAuthSession,
  getDeviceIdentifier,
  getDeviceName,
  hasUsableRefreshToken,
  readAuthSession,
  writeAuthSession,
} from "../utils/storage";
const rawApiBaseUrl = import.meta.env.VITE_API_BASE_URL;

if (!rawApiBaseUrl) {
  throw new Error("VITE_API_BASE_URL is not configured.");
}

const API_BASE_URL = rawApiBaseUrl.replace(/\/$/, "");
const SESSION_EXPIRED_EVENT = "marketsphere:session-expired";
const IDEMPOTENCY_HEADER = "Idempotency-Key";

interface ApiErrorPayload extends ApiProblemDetails {
  message?: string;
}

interface RetryableRequestConfig extends InternalAxiosRequestConfig {
  _retry?: boolean;
  _idempotencyFingerprint?: string;
}

interface IdempotencyEntry {
  key: string;
  references: number;
}

const idempotencyEntries = new Map<string, IdempotencyEntry>();

export class ApiClientError extends Error {
  readonly status: number;
  readonly errorCode: string;
  readonly traceIdentifier: string | null;
  readonly validationErrors: ApiValidationErrors | null;
  readonly retryable: boolean;

  constructor(
    message: string,
    status: number,
    errorCode: string,
    traceIdentifier: string | null,
    validationErrors: ApiValidationErrors | null,
    retryable: boolean,
  ) {
    super(message);

    this.name = "ApiClientError";
    this.status = status;
    this.errorCode = errorCode;
    this.traceIdentifier = traceIdentifier;
    this.validationErrors = validationErrors;
    this.retryable = retryable;

    Object.setPrototypeOf(this, ApiClientError.prototype);
  }
}

export function getApiErrorMessage(
  error: unknown,
  fallback = "The request could not be completed.",
): string {
  return error instanceof Error && error.message.trim()
    ? error.message
    : fallback;
}

export function getApiValidationErrors(
  error: unknown,
): ApiValidationErrors | null {
  return error instanceof ApiClientError ? error.validationErrors : null;
}

export function isApiErrorStatus(error: unknown, status: number): boolean {
  return error instanceof ApiClientError && error.status === status;
}

export function requireApiData<T>(response: ApiResponse<T>): T {
  if (!response.succeeded || response.data === null) {
    throw new ApiClientError(
      response.message || "The API response did not contain data.",
      200,
      "invalid_api_response",
      null,
      response.errors,
      false,
    );
  }

  return response.data;
}

function normalizeAxiosError(
  error: AxiosError<ApiErrorPayload>,
): ApiClientError {
  const status = error.response?.status ?? 0;
  const payload = error.response?.data;
  const message =
    payload?.message ||
    payload?.detail ||
    payload?.title ||
    getStatusMessage(status) ||
    error.message ||
    "The request could not be completed.";

  return new ApiClientError(
    message,
    status,
    payload?.errorCode || `http_${status || "network"}`,
    payload?.traceIdentifier || null,
    payload?.errors || null,
    status === 0 || status === 408 || status === 429 || status >= 500,
  );
}

function getStatusMessage(status: number): string {
  switch (status) {
    case 0:
      return "The API is unavailable. Confirm that the backend is running and the HTTPS certificate is trusted.";
    case 400:
      return "The request is invalid. Review the submitted information.";
    case 401:
      return "Your session is no longer authorized.";
    case 403:
      return "You do not have permission to perform this action.";
    case 404:
      return "The requested record or endpoint was not found.";
    case 408:
      return "The request timed out. Try again.";
    case 409:
      return "The request conflicts with the current record state. Refresh the page and try again.";
    case 422:
      return "The submitted information failed validation.";
    case 429:
      return "Too many requests were submitted. Wait briefly and try again.";
    default:
      return status >= 500
        ? "The server could not complete the request. Try again or contact the administrator."
        : "";
  }
}

const httpClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000,
  headers: {
    Accept: "application/json",
    "X-Requested-With": "XMLHttpRequest",
  },
});

const publicAuthPaths = [
  "/auth/login",
  "/auth/refresh",
  "/auth/activate-account",
  "/auth/reset-password",
] as const;

const mutationMethods = new Set(["post", "put", "patch", "delete"]);

httpClient.interceptors.request.use((config) => {
  const session = readAuthSession();
  const isPublicAuthRequest = publicAuthPaths.some((path) =>
    config.url?.endsWith(path),
  );

  if (session?.accessToken && !isPublicAuthRequest) {
    config.headers.set("Authorization", `Bearer ${session.accessToken}`);
  }

  if (config.data instanceof FormData) {
    config.headers.delete("Content-Type");
  }

  if (
    mutationMethods.has((config.method ?? "").toLowerCase()) &&
    !isPublicAuthRequest &&
    !config.headers.has(IDEMPOTENCY_HEADER)
  ) {
    const fingerprint = createRequestFingerprint(config);
    const key = acquireIdempotencyKey(fingerprint);
    config.headers.set(IDEMPOTENCY_HEADER, key);
    (config as RetryableRequestConfig)._idempotencyFingerprint = fingerprint;
  }

  return config;
});

let refreshPromise: Promise<AuthSession> | null = null;

async function requestRefreshedSession(): Promise<AuthSession> {
  const session = readAuthSession();

  if (!session || !hasUsableRefreshToken(session)) {
    throw new ApiClientError(
      "A refresh token is not available.",
      401,
      "refresh_token_unavailable",
      null,
      null,
      false,
    );
  }

  const request: RefreshSessionRequest = {
    refreshToken: session.refreshToken,
    deviceIdentifier: getDeviceIdentifier(),
    deviceName: getDeviceName(),
  };

  const response = await axios.post<ApiResponse<AuthSession>>(
    `${API_BASE_URL}/auth/refresh`,
    request,
    {
      timeout: 30000,
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
    },
  );

  const refreshedSession = requireApiData(response.data);
  writeAuthSession(refreshedSession);
  return refreshedSession;
}

httpClient.interceptors.response.use(
  (response) => {
    releaseRequestIdempotency(response.config as RetryableRequestConfig);
    return response;
  },
  async (error: AxiosError<ApiErrorPayload>) => {
    const config = error.config as RetryableRequestConfig | undefined;
    const isUnauthorized = error.response?.status === 401;
    const isRefreshRequest = config?.url?.endsWith("/auth/refresh") ?? false;

    if (!config || !isUnauthorized || isRefreshRequest || config._retry) {
      releaseRequestIdempotency(config);
      return Promise.reject(normalizeAxiosError(error));
    }

    const session = readAuthSession();

    if (!hasUsableRefreshToken(session)) {
      releaseRequestIdempotency(config);
      clearAuthSession();
      window.dispatchEvent(new CustomEvent(SESSION_EXPIRED_EVENT));
      return Promise.reject(normalizeAxiosError(error));
    }

    config._retry = true;

    try {
      refreshPromise ??= requestRefreshedSession().finally(() => {
        refreshPromise = null;
      });

      const refreshedSession = await refreshPromise;
      config.headers.set(
        "Authorization",
        `Bearer ${refreshedSession.accessToken}`,
      );

      return httpClient(config);
    } catch (refreshError) {
      releaseRequestIdempotency(config);
      clearAuthSession();
      window.dispatchEvent(new CustomEvent(SESSION_EXPIRED_EVENT));
      return Promise.reject(
        refreshError instanceof ApiClientError
          ? refreshError
          : normalizeAxiosError(error),
      );
    }
  },
);

function createRequestFingerprint(config: InternalAxiosRequestConfig): string {
  return `${(config.method ?? "get").toUpperCase()}:${config.url ?? ""}:${stableSerialize(config.params)}:${stableSerialize(config.data)}`;
}

function acquireIdempotencyKey(fingerprint: string): string {
  const existing = idempotencyEntries.get(fingerprint);

  if (existing) {
    existing.references += 1;
    return existing.key;
  }

  const key =
    typeof crypto.randomUUID === "function"
      ? crypto.randomUUID()
      : `web-${Date.now()}-${Math.random().toString(36).slice(2, 14)}`;

  idempotencyEntries.set(fingerprint, { key, references: 1 });
  return key;
}

function releaseRequestIdempotency(
  config: RetryableRequestConfig | undefined,
): void {
  const fingerprint = config?._idempotencyFingerprint;

  if (!fingerprint) {
    return;
  }

  const entry = idempotencyEntries.get(fingerprint);

  if (!entry) {
    return;
  }

  entry.references -= 1;

  if (entry.references <= 0) {
    idempotencyEntries.delete(fingerprint);
  }
}

function stableSerialize(value: unknown): string {
  if (value instanceof FormData) {
    const parts: string[] = [];
    value.forEach((entry, key) => {
      const normalized =
        entry instanceof File
          ? `${entry.name}:${entry.size}:${entry.lastModified}`
          : String(entry);
      parts.push(`${key}=${normalized}`);
    });
    return parts.sort().join("&");
  }

  if (Array.isArray(value)) {
    return `[${value.map(stableSerialize).join(",")}]`;
  }

  if (value && typeof value === "object") {
    const record = value as Record<string, unknown>;
    return `{${Object.keys(record)
      .sort()
      .map((key) => `${key}:${stableSerialize(record[key])}`)
      .join(",")}}`;
  }

  return JSON.stringify(value) ?? String(value);
}

export default httpClient;
