import { Routes } from '@angular/router';

import { authGuard } from '../../core/auth/auth.guard';
import { AccessDeniedPageComponent } from './pages/access-denied-page/access-denied-page.component';
import { ChangePasswordPageComponent } from './pages/change-password-page/change-password-page.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';

export const AUTH_ROUTES: Routes = [
  {
    path: 'login',
    component: LoginPageComponent,
    title: 'Sign In | MarketSphere ConvertX',
  },
  {
    path: 'change-password',
    component: ChangePasswordPageComponent,
    canActivate: [authGuard],
    title: 'Account Security | MarketSphere ConvertX',
  },
  {
    path: 'access-denied',
    component: AccessDeniedPageComponent,
    canActivate: [authGuard],
    title: 'Access Denied | MarketSphere ConvertX',
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'login',
  },
];
