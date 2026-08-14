import { Injectable, inject, signal } from '@angular/core';
import type { Observable } from 'rxjs';
import { finalize, tap } from 'rxjs';

import type { SystemCheckRun } from '../../features/notifications/models/notifications.model';
import { NotificationsApiService } from '../../features/notifications/services/notifications-api.service';
import { NotificationCenterService } from './notification-center.service';

@Injectable({ providedIn: 'root' })
export class SystemCheckService {
  private readonly api = inject(NotificationsApiService);
  private readonly notificationCenter = inject(NotificationCenterService);

  private readonly runningState = signal(false);
  private readonly lastRunState = signal<SystemCheckRun | null>(null);

  readonly running = this.runningState.asReadonly();
  readonly lastRun = this.lastRunState.asReadonly();

  run(): Observable<SystemCheckRun> {
    this.runningState.set(true);

    return this.api.runSystemChecks().pipe(
      tap((result) => {
        this.lastRunState.set(result);
        this.notificationCenter.refreshBadge().subscribe({ error: () => undefined });
      }),
      finalize(() => this.runningState.set(false)),
    );
  }

  clearResult(): void {
    this.lastRunState.set(null);
  }
}
