import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import type { AuthRouteData } from '../../features/auth/models/auth.model';
import { AuthService } from './auth.service';

export const permissionGuard: CanActivateFn = (route) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const data = route.data as AuthRouteData;
  const permissions = data.permissions ?? [];
  const permissionMatch = data.permissionMatch ?? 'all';

  if (permissions.length === 0) {
    return true;
  }

  const isAllowed =
    permissionMatch === 'any'
      ? authService.hasAnyPermission(permissions)
      : authService.hasEveryPermission(permissions);

  return isAllowed ? true : router.createUrlTree(['/auth/access-denied']);
};
