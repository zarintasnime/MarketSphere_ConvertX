import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';

import { environment } from '../../../environments/environment';
import { AuthService } from '../auth/auth.service';

const publicAuthPaths = [
  '/auth/login',
  '/auth/refresh',
  '/auth/activate-account',
  '/auth/reset-password',
] as const;

function isPublicAuthenticationRequest(url: string): boolean {
  return publicAuthPaths.some((path) => url.endsWith(path));
}

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const authService = inject(AuthService);
  const accessToken = authService.accessToken();
  const apiBaseUrl = environment.apiBaseUrl.replace(/\/$/, '');
  const isApiRequest = request.url.startsWith(apiBaseUrl);

  if (!isApiRequest) {
    return next(request);
  }

  const headers: Record<string, string> = {
    Accept: 'application/json',
    'X-Requested-With': 'XMLHttpRequest',
  };

  if (accessToken && !isPublicAuthenticationRequest(request.url)) {
    headers['Authorization'] = `Bearer ${accessToken}`;
  }

  return next(request.clone({ setHeaders: headers }));
};
