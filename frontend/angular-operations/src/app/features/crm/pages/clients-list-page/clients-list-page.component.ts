import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { forkJoin, finalize } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { createEmptyPagedResult } from '../../../../core/models/paged-result.model';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { MetricCardComponent } from '../../../../shared/components/metric-card.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { PaginationComponent } from '../../../../shared/components/pagination.component';
import {
  StatusBadgeComponent,
  StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import {
  CLIENT_LIFECYCLE_OPTIONS,
  CLIENT_RISK_OPTIONS,
  CLIENT_TYPE_OPTIONS,
  SALES_CHANNEL_OPTIONS,
  ClientLifecycleStatus,
  ClientListItem,
  ClientRiskStatus,
  ClientType,
  CrmDashboard,
  SalesChannel,
  optionLabel,
} from '../../models/crm.model';
import { CrmApiService } from '../../services/crm-api.service';

@Component({
  selector: 'app-clients-list-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    LoadingPanelComponent,
    MetricCardComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './clients-list-page.component.html',
  styleUrl: './clients-list-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ClientsListPageComponent {
  private readonly api = inject(CrmApiService);
  private readonly auth = inject(AuthService);
  protected readonly router = inject(Router);

  protected readonly result = signal(createEmptyPagedResult<ClientListItem>(1, 10));
  protected readonly dashboard = signal<CrmDashboard | null>(null);
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly search = signal('');
  protected readonly canManage = computed(() => this.auth.hasPermission('crm.clients.manage'));

  constructor() {
    this.load();
  }

  protected load(pageNumber = this.result().pageNumber): void {
    this.loading.set(true);
    this.errorMessage.set('');
    const clients$ = this.api.getClients({
      pageNumber,
      pageSize: this.result().pageSize,
      search: this.search(),
      sortBy: 'ClientName',
      sortDirection: 'asc',
    });
    const dashboard$ = this.api.getDashboard();
    forkJoin({ clients: clients$, dashboard: dashboard$ })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: ({ clients, dashboard }) => {
          this.result.set(clients);
          this.dashboard.set(dashboard);
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected clientTypeLabel(value: ClientType): string {
    return optionLabel(CLIENT_TYPE_OPTIONS, value);
  }
  protected channelLabel(value: SalesChannel): string {
    return optionLabel(SALES_CHANNEL_OPTIONS, value);
  }
  protected lifecycleLabel(value: ClientLifecycleStatus): string {
    return optionLabel(CLIENT_LIFECYCLE_OPTIONS, value);
  }
  protected riskLabel(value: ClientRiskStatus): string {
    return optionLabel(CLIENT_RISK_OPTIONS, value);
  }
  protected lifecycleTone(value: ClientLifecycleStatus): StatusBadgeTone {
    return value === ClientLifecycleStatus.Active
      ? 'success'
      : value === ClientLifecycleStatus.Churned
        ? 'danger'
        : value === ClientLifecycleStatus.ReactivationInProgress
          ? 'warning'
          : 'neutral';
  }
  protected riskTone(value: ClientRiskStatus): StatusBadgeTone {
    return value === ClientRiskStatus.Normal
      ? 'success'
      : value === ClientRiskStatus.Watch
        ? 'warning'
        : 'danger';
  }
}
