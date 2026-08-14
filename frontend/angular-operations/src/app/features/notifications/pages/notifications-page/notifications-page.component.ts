import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { NotificationCenterService } from '../../../../core/services/notification-center.service';
import { EmptyStateComponent } from '../../../../shared/components/empty-state.component';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import {
  StatusBadgeComponent,
  type StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import {
  NotificationPriority,
  NotificationType,
  type NotificationItem,
  type NotificationViewFilter,
} from '../../models/notifications.model';

@Component({
  selector: 'app-notifications-page',
  standalone: true,
  imports: [
    DatePipe,
    FormsModule,
    EmptyStateComponent,
    LoadingPanelComponent,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './notifications-page.component.html',
  styleUrl: './notifications-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NotificationsPageComponent {
  private readonly center = inject(NotificationCenterService);
  private readonly router = inject(Router);

  protected readonly page = this.center.page;
  protected readonly loading = this.center.pageLoading;
  protected readonly unreadCount = this.center.unreadCount;
  protected readonly errorMessage = signal('');
  protected readonly busyNotificationID = signal<number | null>(null);
  protected readonly markingAll = signal(false);
  protected readonly viewFilter = signal<NotificationViewFilter>('all');

  protected search = '';
  protected pageSize = 20;

  protected readonly visibleItems = computed(() => {
    const filter = this.viewFilter();
    const items = this.page().items;

    if (filter === 'unread') {
      return items.filter((item) => !item.isRead);
    }

    if (filter === 'read') {
      return items.filter((item) => item.isRead);
    }

    return items;
  });

  constructor() {
    this.load(1);
  }

  protected load(pageNumber = this.page().pageNumber || 1): void {
    this.errorMessage.set('');

    this.center
      .loadPage({
        pageNumber,
        pageSize: this.pageSize,
        search: this.search,
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

  protected applySearch(): void {
    this.load(1);
  }

  protected clearSearch(): void {
    this.search = '';
    this.load(1);
  }

  protected setFilter(filter: NotificationViewFilter): void {
    this.viewFilter.set(filter);
  }

  protected markRead(item: NotificationItem): void {
    if (item.isRead || this.busyNotificationID() !== null) {
      return;
    }

    this.busyNotificationID.set(item.notificationID);
    this.errorMessage.set('');

    this.center.markRead(item.notificationID).subscribe({
      next: () => this.busyNotificationID.set(null),
      error: (error: unknown) => {
        this.busyNotificationID.set(null);
        this.errorMessage.set(getApiErrorMessage(error));
      },
    });
  }

  protected markAllRead(): void {
    if (this.markingAll() || this.unreadCount() === 0) {
      return;
    }

    this.markingAll.set(true);
    this.errorMessage.set('');

    this.center.markAllRead().subscribe({
      next: () => this.markingAll.set(false),
      error: (error: unknown) => {
        this.markingAll.set(false);
        this.errorMessage.set(getApiErrorMessage(error));
      },
    });
  }

  protected previousPage(): void {
    if (this.page().pageNumber > 1) {
      this.load(this.page().pageNumber - 1);
    }
  }

  protected nextPage(): void {
    if (this.page().pageNumber < this.page().totalPages) {
      this.load(this.page().pageNumber + 1);
    }
  }

  protected typeLabel(value: NotificationType): string {
    return NotificationType[value] ?? 'Unknown';
  }

  protected priorityLabel(value: NotificationPriority): string {
    return NotificationPriority[value] ?? 'Unknown';
  }

  protected priorityTone(value: NotificationPriority): StatusBadgeTone {
    switch (value) {
      case NotificationPriority.Critical:
        return 'danger';
      case NotificationPriority.High:
        return 'warning';
      case NotificationPriority.Normal:
        return 'info';
      default:
        return 'neutral';
    }
  }

  protected hasReference(item: NotificationItem): boolean {
    return !!this.referenceCommands(item);
  }

  protected openReference(item: NotificationItem): void {
    const commands = this.referenceCommands(item);

    if (commands) {
      void this.router.navigate(commands);
    }
  }

  private referenceCommands(item: NotificationItem): (string | number)[] | null {
    const referenceType = item.referenceType?.trim().toUpperCase();
    const referenceID = item.referenceID;

    if (!referenceType) {
      return null;
    }

    switch (referenceType) {
      case 'CLIENT':
        return referenceID ? ['/crm/clients', referenceID] : ['/crm/clients'];
      case 'QUOTATION':
        return referenceID ? ['/crm/quotations', referenceID] : ['/crm/quotations'];
      case 'COMPLAINT':
        return referenceID ? ['/crm/complaints', referenceID] : ['/crm/complaints'];
      case 'CRM_TASK':
        return ['/crm/tasks'];
      case 'BATCH':
        return ['/inventory/batch-expiry'];
      case 'ORDER':
        return referenceID ? ['/orders', referenceID] : ['/orders/list'];
      case 'DELIVERY':
        return referenceID ? ['/fulfilment/deliveries', referenceID] : ['/fulfilment/deliveries'];
      default:
        return null;
    }
  }
}
