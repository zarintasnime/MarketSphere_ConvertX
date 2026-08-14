import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, type Observable } from 'rxjs';
import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { createEmptyPagedResult } from '../../../../core/models/paged-result.model';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import type { SamplingLogListItem } from '../../models/marketing.model';
import { MarketingApiService } from '../../services/marketing-api.service';
@Component({
  selector: 'app-sampling-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './sampling-page.component.html',
  styleUrl: './sampling-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SamplingPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(MarketingApiService);
  private readonly auth = inject(AuthService);
  protected readonly result = signal(createEmptyPagedResult<SamplingLogListItem>());
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly editingID = signal<number | null>(null);
  protected readonly canManage = computed(() =>
    this.auth.hasPermission('marketing.sampling.manage'),
  );
  protected readonly form = this.fb.nonNullable.group({
    visitID: [null as number | null],
    campaignID: [null as number | null],
    clientID: [null as number | null],
    leadID: [null as number | null],
    employeeID: [
      this.auth.currentUser()?.employeeID ?? 0,
      [Validators.required, Validators.min(1)],
    ],
    skuID: [0, [Validators.required, Validators.min(1)]],
    issuedQuantity: [0, [Validators.min(0)]],
    consumedQuantity: [0, [Validators.min(0)]],
    returnedQuantity: [0, [Validators.min(0)]],
    damagedQuantity: [0, [Validators.min(0)]],
    sampleDate: [new Date().toISOString().slice(0, 10), Validators.required],
    feedbackSummary: [''],
    outcome: [0],
    followUpRequired: [false],
  });
  constructor() {
    this.load();
  }
  protected load(): void {
    this.loading.set(true);
    this.api
      .getSamplingLogs({ pageNumber: 1, pageSize: 100, sortDirection: 'desc' })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  protected edit(item: SamplingLogListItem): void {
    this.editingID.set(item.samplingLogID);
    this.form.patchValue(item as any);
  }
  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const request = (() => {
      const value = this.form.getRawValue();
      return { ...value, feedbackSummary: value.feedbackSummary || null };
    })();
    const id = this.editingID();
    const operation: Observable<number | boolean> = id
      ? this.api.updateSamplingLog(id, request)
      : this.api.createSamplingLog(request);
    operation.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.reset();
        this.load();
      },
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }
  protected remove(item: SamplingLogListItem): void {
    if (!confirm('Delete this record?')) return;
    this.api
      .deleteSamplingLog(item.samplingLogID)
      .subscribe({
        next: () => this.load(),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  protected reset(): void {
    this.editingID.set(null);
    this.form.reset({
      visitID: null,
      campaignID: null,
      clientID: null,
      leadID: null,
      employeeID: this.auth.currentUser()?.employeeID ?? 0,
      skuID: 0,
      issuedQuantity: 0,
      consumedQuantity: 0,
      returnedQuantity: 0,
      damagedQuantity: 0,
      sampleDate: new Date().toISOString().slice(0, 10),
      feedbackSummary: '',
      outcome: 0,
      followUpRequired: false,
    });
  }
}

