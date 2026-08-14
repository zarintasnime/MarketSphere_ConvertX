import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, type Observable } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { StatusBadgeComponent } from '../../../../shared/components/status-badge.component';
import type { Brand } from '../../models/products.model';
import { ProductsApiService } from '../../services/products-api.service';

@Component({
  selector: 'app-brands-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent, StatusBadgeComponent],
  templateUrl: './brands-page.component.html',
  styleUrl: './brands-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BrandsPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(ProductsApiService);
  private readonly auth = inject(AuthService);

  protected readonly brands = signal<readonly Brand[]>([]);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly editingID = signal<number | null>(null);
  protected readonly canManage = computed(() => this.auth.hasPermission('products.brands.manage'));

  protected readonly form = this.fb.nonNullable.group({
    brandCode: ['', [Validators.required, Validators.maxLength(50)]],
    brandName: ['', [Validators.required, Validators.maxLength(150)]],
    ownerCompanyName: [''],
    isCustomerFacing: [true],
    isActive: [true],
  });

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.api
      .getBrands()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (items) => this.brands.set(items),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected edit(item: Brand): void {
    if (!this.canManage()) return;
    this.editingID.set(item.brandID);
    this.form.setValue({
      brandCode: item.brandCode,
      brandName: item.brandName,
      ownerCompanyName: item.ownerCompanyName ?? '',
      isCustomerFacing: item.isCustomerFacing,
      isActive: item.isActive,
    });
  }

  protected save(): void {
    if (!this.canManage() || this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const brandID = this.editingID();
    const value = this.form.getRawValue();
    const request = { ...value, ownerCompanyName: value.ownerCompanyName || null };
    const operation: Observable<number | boolean> = brandID
      ? this.api.updateBrand(brandID, request)
      : this.api.createBrand(request);
    operation.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.reset();
        this.load();
      },
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected toggleStatus(item: Brand): void {
    this.api.setBrandStatus(item.brandID, !item.isActive).subscribe({
      next: () => this.load(),
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected reset(): void {
    this.editingID.set(null);
    this.form.reset({
      brandCode: '',
      brandName: '',
      ownerCompanyName: '',
      isCustomerFacing: true,
      isActive: true,
    });
  }
}

