import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

import type { ApiProblemDetails, ApiValidationErrors } from '../models/api-response.model';

interface ApiErrorPayload extends ApiProblemDetails {
  message?: string;
}

export class ApiError extends Error {
  constructor(
    message: string,
    readonly status: number,
    readonly errorCode: string,
    readonly traceIdentifier: string | null,
    readonly validationErrors: ApiValidationErrors | null,
    readonly retryable: boolean,
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

export function getApiErrorMessage(
  error: unknown,
  fallback = 'The request could not be completed.',
): string {
  if (error instanceof Error && error.message.trim()) {
    return error.message;
  }

  return fallback;
}

export function getApiValidationErrors(error: unknown): ApiValidationErrors | null {
  return error instanceof ApiError ? error.validationErrors : null;
}

export function isApiErrorStatus(error: unknown, status: number): boolean {
  return error instanceof ApiError && error.status === status;
}

function normalizeHttpError(error: HttpErrorResponse): ApiError {
  const payload =
    error.error && typeof error.error === 'object' ? (error.error as ApiErrorPayload) : null;
  const status = error.status;
  const defaultMessage = getStatusMessage(status);
  const message =
    payload?.message ||
    payload?.detail ||
    payload?.title ||
    defaultMessage ||
    error.message ||
    'The request could not be completed.';

  return new ApiError(
    message,
    status,
    payload?.errorCode || `http_${status || 'network'}`,
    payload?.traceIdentifier || null,
    payload?.errors || null,
    status === 0 || status === 408 || status === 429 || status >= 500,
  );
}

function getStatusMessage(status: number): string {
  switch (status) {
    case 0:
      return 'The API is unavailable. Confirm that the backend is running and the HTTPS certificate is trusted.';
    case 400:
      return 'The request is invalid. Review the submitted information.';
    case 401:
      return 'Your session is no longer authorized.';
    case 403:
      return 'You do not have permission to perform this action.';
    case 404:
      return 'The requested record or endpoint was not found.';
    case 408:
      return 'The request timed out. Try again.';
    case 409:
      return 'The request conflicts with the current record state. Refresh the page and try again.';
    case 422:
      return 'The submitted information failed validation.';
    case 429:
      return 'Too many requests were submitted. Wait briefly and try again.';
    default:
      return status >= 500
        ? 'The server could not complete the request. Try again or contact the administrator.'
        : '';
  }
}

export const errorInterceptor: HttpInterceptorFn = (request, next) =>
  next(request).pipe(
    catchError((error: unknown) => {
      if (error instanceof ApiError) {
        return throwError(() => error);
      }

      if (error instanceof HttpErrorResponse) {
        return throwError(() => normalizeHttpError(error));
      }

      return throwError(
        () =>
          new ApiError(
            'An unexpected client error occurred.',
            0,
            'client_error',
            null,
            null,
            false,
          ),
      );
    }),
  );
