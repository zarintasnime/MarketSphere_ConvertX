import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';

import { environment } from '../../../environments/environment';
import { API_ENDPOINTS } from '../config/api-endpoints';
import { AuthService } from '../auth/auth.service';

const nonRefreshablePaths = [
  '/auth/login',
  '/auth/refresh',
  '/auth/activate-account',
  '/auth/reset-password',
] as const;

export const refreshTokenInterceptor: HttpInterceptorFn = (request, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const apiBaseUrl = environment.apiBaseUrl.replace(/\/$/, '');

  return next(request).pipe(
    catchError((error: unknown) => {
      const isUnauthorized = error instanceof HttpErrorResponse && error.status === 401;
      const isApiRequest = request.url.startsWith(apiBaseUrl);
      const cannotRefreshRequest = nonRefreshablePaths.some((path) => request.url.endsWith(path));

      if (
        !isUnauthorized ||
        !isApiRequest ||
        cannotRefreshRequest ||
        !authService.canRefreshSession()
      ) {
        return throwError(() => error);
      }

      return authService.refreshSession().pipe(
        switchMap((session) =>
          next(
            request.clone({
              setHeaders: {
                Authorization: `Bearer ${session.accessToken}`,
              },
            }),
          ),
        ),
        catchError((refreshError: unknown) => {
          authService.clearSession();
          const returnUrl = router.url.startsWith('/auth/') ? '/' : router.url;

          void router.navigate(['/auth/login'], {
            queryParams: {
              sessionExpired: 'true',
              returnUrl,
            },
          });

          return throwError(() => refreshError);
        }),
      );
    }),
  );
};
