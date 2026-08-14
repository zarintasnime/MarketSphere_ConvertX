import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { AuthService } from '../core/auth/auth.service';
import { NotificationCenterService } from '../core/services/notification-center.service';

@Component({
  selector: 'app-top-header',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './top-header.component.html',
  styleUrl: './top-header.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TopHeaderComponent {
  private readonly authService = inject(AuthService);
  private readonly notificationCenter = inject(NotificationCenterService);
  private readonly router = inject(Router);

  protected readonly user = this.authService.currentUser;
  protected readonly unreadCount = this.notificationCenter.unreadCount;
  protected readonly badgeLoading = this.notificationCenter.badgeLoading;

  protected refreshNotifications(): void {
    this.notificationCenter.refreshBadge().subscribe({ error: () => undefined });
  }

  protected signOut(): void {
    this.notificationCenter.reset();
    this.authService.logout();
    void this.router.navigate(['/auth/login']);
  }
}
