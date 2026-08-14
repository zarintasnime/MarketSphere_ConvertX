import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable, finalize, forkJoin } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { StatusBadgeComponent } from '../../../../shared/components/status-badge.component';
import { Branch, BranchType, Company } from '../../models/organization.model';
import { OrganizationApiService } from '../../services/organization-api.service';

@Component({
  selector: 'app-branches-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent, StatusBadgeComponent],
  templateUrl: './branches-page.component.html',
  styleUrl: './branches-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BranchesPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(OrganizationApiService);
  private readonly auth = inject(AuthService);

  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly companies = signal<readonly Company[]>([]);
  protected readonly branches = signal<readonly Branch[]>([]);
  protected readonly selectedID = signal<number | null>(null);
  protected readonly canManage = computed(() => this.auth.hasPermission('organization.manage'));
  protected readonly branchTypes = [
    BranchType.HeadOffice,
    BranchType.RegionalOffice,
    BranchType.Depot,
  ] as const;

  protected readonly form = this.fb.nonNullable.group({
    companyID: [0, [Validators.required, Validators.min(1)]],
    branchCode: ['', [Validators.required, Validators.maxLength(50)]],
    branchName: ['', [Validators.required, Validators.maxLength(150)]],
    branchType: [BranchType.HeadOffice, Validators.required],
    address: [''],
    phone: [''],
    isActive: [true],
  });

  constructor() {
    this.load();
  }

  protected select(branch: Branch): void {
    this.selectedID.set(branch.branchID);
    this.form.patchValue({
      companyID: branch.companyID,
      branchCode: branch.branchCode,
      branchName: branch.branchName,
      branchType: branch.branchType,
      address: branch.address ?? '',
      phone: branch.phone ?? '',
      isActive: branch.isActive,
    });
    this.form.controls.companyID.disable();
    this.form.controls.branchCode.disable();
  }

  protected resetForm(): void {
    this.selectedID.set(null);
    this.form.controls.companyID.enable();
    this.form.controls.branchCode.enable();
    this.form.reset({
      companyID: this.companies()[0]?.companyID ?? 0,
      branchCode: '',
      branchName: '',
      branchType: BranchType.HeadOffice,
      address: '',
      phone: '',
      isActive: true,
    });
  }

  protected save(): void {
    if (this.form.invalid || !this.canManage()) {
      this.form.markAllAsTouched();
      return;
    }
    const raw = this.form.getRawValue();
    const branchID = this.selectedID();
    let request$: Observable<unknown> = branchID
      ? this.api.updateBranch(branchID, {
          branchName: raw.branchName,
          branchType: raw.branchType,
          address: raw.address || null,
          phone: raw.phone || null,
          isActive: raw.isActive,
        })
      : this.api.createBranch({
          companyID: raw.companyID,
          branchCode: raw.branchCode,
          branchName: raw.branchName,
          branchType: raw.branchType,
          address: raw.address || null,
          phone: raw.phone || null,
        });
    this.saving.set(true);
    this.errorMessage.set('');
    request$.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.resetForm();
        this.load(false);
      },
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected branchTypeName(value: BranchType): string {
    return BranchType[value];
  }

  protected load(showSpinner = true): void {
    if (showSpinner) this.loading.set(true);
    forkJoin({ companies: this.api.getCompanies(), branches: this.api.getBranches() })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: ({ companies, branches }) => {
          this.companies.set(companies);
          this.branches.set(branches);
          if (!this.selectedID())
            this.form.controls.companyID.setValue(companies[0]?.companyID ?? 0);
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}
