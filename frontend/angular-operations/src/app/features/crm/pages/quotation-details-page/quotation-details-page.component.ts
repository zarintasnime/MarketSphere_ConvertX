import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { StatusBadgeComponent } from '../../../../shared/components/status-badge.component';
import {
  QUOTATION_STATUS_OPTIONS,
  QuotationDetails,
  QuotationStatus,
  optionLabel,
} from '../../models/crm.model';
import { CrmApiService } from '../../services/crm-api.service';

@Component({
  selector: 'app-quotation-details-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    LoadingPanelComponent,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './quotation-details-page.component.html',
  styleUrl: './quotation-details-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class QuotationDetailsPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(CrmApiService);
  private readonly auth = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);

  protected readonly quotationID = Number(this.route.snapshot.paramMap.get('quotationID'));
  protected readonly details = signal<QuotationDetails | null>(null);
  protected readonly loading = signal(false);
  protected readonly busy = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly statusOptions = QUOTATION_STATUS_OPTIONS;
  protected readonly canManage = computed(() => this.auth.hasPermission('crm.quotations.manage'));
  protected readonly canApprove = computed(() => this.auth.hasPermission('crm.quotations.approve'));
  protected readonly canEditDraft = computed(
    () => this.canManage() && this.details()?.status === QuotationStatus.Draft,
  );
  protected readonly statusForm = this.fb.nonNullable.group({
    status: [QuotationStatus.Submitted],
  });

  constructor() {
    this.load();
  }

  protected createVersion(): void {
    if (!this.canManage()) return;
    this.busy.set(true);
    this.api
      .createQuotationVersion(this.quotationID)
      .pipe(finalize(() => this.busy.set(false)))
      .subscribe({
        next: (quotationID) => void this.router.navigate(['/crm/quotations', quotationID]),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected changeStatus(): void {
    if (!this.canApprove()) return;
    this.busy.set(true);
    this.api
      .changeQuotationStatus(this.quotationID, this.statusForm.controls.status.value)
      .pipe(finalize(() => this.busy.set(false)))
      .subscribe({
        next: () => this.load(),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected statusLabel(value: QuotationStatus): string {
    return optionLabel(QUOTATION_STATUS_OPTIONS, value);
  }
  protected statusTone(
    value: QuotationStatus,
  ): 'neutral' | 'info' | 'success' | 'warning' | 'danger' {
    if (value === QuotationStatus.Accepted || value === QuotationStatus.Converted) return 'success';
    if (value === QuotationStatus.Rejected || value === QuotationStatus.Expired) return 'danger';
    if (value === QuotationStatus.Submitted || value === QuotationStatus.Reviewed) return 'info';
    return 'neutral';
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.api
      .getQuotation(this.quotationID)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (details) => {
          this.details.set(details);
          this.statusForm.controls.status.setValue(details.status);
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}
