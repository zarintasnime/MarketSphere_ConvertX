import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { createEmptyPagedResult } from '../../../../core/models/paged-result.model';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { PaginationComponent } from '../../../../shared/components/pagination.component';
import {
  StatusBadgeComponent,
  StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import {
  LEAD_SOURCE_OPTIONS,
  LEAD_STATUS_OPTIONS,
  LEAD_TEMPERATURE_OPTIONS,
  LeadListItem,
  LeadSource,
  LeadStatus,
  LeadTemperature,
  optionLabel,
} from '../../models/crm.model';
import { CrmApiService } from '../../services/crm-api.service';

@Component({
  selector: 'app-leads-list-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './leads-list-page.component.html',
  styleUrl: './leads-list-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LeadsListPageComponent {
  private readonly api = inject(CrmApiService);
  private readonly auth = inject(AuthService);
  protected readonly router = inject(Router);

  protected readonly result = signal(createEmptyPagedResult<LeadListItem>(1, 10));
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly search = signal('');
  protected readonly canManage = computed(() => this.auth.hasPermission('crm.leads.manage'));

  constructor() {
    this.load();
  }

  protected load(pageNumber = this.result().pageNumber): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.api
      .getLeads({
        pageNumber,
        pageSize: this.result().pageSize,
        search: this.search(),
        sortBy: 'NextFollowUpAt',
        sortDirection: 'asc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected sourceLabel(value: LeadSource): string {
    return optionLabel(LEAD_SOURCE_OPTIONS, value);
  }
  protected statusLabel(value: LeadStatus): string {
    return optionLabel(LEAD_STATUS_OPTIONS, value);
  }
  protected temperatureLabel(value: LeadTemperature): string {
    return optionLabel(LEAD_TEMPERATURE_OPTIONS, value);
  }
  protected temperatureTone(value: LeadTemperature): StatusBadgeTone {
    return value === LeadTemperature.Hot
      ? 'danger'
      : value === LeadTemperature.Warm
        ? 'warning'
        : 'info';
  }
}
