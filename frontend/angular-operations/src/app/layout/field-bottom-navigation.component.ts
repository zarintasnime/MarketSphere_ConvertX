import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

import { NotificationCenterService } from '../core/services/notification-center.service';

@Component({
  selector: 'app-field-bottom-navigation',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './field-bottom-navigation.component.html',
  styleUrl: './field-bottom-navigation.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FieldBottomNavigationComponent {
  private readonly notificationCenter = inject(NotificationCenterService);

  protected readonly unreadCount = this.notificationCenter.unreadCount;
  protected readonly items = [
    { label: 'Home', route: '/field/home', icon: 'H' },
    { label: 'Clients', route: '/field/clients', icon: 'C' },
    { label: 'Visits', route: '/field/visits', icon: 'V' },
    { label: 'Capture', route: '/field/active-visit', icon: '+' },
    { label: 'Alerts', route: '/field/notifications', icon: 'N' },
  ] as const;
}
