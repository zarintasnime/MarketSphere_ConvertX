import { ApplicationConfig, inject, provideAppInitializer } from '@angular/core';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { provideRouter, withInMemoryScrolling } from '@angular/router';
import { AuthService } from './core/auth/auth.service';
import { authInterceptor } from './core/http/auth.interceptor';
import { errorInterceptor } from './core/http/error.interceptor';
import { idempotencyInterceptor } from './core/http/idempotency.interceptor';
import { refreshTokenInterceptor } from './core/http/refresh-token.interceptor';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(
      routes,
      withInMemoryScrolling({
        scrollPositionRestoration: 'top',
        anchorScrolling: 'enabled',
      }),
    ),
    provideHttpClient(
      withFetch(),
      withInterceptors([
        authInterceptor,
        idempotencyInterceptor,
        errorInterceptor,
        refreshTokenInterceptor,
      ]),
    ),
    provideAppInitializer(() => inject(AuthService).initialize()),
  ],
};
