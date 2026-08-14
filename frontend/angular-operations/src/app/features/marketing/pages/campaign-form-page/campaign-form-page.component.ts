import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize, type Observable } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { SALES_CHANNEL_OPTIONS } from '../../models/marketing.model';
import { MarketingApiService } from '../../services/marketing-api.service';

@Component({
  selector: 'app-campaign-form-page',
  standalone: true,
  imports: [ReactiveFormsModule, PageHeaderComponent],
  templateUrl: './campaign-form-page.component.html',
  styleUrl: './campaign-form-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CampaignFormPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(MarketingApiService);
  private readonly auth = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  protected readonly saving = signal(false);
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly channelOptions = SALES_CHANNEL_OPTIONS;
  protected readonly campaignID = Number(this.route.snapshot.paramMap.get('campaignID') ?? 0);
  protected readonly form = this.fb.nonNullable.group({
    campaignCode: ['', [Validators.required, Validators.maxLength(50)]],
    campaignTitle: ['', [Validators.required, Validators.maxLength(200)]],
    objective: ['', [Validators.required, Validators.maxLength(1000)]],
    budget: [0, [Validators.required, Validators.min(0)]],
    startDate: ['', Validators.required],
    endDate: ['', Validators.required],
    channel: [0, Validators.required],
    createdByEmployeeID: [
      this.auth.currentUser()?.employeeID ?? 0,
      [Validators.required, Validators.min(1)],
    ],
  });
  constructor() {
    if (this.campaignID) this.load();
  }
  private load(): void {
    this.loading.set(true);
    this.api
      .getCampaign(this.campaignID)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (item) =>
          this.form.setValue({
            campaignCode: item.campaignCode,
            campaignTitle: item.campaignTitle,
            objective: item.objective,
            budget: item.budget,
            startDate: item.startDate,
            endDate: item.endDate,
            channel: item.channel,
            createdByEmployeeID: item.createdByEmployeeID,
          }),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    if (value.endDate < value.startDate) {
      this.errorMessage.set('End date cannot be earlier than start date.');
      return;
    }
    this.saving.set(true);
    const operation: Observable<number | boolean> = this.campaignID
      ? this.api.updateCampaign(this.campaignID, value)
      : this.api.createCampaign(value);
    operation
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: (result) =>
          void this.router.navigate(['/marketing/campaigns', this.campaignID || result]),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}


