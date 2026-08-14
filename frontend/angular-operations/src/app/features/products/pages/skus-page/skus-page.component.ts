import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, forkJoin, type Observable } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { createEmptyPagedResult } from '../../../../core/models/paged-result.model';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { PaginationComponent } from '../../../../shared/components/pagination.component';
import { StatusBadgeComponent } from '../../../../shared/components/status-badge.component';
import type { ProductListItem, SKUListItem } from '../../models/products.model';
import { ProductsApiService } from '../../services/products-api.service';

@Component({
  selector: 'app-skus-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './skus-page.component.html',
  styleUrl: './skus-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SkusPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(ProductsApiService);
  private readonly auth = inject(AuthService);

  protected readonly result = signal(createEmptyPagedResult<SKUListItem>(1, 10));

  protected readonly products = signal<readonly ProductListItem[]>([]);

  protected readonly loading = signal(false);
  protected readonly editorLoading = signal(false);
  protected readonly saving = signal(false);

  protected readonly listErrorMessage = signal('');
  protected readonly editorErrorMessage = signal('');

  protected readonly search = signal('');
  protected readonly editingID = signal<number | null>(null);

  protected readonly editorOpen = signal(false);

  protected readonly canManage = computed(() => this.auth.hasPermission('products.skus.manage'));

  protected readonly form = this.fb.nonNullable.group({
    productID: [0, Validators.min(1)],
    skuCode: ['', [Validators.required, Validators.maxLength(50)]],
    skuName: ['', [Validators.required, Validators.maxLength(200)]],
    size: [''],
    unit: ['', [Validators.required, Validators.maxLength(50)]],
    barcode: [''],
    mrp: [0, Validators.min(0)],
    standardTradePrice: [0, Validators.min(0)],
    isActive: [true],
  });

  constructor() {
    this.load();
  }

  protected load(pageNumber = this.result().pageNumber): void {
    this.loading.set(true);
    this.listErrorMessage.set('');

    this.api
      .getSKUs({
        pageNumber,
        pageSize: this.result().pageSize,
        search: this.search().trim(),
        sortBy: 'SKUName',
        sortDirection: 'asc',
      })
      .pipe(
        finalize(() => {
          this.loading.set(false);
        }),
      )
      .subscribe({
        next: (result) => {
          this.result.set(result);
        },
        error: (error: unknown) => {
          this.listErrorMessage.set(getApiErrorMessage(error, 'Unable to load SKUs.'));
        },
      });
  }

  protected openCreate(): void {
    if (!this.canManage()) {
      return;
    }

    this.resetForm();
    this.editorErrorMessage.set('');
    this.editorOpen.set(true);

    this.loadProductsForEditor();
  }

  protected edit(item: SKUListItem): void {
    if (!this.canManage()) {
      return;
    }

    const skuID = Number(item.skuID);

    if (!Number.isInteger(skuID) || skuID < 1) {
      this.listErrorMessage.set('The selected SKU has an invalid identifier.');

      return;
    }

    this.resetForm();
    this.editingID.set(skuID);
    this.editorOpen.set(true);
    this.editorLoading.set(true);
    this.editorErrorMessage.set('');

    forkJoin({
      productResult: this.api.getProducts({
        pageNumber: 1,
        pageSize: 200,
        sortBy: 'ProductName',
        sortDirection: 'asc',
      }),

      details: this.api.getSKU(skuID),
    })
      .pipe(
        finalize(() => {
          this.editorLoading.set(false);
        }),
      )
      .subscribe({
        next: ({ productResult, details }) => {
          this.products.set(
            productResult.items.filter(
              (product) => product.isActive || product.productID === details.productID,
            ),
          );

          this.form.setValue({
            productID: details.productID,
            skuCode: details.skuCode,
            skuName: details.skuName,
            size: details.size ?? '',
            unit: details.unit,
            barcode: details.barcode ?? '',
            mrp: details.mrp,
            standardTradePrice: details.standardTradePrice,
            isActive: details.isActive,
          });
        },
        error: (error: unknown) => {
          this.editorErrorMessage.set(getApiErrorMessage(error, 'Unable to load the SKU editor.'));
        },
      });
  }

  protected save(): void {
    this.editorErrorMessage.set('');

    if (this.form.invalid) {
      this.form.markAllAsTouched();

      const productControl = this.form.controls.productID;

      if (productControl.invalid) {
        this.editorErrorMessage.set('Select a valid product.');

        return;
      }

      this.editorErrorMessage.set('Complete all required SKU fields correctly.');

      return;
    }

    this.saving.set(true);

    const skuID = this.editingID();
    const value = this.form.getRawValue();

    const request = {
      ...value,
      size: value.size.trim() || null,
      barcode: value.barcode.trim() || null,
    };

    const operation: Observable<number | boolean> = skuID
      ? this.api.updateSKU(skuID, request)
      : this.api.createSKU(request);

    operation
      .pipe(
        finalize(() => {
          this.saving.set(false);
        }),
      )
      .subscribe({
        next: () => {
          this.editorOpen.set(false);
          this.resetForm();
          this.load(1);
        },
        error: (error: unknown) => {
          this.editorErrorMessage.set(getApiErrorMessage(error, 'Unable to save the SKU.'));
        },
      });
  }

  protected toggleStatus(item: SKUListItem): void {
    const skuID = Number(item.skuID);

    if (!Number.isInteger(skuID) || skuID < 1) {
      this.listErrorMessage.set('The selected SKU has an invalid identifier.');

      return;
    }

    this.listErrorMessage.set('');

    this.api.setSKUStatus(skuID, !item.isActive).subscribe({
      next: () => {
        this.load();
      },
      error: (error: unknown) => {
        this.listErrorMessage.set(getApiErrorMessage(error, 'Unable to change the SKU status.'));
      },
    });
  }

  protected closeEditor(): void {
    this.editorOpen.set(false);
    this.editorErrorMessage.set('');
    this.resetForm();
  }

  private loadProductsForEditor(): void {
    this.editorLoading.set(true);
    this.editorErrorMessage.set('');

    this.api
      .getProducts({
        pageNumber: 1,
        pageSize: 200,
        sortBy: 'ProductName',
        sortDirection: 'asc',
      })
      .pipe(
        finalize(() => {
          this.editorLoading.set(false);
        }),
      )
      .subscribe({
        next: (result) => {
          this.products.set(result.items.filter((item) => item.isActive));
        },
        error: (error: unknown) => {
          this.editorErrorMessage.set(
            getApiErrorMessage(error, 'Unable to load the product list.'),
          );
        },
      });
  }

  private resetForm(): void {
    this.editingID.set(null);

    this.form.reset({
      productID: 0,
      skuCode: '',
      skuName: '',
      size: '',
      unit: '',
      barcode: '',
      mrp: 0,
      standardTradePrice: 0,
      isActive: true,
    });
  }
}
