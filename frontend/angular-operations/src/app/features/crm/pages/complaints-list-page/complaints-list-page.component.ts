import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { createEmptyPagedResult } from '../../../../core/models/paged-result.model';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { PaginationComponent } from '../../../../shared/components/pagination.component';
import { StatusBadgeComponent } from '../../../../shared/components/status-badge.component';
import {
  COMPLAINT_CATEGORY_OPTIONS,
  COMPLAINT_PRIORITY_OPTIONS,
  COMPLAINT_STATUS_OPTIONS,
  ComplaintCategory,
  ComplaintListItem,
  ComplaintPriority,
  ComplaintStatus,
  optionLabel,
} from '../../models/crm.model';
import { CrmApiService } from '../../services/crm-api.service';

@Component({
  selector: 'app-complaints-list-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './complaints-list-page.component.html',
  styleUrl: './complaints-list-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ComplaintsListPageComponent {
  private readonly api = inject(CrmApiService);
  private readonly auth = inject(AuthService);
  protected readonly router = inject(Router);

  protected readonly result = signal(createEmptyPagedResult<ComplaintListItem>(1, 10));
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly search = signal('');
  protected readonly slaBreachedOnly = signal(false);
  protected readonly canManage = computed(() => this.auth.hasPermission('crm.complaints.manage'));

  constructor() {
    this.load();
  }

  protected load(pageNumber = this.result().pageNumber): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.api
      .getComplaints(
        {
          pageNumber,
          pageSize: this.result().pageSize,
          search: this.search(),
          sortBy: 'OpenedAt',
          sortDirection: 'desc',
        },
        this.slaBreachedOnly(),
      )
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected categoryLabel(value: ComplaintCategory): string {
    return optionLabel(COMPLAINT_CATEGORY_OPTIONS, value);
  }
  protected priorityLabel(value: ComplaintPriority): string {
    return optionLabel(COMPLAINT_PRIORITY_OPTIONS, value);
  }
  protected statusLabel(value: ComplaintStatus): string {
    return optionLabel(COMPLAINT_STATUS_OPTIONS, value);
  }
  protected priorityTone(
    value: ComplaintPriority,
  ): 'neutral' | 'info' | 'success' | 'warning' | 'danger' {
    if (value === ComplaintPriority.Critical) return 'danger';
    if (value === ComplaintPriority.High) return 'warning';
    return value === ComplaintPriority.Low ? 'neutral' : 'info';
  }
  protected statusTone(
    value: ComplaintStatus,
  ): 'neutral' | 'info' | 'success' | 'warning' | 'danger' {
    if (value === ComplaintStatus.Resolved || value === ComplaintStatus.Closed) return 'success';
    if (value === ComplaintStatus.Rejected) return 'danger';
    if (value === ComplaintStatus.WaitingForCustomer) return 'warning';
    return value === ComplaintStatus.Open ? 'danger' : 'info';
  }
  protected isBreached(item: ComplaintListItem): boolean {
    if (
      !item.slaDueAt ||
      item.status === ComplaintStatus.Resolved ||
      item.status === ComplaintStatus.Closed
    )
      return false;
    return new Date(item.slaDueAt).getTime() < Date.now();
  }
}
