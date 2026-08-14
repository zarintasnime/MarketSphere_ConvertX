import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { FEEDBACK_TYPE_OPTIONS } from '../../../marketing/models/marketing.model';
import { FieldOperationsApiService } from '../../services/field-operations-api.service';
@Component({
  selector: 'app-feedback-capture-page',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './feedback-capture-page.component.html',
  styleUrl: './feedback-capture-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FeedbackCapturePageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(FieldOperationsApiService);
  private readonly auth = inject(AuthService);
  protected readonly options = FEEDBACK_TYPE_OPTIONS;
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly successMessage = signal('');
  protected readonly form = this.fb.nonNullable.group({
    clientID: [null as number | null],
    leadID: [null as number | null],
    campaignID: [null as number | null],
    visitID: [null as number | null],
    feedbackType: [0],
    rating: [null as number | null, [Validators.min(1), Validators.max(5)]],
    comments: ['', Validators.required],
    isFollowUpRequired: [false],
  });
  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const employeeID = this.auth.currentUser()?.employeeID ?? null;
    const value = this.form.getRawValue();
    this.saving.set(true);
    this.api
      .createFeedback({
        ...value,
        submittedByEmployeeID: employeeID,
        comments: value.comments || null,
        submittedAt: new Date().toISOString(),
      })
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => {
          this.successMessage.set('Feedback was submitted.');
          this.form.reset({
            clientID: null,
            leadID: null,
            campaignID: null,
            visitID: null,
            feedbackType: 0,
            rating: null,
            comments: '',
            isFollowUpRequired: false,
          });
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}
