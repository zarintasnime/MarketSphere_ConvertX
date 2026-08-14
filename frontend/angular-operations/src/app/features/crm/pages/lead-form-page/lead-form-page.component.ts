import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize, type Observable } from 'rxjs';

import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { LEAD_SOURCE_OPTIONS, LeadSource } from '../../models/crm.model';
import { CrmApiService } from '../../services/crm-api.service';

@Component({
  selector: 'app-lead-form-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './lead-form-page.component.html',
  styleUrl: './lead-form-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LeadFormPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(CrmApiService);
  private readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);

  protected readonly leadID = Number(this.route.snapshot.paramMap.get('leadID')) || null;
  protected readonly isEdit = computed(() => this.leadID !== null);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly sourceOptions = LEAD_SOURCE_OPTIONS;

  protected readonly form = this.fb.nonNullable.group({
    leadCode: ['', [Validators.required, Validators.maxLength(50)]],
    leadName: ['', [Validators.required, Validators.maxLength(150)]],
    businessName: [''],
    phone: [''],
    email: ['', Validators.email],
    source: [LeadSource.Manual, Validators.required],
    sourceCampaignID: this.fb.control<number | null>(null),
    assignedEmployeeID: this.fb.control<number | null>(null),
    regionID: this.fb.control<number | null>(null),
    productInterest: [''],
    estimatedValue: this.fb.control<number | null>(null),
    nextFollowUpAt: [''],
  });

  constructor() {
    if (this.leadID) this.load();
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const value = this.form.getRawValue();
    const request = {
      ...value,
      businessName: value.businessName || null,
      phone: value.phone || null,
      email: value.email || null,
      productInterest: value.productInterest || null,
      nextFollowUpAt: value.nextFollowUpAt || null,
    };
    const operation: Observable<number | boolean> = this.leadID
      ? this.api.updateLead(this.leadID, request)
      : this.api.createLead(request);
    operation.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: (result) => void this.router.navigate(['/crm/leads', this.leadID ?? Number(result)]),
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  private load(): void {
    this.loading.set(true);
    this.api
      .getLead(this.leadID!)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (item) =>
          this.form.setValue({
            leadCode: item.leadCode,
            leadName: item.leadName,
            businessName: item.businessName ?? '',
            phone: item.phone ?? '',
            email: item.email ?? '',
            source: item.source,
            sourceCampaignID: item.sourceCampaignID,
            assignedEmployeeID: item.assignedEmployeeID,
            regionID: item.regionID,
            productInterest: item.productInterest ?? '',
            estimatedValue: item.estimatedValue,
            nextFollowUpAt: item.nextFollowUpAt?.slice(0, 16) ?? '',
          }),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}

