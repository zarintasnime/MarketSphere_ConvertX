import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, type Observable } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { StatusBadgeComponent } from '../../../../shared/components/status-badge.component';
import {
  PRODUCT_CATEGORY_TYPE_OPTIONS,
  ProductCategoryListItem,
  ProductCategoryType,
  optionLabel,
} from '../../models/products.model';
import { ProductsApiService } from '../../services/products-api.service';

@Component({
  selector: 'app-categories-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent, StatusBadgeComponent],
  templateUrl: './categories-page.component.html',
  styleUrl: './categories-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CategoriesPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(ProductsApiService);
  private readonly auth = inject(AuthService);

  protected readonly categories = signal<readonly ProductCategoryListItem[]>([]);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly editingID = signal<number | null>(null);
  protected readonly canManage = computed(() =>
    this.auth.hasPermission('products.categories.manage'),
  );
  protected readonly typeOptions = PRODUCT_CATEGORY_TYPE_OPTIONS;

  protected readonly form = this.fb.nonNullable.group({
    parentProductCategoryID: this.fb.control<number | null>(null),
    categoryCode: ['', [Validators.required, Validators.maxLength(50)]],
    categoryName: ['', [Validators.required, Validators.maxLength(150)]],
    categoryType: [ProductCategoryType.Standard, Validators.required],
    isActive: [true],
  });

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.api
      .getCategories()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (items) => this.categories.set(items),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected edit(item: ProductCategoryListItem): void {
    if (!this.canManage()) return;
    this.editingID.set(item.productCategoryID);
    this.form.setValue({
      parentProductCategoryID: item.parentProductCategoryID,
      categoryCode: item.categoryCode,
      categoryName: item.categoryName,
      categoryType: item.categoryType,
      isActive: item.isActive,
    });
  }

  protected save(): void {
    if (!this.canManage() || this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.errorMessage.set('');
    const categoryID = this.editingID();
    const request = this.form.getRawValue();
    const operation: Observable<number | boolean> = categoryID
      ? this.api.updateCategory(categoryID, request)
      : this.api.createCategory(request);

    operation.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.reset();
        this.load();
      },
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected toggleStatus(item: ProductCategoryListItem): void {
    if (!this.canManage()) return;
    this.api.setCategoryStatus(item.productCategoryID, !item.isActive).subscribe({
      next: () => this.load(),
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected reset(): void {
    this.editingID.set(null);
    this.form.reset({
      parentProductCategoryID: null,
      categoryCode: '',
      categoryName: '',
      categoryType: ProductCategoryType.Standard,
      isActive: true,
    });
  }

  protected typeLabel(value: ProductCategoryType): string {
    return optionLabel(this.typeOptions, value);
  }
}

