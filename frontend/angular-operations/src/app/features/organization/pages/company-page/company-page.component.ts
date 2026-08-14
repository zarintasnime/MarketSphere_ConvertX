import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import type { Company } from '../../models/organization.model';
import { OrganizationApiService } from '../../services/organization-api.service';

@Component({
  selector: 'app-company-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './company-page.component.html',
  styleUrl: './company-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CompanyPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(OrganizationApiService);
  private readonly auth = inject(AuthService);

  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly successMessage = signal('');
  protected readonly company = signal<Company | null>(null);
  protected readonly canManage = computed(() => this.auth.hasPermission('organization.manage'));

  protected readonly form = this.fb.nonNullable.group({
    companyName: ['', [Validators.required, Validators.maxLength(150)]],
    tradeLicenseNo: [''],
    phone: [''],
    email: ['', Validators.email],
    address: [''],
    isActive: [true],
  });

  constructor() {
    this.load();
  }

  protected save(): void {
    const company = this.company();
    if (!company || this.form.invalid || !this.canManage()) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    this.saving.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');
    this.api
      .updateCompany(company.companyID, {
        companyName: value.companyName,
        tradeLicenseNo: value.tradeLicenseNo || null,
        phone: value.phone || null,
        email: value.email || null,
        address: value.address || null,
        isActive: value.isActive,
      })
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => {
          this.successMessage.set('Company updated successfully.');
          this.load(false);
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected load(showSpinner = true): void {
    if (showSpinner) this.loading.set(true);
    this.api
      .getCompanies()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (companies) => {
          const company = companies[0] ?? null;
          this.company.set(company);
          if (company)
            this.form.patchValue({
              companyName: company.companyName,
              tradeLicenseNo: company.tradeLicenseNo ?? '',
              phone: company.phone ?? '',
              email: company.email ?? '',
              address: company.address ?? '',
              isActive: company.isActive,
            });
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}
