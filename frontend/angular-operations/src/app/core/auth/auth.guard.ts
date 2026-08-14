import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = (_route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isAuthenticated()) {
    return router.createUrlTree(['/auth/login'], {
      queryParams: { returnUrl: state.url },
    });
  }

  const user = authService.currentUser();
  const isChangePasswordRoute = state.url.startsWith('/auth/change-password');

  if (user?.mustChangePassword && !isChangePasswordRoute) {
    return router.createUrlTree(['/auth/change-password']);
  }

  return true;
};
