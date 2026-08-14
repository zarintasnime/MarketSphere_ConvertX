import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, type Observable } from 'rxjs';
import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { createEmptyPagedResult } from '../../../../core/models/paged-result.model';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import type { FeedbackListItem } from '../../models/marketing.model';
import { MarketingApiService } from '../../services/marketing-api.service';
@Component({
  selector: 'app-feedback-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './feedback-page.component.html',
  styleUrl: './feedback-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FeedbackPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(MarketingApiService);
  private readonly auth = inject(AuthService);
  protected readonly result = signal(createEmptyPagedResult<FeedbackListItem>());
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly editingID = signal<number | null>(null);
  protected readonly canManage = computed(() =>
    this.auth.hasPermission('marketing.feedback.manage'),
  );
  protected readonly form = this.fb.nonNullable.group({
    clientID: [null as number | null],
    leadID: [null as number | null],
    campaignID: [null as number | null],
    visitID: [null as number | null],
    submittedByEmployeeID: [this.auth.currentUser()?.employeeID ?? null],
    feedbackType: [0],
    rating: [null as number | null, [Validators.min(1), Validators.max(5)]],
    comments: [''],
    submittedAt: [null as string | null],
    isFollowUpRequired: [false],
  });
  constructor() {
    this.load();
  }
  protected load(): void {
    this.loading.set(true);
    this.api
      .getFeedback({ pageNumber: 1, pageSize: 100, sortDirection: 'desc' })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  protected edit(item: FeedbackListItem): void {
    this.editingID.set(item.feedbackID);
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
      return { ...value, comments: value.comments || null };
    })();
    const id = this.editingID();
    const operation: Observable<number | boolean> = id ? this.api.updateFeedback(id, request) : this.api.createFeedback(request);
    operation.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.reset();
        this.load();
      },
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }
  protected remove(item: FeedbackListItem): void {
    if (!confirm('Delete this record?')) return;
    this.api
      .deleteFeedback(item.feedbackID)
      .subscribe({
        next: () => this.load(),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  protected reset(): void {
    this.editingID.set(null);
    this.form.reset({
      clientID: null,
      leadID: null,
      campaignID: null,
      visitID: null,
      submittedByEmployeeID: this.auth.currentUser()?.employeeID ?? null,
      feedbackType: 0,
      rating: null,
      comments: '',
      submittedAt: null,
      isFollowUpRequired: false,
    });
  }
}

