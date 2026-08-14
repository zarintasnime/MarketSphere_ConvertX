import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, finalize, map, shareReplay } from 'rxjs';

import { ApiResponse, requireApiData } from '../models/api-response.model';

@Injectable({ providedIn: 'root' })
export class ApiClientService {
  private readonly http = inject(HttpClient);
  private readonly inFlightMutations = new Map<string, Observable<unknown>>();

  get<T>(url: string, params?: HttpParams): Observable<T> {
    return this.http.get<ApiResponse<T>>(url, { params }).pipe(map(requireApiData));
  }

  post<TResponse, TBody = unknown>(
    url: string,
    body: TBody,
    context?: HttpContext,
  ): Observable<TResponse> {
    return this.mutate('POST', url, body, () =>
      this.http.post<ApiResponse<TResponse>>(url, body, { context }),
    );
  }

  put<TResponse, TBody = unknown>(
    url: string,
    body: TBody,
    context?: HttpContext,
  ): Observable<TResponse> {
    return this.mutate('PUT', url, body, () =>
      this.http.put<ApiResponse<TResponse>>(url, body, { context }),
    );
  }

  patch<TResponse, TBody = unknown>(
    url: string,
    body: TBody,
    context?: HttpContext,
  ): Observable<TResponse> {
    return this.mutate('PATCH', url, body, () =>
      this.http.patch<ApiResponse<TResponse>>(url, body, { context }),
    );
  }

  delete<T>(url: string, context?: HttpContext): Observable<T> {
    return this.mutate('DELETE', url, null, () =>
      this.http.delete<ApiResponse<T>>(url, { context }),
    );
  }

  getBlob(url: string, params?: HttpParams): Observable<Blob> {
    return this.http.get(url, {
      params,
      responseType: 'blob',
    });
  }

  private mutate<T>(
    method: string,
    url: string,
    body: unknown,
    requestFactory: () => Observable<ApiResponse<T>>,
  ): Observable<T> {
    const fingerprint = this.createFingerprint(method, url, body);
    const existing = this.inFlightMutations.get(fingerprint) as Observable<T> | undefined;

    if (existing) {
      return existing;
    }

    const request$ = requestFactory().pipe(
      map(requireApiData),
      finalize(() => this.inFlightMutations.delete(fingerprint)),
      shareReplay({ bufferSize: 1, refCount: false }),
    );

    this.inFlightMutations.set(fingerprint, request$);
    return request$;
  }

  private createFingerprint(method: string, url: string, body: unknown): string {
    return `${method}:${url}:${this.stableSerialize(body)}`;
  }

  private stableSerialize(value: unknown): string {
    if (value instanceof FormData) {
      const parts: string[] = [];
      value.forEach((entry, key) => {
        const normalized =
          entry instanceof File
            ? `${entry.name}:${entry.size}:${entry.lastModified}`
            : String(entry);
        parts.push(`${key}=${normalized}`);
      });
      return parts.sort().join('&');
    }

    if (Array.isArray(value)) {
      return `[${value.map((item) => this.stableSerialize(item)).join(',')}]`;
    }

    if (value && typeof value === 'object') {
      const record = value as Record<string, unknown>;
      return `{${Object.keys(record)
        .sort()
        .map((key) => `${key}:${this.stableSerialize(record[key])}`)
        .join(',')}}`;
    }

    return JSON.stringify(value) ?? String(value);
  }
}
