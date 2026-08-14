import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';

import { AuthService } from '../core/auth/auth.service';
import { NotificationCenterService } from '../core/services/notification-center.service';
import { FieldBottomNavigationComponent } from './field-bottom-navigation.component';

@Component({
  selector: 'app-field-shell',
  standalone: true,
  imports: [RouterLink, RouterOutlet, FieldBottomNavigationComponent],
  templateUrl: './field-shell.component.html',
  styleUrl: './field-shell.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FieldShellComponent {
  private readonly auth = inject(AuthService);
  private readonly notificationCenter = inject(NotificationCenterService);
  private readonly router = inject(Router);

  protected readonly currentUser = this.auth.currentUser;
  protected readonly unreadCount = this.notificationCenter.unreadCount;

  protected refreshNotifications(): void {
    this.notificationCenter.refreshBadge().subscribe({ error: () => undefined });
  }

  protected logout(): void {
    this.notificationCenter.reset();
    this.auth.logout();
    void this.router.navigate(['/auth/login']);
  }
}
