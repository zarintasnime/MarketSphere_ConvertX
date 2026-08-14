import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';

import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { NotificationCenterService } from '../../../../core/services/notification-center.service';
import {
  NotificationPriority,
  type NotificationItem,
} from '../../../notifications/models/notifications.model';

@Component({
  selector: 'app-field-notifications-page',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './field-notifications-page.component.html',
  styleUrl: './field-notifications-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FieldNotificationsPageComponent {
  private readonly center = inject(NotificationCenterService);

  protected readonly page = this.center.page;
  protected readonly loading = this.center.pageLoading;
  protected readonly unreadCount = this.center.unreadCount;
  protected readonly errorMessage = signal('');
  protected readonly busyID = signal<number | null>(null);
  protected readonly showUnreadOnly = signal(false);

  protected readonly items = computed(() =>
    this.showUnreadOnly() ? this.page().items.filter((item) => !item.isRead) : this.page().items,
  );

  constructor() {
    this.load();
  }

  protected load(): void {
    this.errorMessage.set('');

    this.center
      .loadPage({
        pageNumber: 1,
        pageSize: 100,
        sortBy: 'CreatedAt',
        sortDirection: 'desc',
      })
      .subscribe({
        next: () => {
          this.center.refreshBadge().subscribe({ error: () => undefined });
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected toggleUnreadOnly(): void {
    this.showUnreadOnly.update((value) => !value);
  }

  protected markRead(item: NotificationItem): void {
    if (item.isRead || this.busyID() !== null) {
      return;
    }

    this.busyID.set(item.notificationID);
    this.center.markRead(item.notificationID).subscribe({
      next: () => this.busyID.set(null),
      error: (error: unknown) => {
        this.busyID.set(null);
        this.errorMessage.set(getApiErrorMessage(error));
      },
    });
  }

  protected markAllRead(): void {
    this.center.markAllRead().subscribe({
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected priorityLabel(value: NotificationPriority): string {
    return NotificationPriority[value] ?? 'Unknown';
  }
}
