import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Location } from '@angular/common';
import { Router } from '@angular/router';

import { AuthService } from '../../../../core/auth/auth.service';

@Component({
  selector: 'app-access-denied-page',
  standalone: true,
  templateUrl: './access-denied-page.component.html',
  styleUrl: './access-denied-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AccessDeniedPageComponent {
  private readonly location = inject(Location);
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);

  protected goBack(): void {
    this.location.back();
  }

  protected goToAccount(): void {
    void this.router.navigate(['/auth/change-password']);
  }

  protected signOut(): void {
    this.authService.logout();
    void this.router.navigate(['/auth/login']);
  }
}
