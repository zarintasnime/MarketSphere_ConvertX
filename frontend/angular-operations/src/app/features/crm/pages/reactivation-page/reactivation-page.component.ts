import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { createEmptyPagedResult } from '../../../../core/models/paged-result.model';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { PaginationComponent } from '../../../../shared/components/pagination.component';
import { StatusBadgeComponent } from '../../../../shared/components/status-badge.component';
import {
  REACTIVATION_RESULT_OPTIONS,
  REACTIVATION_STATUS_OPTIONS,
  ReactivationCase,
  ReactivationCaseStatus,
  ReactivationResult,
  optionLabel,
} from '../../models/crm.model';
import { CrmApiService } from '../../services/crm-api.service';

@Component({
  selector: 'app-reactivation-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './reactivation-page.component.html',
  styleUrl: './reactivation-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ReactivationPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(CrmApiService);
  private readonly auth = inject(AuthService);

  protected readonly result = signal(createEmptyPagedResult<ReactivationCase>(1, 10));
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly search = signal('');
  protected readonly createOpen = signal(false);
  protected readonly resolvingCase = signal<ReactivationCase | null>(null);
  protected readonly canManage = computed(() => this.auth.hasPermission('crm.reactivation.manage'));
  protected readonly statusOptions = REACTIVATION_STATUS_OPTIONS;
  protected readonly resultOptions = REACTIVATION_RESULT_OPTIONS;

  protected readonly createForm = this.fb.nonNullable.group({
    clientID: [0, Validators.min(1)],
    inactiveAt: ['', Validators.required],
    churnReason: [''],
    campaignID: this.fb.control<number | null>(null),
    assignedEmployeeID: [0, Validators.min(1)],
  });
  protected readonly resolveForm = this.fb.nonNullable.group({
    status: [ReactivationCaseStatus.Successful],
    reactivationResult: this.fb.control<ReactivationResult | null>(ReactivationResult.Reordered),
    repeatOrderID: this.fb.control<number | null>(null),
  });

  constructor() {
    this.load();
  }

  protected load(pageNumber = this.result().pageNumber): void {
    this.loading.set(true);
    this.api
      .getReactivationCases({
        pageNumber,
        pageSize: this.result().pageSize,
        search: this.search(),
        sortBy: 'OpenedAt',
        sortDirection: 'desc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected create(): void {
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const value = this.createForm.getRawValue();
    this.api
      .createReactivationCase({ ...value, churnReason: value.churnReason || null })
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => {
          this.createOpen.set(false);
          this.createForm.reset({
            clientID: 0,
            inactiveAt: '',
            churnReason: '',
            campaignID: null,
            assignedEmployeeID: 0,
          });
          this.load(1);
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected openResolve(item: ReactivationCase): void {
    this.resolvingCase.set(item);
    this.resolveForm.setValue({
      status: item.status,
      reactivationResult: item.reactivationResult,
      repeatOrderID: item.repeatOrderID,
    });
  }

  protected resolve(): void {
    const item = this.resolvingCase();
    if (!item) return;
    this.saving.set(true);
    this.api
      .resolveReactivationCase(item.reactivationCaseID, this.resolveForm.getRawValue())
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => {
          this.resolvingCase.set(null);
          this.load();
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected statusLabel(value: ReactivationCaseStatus): string {
    return optionLabel(REACTIVATION_STATUS_OPTIONS, value);
  }
  protected resultLabel(value: ReactivationResult | null): string {
    return value ? optionLabel(REACTIVATION_RESULT_OPTIONS, value) : 'Pending';
  }
  protected statusTone(
    value: ReactivationCaseStatus,
  ): 'neutral' | 'info' | 'success' | 'warning' | 'danger' {
    if (value === ReactivationCaseStatus.Successful) return 'success';
    if (value === ReactivationCaseStatus.Unsuccessful) return 'danger';
    if (value === ReactivationCaseStatus.Closed) return 'neutral';
    return value === ReactivationCaseStatus.Open ? 'warning' : 'info';
  }
}
