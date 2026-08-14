import { Injectable, computed, effect, inject, signal } from '@angular/core';
import type { Observable } from 'rxjs';
import { finalize, tap } from 'rxjs';

import { AuthService } from '../auth/auth.service';
import {
  createEmptyPagedResult,
  type PagedRequest,
  type PagedResult,
} from '../models/paged-result.model';
import type { NotificationItem } from '../../features/notifications/models/notifications.model';
import { NotificationsApiService } from '../../features/notifications/services/notifications-api.service';

@Injectable({ providedIn: 'root' })
export class NotificationCenterService {
  private readonly api = inject(NotificationsApiService);
  private readonly auth = inject(AuthService);

  private readonly pageState = signal(createEmptyPagedResult<NotificationItem>(1, 20));
  private readonly unreadCountState = signal(0);
  private readonly pageLoadingState = signal(false);
  private readonly badgeLoadingState = signal(false);

  readonly page = this.pageState.asReadonly();
  readonly unreadCount = this.unreadCountState.asReadonly();
  readonly pageLoading = this.pageLoadingState.asReadonly();
  readonly badgeLoading = this.badgeLoadingState.asReadonly();
  readonly hasUnread = computed(() => this.unreadCountState() > 0);

  constructor() {
    effect(() => {
      const user = this.auth.currentUser();
      const canView = this.auth.hasPermission('infrastructure.notifications.view');

      if (!user || !canView) {
        this.reset();
        return;
      }

      this.refreshBadge().subscribe({ error: () => undefined });
    });
  }

  loadPage(request: PagedRequest): Observable<PagedResult<NotificationItem>> {
    this.pageLoadingState.set(true);

    return this.api.getMine(request).pipe(
      tap((result) => this.pageState.set(result)),
      finalize(() => this.pageLoadingState.set(false)),
    );
  }

  refreshBadge(): Observable<PagedResult<NotificationItem>> {
    this.badgeLoadingState.set(true);

    return this.api
      .getMine({
        pageNumber: 1,
        pageSize: 200,
        sortBy: 'CreatedAt',
        sortDirection: 'desc',
      })
      .pipe(
        tap((result) => {
          this.unreadCountState.set(result.items.filter((item) => !item.isRead).length);
        }),
        finalize(() => this.badgeLoadingState.set(false)),
      );
  }

  markRead(notificationID: number): Observable<boolean> {
    return this.api.markRead(notificationID).pipe(
      tap(() => {
        const currentPage = this.pageState();
        const existing = currentPage.items.find((item) => item.notificationID === notificationID);

        if (existing && !existing.isRead) {
          this.pageState.set({
            ...currentPage,
            items: currentPage.items.map((item) =>
              item.notificationID === notificationID
                ? {
                    ...item,
                    isRead: true,
                    readAt: new Date().toISOString(),
                  }
                : item,
            ),
          });
          this.unreadCountState.update((count) => Math.max(0, count - 1));
        }
      }),
    );
  }

  markAllRead(): Observable<boolean> {
    return this.api.markAllRead().pipe(
      tap(() => {
        const currentPage = this.pageState();
        const readAt = new Date().toISOString();

        this.pageState.set({
          ...currentPage,
          items: currentPage.items.map((item) => ({
            ...item,
            isRead: true,
            readAt: item.readAt ?? readAt,
          })),
        });
        this.unreadCountState.set(0);
      }),
    );
  }

  reset(): void {
    this.pageState.set(createEmptyPagedResult<NotificationItem>(1, 20));
    this.unreadCountState.set(0);
    this.pageLoadingState.set(false);
    this.badgeLoadingState.set(false);
  }
}
