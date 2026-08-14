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
  OPPORTUNITY_STAGE_OPTIONS,
  OpportunityListItem,
  OpportunityStage,
  optionLabel,
} from '../../models/crm.model';
import { CrmApiService } from '../../services/crm-api.service';

@Component({
  selector: 'app-opportunities-list-page',
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
  templateUrl: './opportunities-list-page.component.html',
  styleUrl: './opportunities-list-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OpportunitiesListPageComponent {
  private readonly api = inject(CrmApiService);
  private readonly auth = inject(AuthService);
  protected readonly router = inject(Router);
  protected readonly result = signal(createEmptyPagedResult<OpportunityListItem>(1, 10));
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly search = signal('');
  protected readonly canManage = computed(() =>
    this.auth.hasPermission('crm.opportunities.manage'),
  );

  constructor() {
    this.load();
  }
  protected load(pageNumber = this.result().pageNumber): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.api
      .getOpportunities({
        pageNumber,
        pageSize: this.result().pageSize,
        search: this.search(),
        sortBy: 'ExpectedCloseDate',
        sortDirection: 'asc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  protected stageLabel(value: OpportunityStage): string {
    return optionLabel(OPPORTUNITY_STAGE_OPTIONS, value);
  }
  protected stageTone(value: OpportunityStage): StatusBadgeTone {
    return value === OpportunityStage.Won
      ? 'success'
      : value === OpportunityStage.Lost
        ? 'danger'
        : value === OpportunityStage.Commit
          ? 'warning'
          : 'info';
  }
}
