import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';

import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { SystemCheckService } from '../../../../core/services/system-check.service';
import { EmptyStateComponent } from '../../../../shared/components/empty-state.component';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import {
  StatusBadgeComponent,
  type StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';

@Component({
  selector: 'app-system-checks-page',
  standalone: true,
  imports: [
    DatePipe,
    EmptyStateComponent,
    LoadingPanelComponent,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './system-checks-page.component.html',
  styleUrl: './system-checks-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SystemChecksPageComponent {
  private readonly systemChecks = inject(SystemCheckService);

  protected readonly running = this.systemChecks.running;
  protected readonly lastRun = this.systemChecks.lastRun;
  protected readonly errorMessage = signal('');
  protected readonly successMessage = signal('');

  protected readonly totalMatches = computed(
    () => this.lastRun()?.results.reduce((total, item) => total + item.matchCount, 0) ?? 0,
  );

  protected runChecks(): void {
    if (this.running()) {
      return;
    }

    this.errorMessage.set('');
    this.successMessage.set('');

    this.systemChecks.run().subscribe({
      next: (result) => {
        this.successMessage.set(
          `System checks completed. ${result.notificationsCreated} new notifications were created.`,
        );
      },
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected friendlyTitle(checkCode: string, fallback: string): string {
    const titles: Readonly<Record<string, string>> = {
      OverdueTask: 'Overdue CRM tasks',
      ExpiringQuotation: 'Expiring quotations',
      ComplaintSLA: 'Complaint SLA breaches',
      NearExpiryBatch: 'Near-expiry batches',
      InactiveClient: 'Inactive clients',
    };

    return titles[checkCode] ?? fallback;
  }

  protected description(checkCode: string): string {
    const descriptions: Readonly<Record<string, string>> = {
      OverdueTask: 'Finds open CRM tasks whose due time has passed and alerts the assigned user.',
      ExpiringQuotation: 'Finds submitted or reviewed quotations approaching their validity date.',
      ComplaintSLA: 'Finds unresolved complaints that have exceeded their SLA deadline.',
      NearExpiryBatch:
        'Finds available inventory batches approaching expiry and alerts warehouse users.',
      InactiveClient:
        'Finds active clients without a recent order based on the configured inactivity period.',
    };

    return descriptions[checkCode] ?? 'Evaluates the configured business rule.';
  }

  protected resultTone(matchCount: number): StatusBadgeTone {
    return matchCount > 0 ? 'warning' : 'success';
  }
}
