import { HttpContextToken, HttpInterceptorFn } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { finalize } from 'rxjs';

import { environment } from '../../../environments/environment';

export const USE_IDEMPOTENCY_KEY = new HttpContextToken<boolean>(() => false);
export const SKIP_IDEMPOTENCY_KEY = new HttpContextToken<boolean>(() => false);
export const IDEMPOTENCY_KEY_HEADER = 'Idempotency-Key';

interface KeyEntry {
  key: string;
  references: number;
}

@Injectable({ providedIn: 'root' })
export class IdempotencyKeyRegistry {
  private readonly keys = new Map<string, KeyEntry>();

  acquire(fingerprint: string): string {
    const existing = this.keys.get(fingerprint);

    if (existing) {
      existing.references += 1;
      return existing.key;
    }

    const key =
      typeof crypto.randomUUID === 'function'
        ? crypto.randomUUID()
        : `web-${Date.now()}-${Math.random().toString(36).slice(2, 14)}`;

    this.keys.set(fingerprint, { key, references: 1 });
    return key;
  }

  release(fingerprint: string): void {
    const entry = this.keys.get(fingerprint);

    if (!entry) {
      return;
    }

    entry.references -= 1;

    if (entry.references <= 0) {
      this.keys.delete(fingerprint);
    }
  }
}

const mutationMethods = new Set(['POST', 'PUT', 'PATCH', 'DELETE']);
const excludedAuthPaths = ['/auth/login', '/auth/refresh'] as const;

export const idempotencyInterceptor: HttpInterceptorFn = (request, next) => {
  const registry = inject(IdempotencyKeyRegistry);
  const apiBaseUrl = environment.apiBaseUrl.replace(/\/$/, '');
  const isApiMutation =
    request.url.startsWith(apiBaseUrl) && mutationMethods.has(request.method.toUpperCase());
  const isExcludedAuthRequest = excludedAuthPaths.some((path) => request.url.endsWith(path));
  const shouldAttach =
    !request.context.get(SKIP_IDEMPOTENCY_KEY) &&
    !isExcludedAuthRequest &&
    (request.context.get(USE_IDEMPOTENCY_KEY) || isApiMutation);

  if (!shouldAttach || request.headers.has(IDEMPOTENCY_KEY_HEADER)) {
    return next(request);
  }

  const fingerprint = createFingerprint(request.method, request.urlWithParams, request.body);
  const key = registry.acquire(fingerprint);

  return next(
    request.clone({
      setHeaders: {
        [IDEMPOTENCY_KEY_HEADER]: key,
      },
    }),
  ).pipe(finalize(() => registry.release(fingerprint)));
};

function createFingerprint(method: string, url: string, body: unknown): string {
  return `${method.toUpperCase()}:${url}:${stableSerialize(body)}`;
}

function stableSerialize(value: unknown): string {
  if (value instanceof FormData) {
    const parts: string[] = [];
    value.forEach((entry, key) => {
      const normalized =
        entry instanceof File ? `${entry.name}:${entry.size}:${entry.lastModified}` : String(entry);
      parts.push(`${key}=${normalized}`);
    });
    return parts.sort().join('&');
  }

  if (Array.isArray(value)) {
    return `[${value.map(stableSerialize).join(',')}]`;
  }

  if (value && typeof value === 'object') {
    const record = value as Record<string, unknown>;
    return `{${Object.keys(record)
      .sort()
      .map((key) => `${key}:${stableSerialize(record[key])}`)
      .join(',')}}`;
  }

  return JSON.stringify(value) ?? String(value);
}
