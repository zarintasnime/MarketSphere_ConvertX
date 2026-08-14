import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize, forkJoin, of, type Observable } from 'rxjs';

import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import {
  Brand,
  PRODUCT_TYPE_OPTIONS,
  ProductCategoryListItem,
  ProductDetails,
  ProductType,
} from '../../models/products.model';
import { ProductsApiService } from '../../services/products-api.service';

@Component({
  selector: 'app-product-form-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './product-form-page.component.html',
  styleUrl: './product-form-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProductFormPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(ProductsApiService);
  private readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);

  protected readonly productID = Number(this.route.snapshot.paramMap.get('productID')) || null;
  protected readonly isEdit = computed(() => this.productID !== null);
  protected readonly categories = signal<readonly ProductCategoryListItem[]>([]);
  protected readonly brands = signal<readonly Brand[]>([]);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly productTypeOptions = PRODUCT_TYPE_OPTIONS;

  protected readonly form = this.fb.nonNullable.group({
    productCode: ['', [Validators.required, Validators.maxLength(50)]],
    productCategoryID: [0, Validators.min(1)],
    brandID: [0, Validators.min(1)],
    productName: ['', [Validators.required, Validators.maxLength(200)]],
    productType: [ProductType.FinishedGood, Validators.required],
    description: [''],
    requiresBatch: [false],
    requiresExpiryDate: [false],
    isActive: [true],
  });

  constructor() {
    this.load();
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.errorMessage.set('');
    const value = this.form.getRawValue();
    const request = { ...value, description: value.description || null };
    const operation: Observable<number | boolean> = this.productID
      ? this.api.updateProduct(this.productID, request)
      : this.api.createProduct(request);
    operation.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => void this.router.navigate(['/products/list']),
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  private load(): void {
    this.loading.set(true);
    const product$ = this.productID
      ? this.api.getProduct(this.productID)
      : of<ProductDetails | null>(null);
    forkJoin({
      categories: this.api.getCategories(),
      brands: this.api.getBrands(),
      product: product$,
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: ({ categories, brands, product }) => {
          this.categories.set(
            categories.filter(
              (item) => item.isActive || item.productCategoryID === product?.productCategoryID,
            ),
          );
          this.brands.set(
            brands.filter((item) => item.isActive || item.brandID === product?.brandID),
          );
          if (product) {
            this.form.setValue({
              productCode: product.productCode,
              productCategoryID: product.productCategoryID,
              brandID: product.brandID,
              productName: product.productName,
              productType: product.productType,
              description: product.description ?? '',
              requiresBatch: product.requiresBatch,
              requiresExpiryDate: product.requiresExpiryDate,
              isActive: product.isActive,
            });
          }
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}

