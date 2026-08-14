import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize, forkJoin, type Observable } from 'rxjs';

import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import type { PagedRequest } from '../../../../core/models/paged-result.model';
import type { SKUListItem } from '../../../products/models/products.model';
import { ProductsApiService } from '../../../products/services/products-api.service';
import type {
  SaveSupplierProductRequest,
  SaveSupplierRequest,
  SupplierDetails,
} from '../../models/procurement.model';
import { ProcurementApiService } from '../../services/procurement-api.service';

@Component({
  selector: 'app-supplier-form-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './supplier-form-page.component.html',
  styleUrl: './supplier-form-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SupplierFormPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  private readonly api = inject(ProcurementApiService);
  private readonly productsApi = inject(ProductsApiService);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly error = signal('');
  protected readonly supplier = signal<SupplierDetails | null>(null);
  protected readonly skus = signal<readonly SKUListItem[]>([]);
  protected readonly supplierID = Number(this.route.snapshot.paramMap.get('id') ?? 0);

  protected readonly form = this.fb.group({
    supplierCode: ['', [Validators.required, Validators.maxLength(30)]],
    supplierName: ['', [Validators.required, Validators.maxLength(150)]],
    contactPerson: [''],
    phone: [''],
    email: ['', Validators.email],
    address: [''],
    paymentTermsDays: [0, [Validators.required, Validators.min(0)]],
  });
  protected readonly productForm = this.fb.group({
    skuID: [0, [Validators.required, Validators.min(1)]],
    supplierSKUCode: [''],
    lastPurchasePrice: [null as number | null],
    minimumOrderQuantity: [null as number | null],
    leadTimeDays: [null as number | null],
    isPreferredSupplier: [false],
    isActive: [true],
  });

  constructor() {
    this.loadSKUs();
    if (this.supplierID) {
      this.loadSupplier();
    }
  }

  private loadSKUs(): void {
    const request: PagedRequest = {
      pageNumber: 1,
      pageSize: 500,
      sortBy: 'SKUName',
      sortDirection: 'asc',
    };
    this.productsApi
      .getSKUs(request)
      .subscribe({
        next: (result) => this.skus.set(result.items),
        error: () => this.error.set('Unable to load SKUs.'),
      });
  }

  private loadSupplier(): void {
    this.loading.set(true);
    this.api
      .getSupplier(this.supplierID)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (item) => {
          this.supplier.set(item);
          this.form.patchValue(item);
        },
        error: () => this.error.set('Unable to load the supplier.'),
      });
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.error.set('');
    const value = this.form.getRawValue();
    const request: SaveSupplierRequest = {
      supplierCode: value.supplierCode ?? '',
      supplierName: value.supplierName ?? '',
      contactPerson: value.contactPerson || null,
      phone: value.phone || null,
      email: value.email || null,
      address: value.address || null,
      paymentTermsDays: Number(value.paymentTermsDays ?? 0),
    };
    const operation: Observable<number | boolean> = this.supplierID
      ? this.api.updateSupplier(this.supplierID, request)
      : this.api.createSupplier(request);
    operation.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: (result) => {
        const id = this.supplierID || Number(result);
        this.router.navigate(['/procurement/suppliers', id, 'edit']);
      },
      error: () => this.error.set('Unable to save the supplier.'),
    });
  }

  protected saveProduct(): void {
    if (!this.supplierID || this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }
    const value = this.productForm.getRawValue();
    const request: SaveSupplierProductRequest = {
      skuID: Number(value.skuID),
      supplierSKUCode: value.supplierSKUCode || null,
      lastPurchasePrice: value.lastPurchasePrice,
      minimumOrderQuantity: value.minimumOrderQuantity,
      leadTimeDays: value.leadTimeDays,
      isPreferredSupplier: Boolean(value.isPreferredSupplier),
      isActive: Boolean(value.isActive),
    };
    this.api.saveSupplierProduct(this.supplierID, request).subscribe({
      next: () => {
        this.productForm.reset({
          skuID: 0,
          supplierSKUCode: '',
          lastPurchasePrice: null,
          minimumOrderQuantity: null,
          leadTimeDays: null,
          isPreferredSupplier: false,
          isActive: true,
        });
        this.loadSupplier();
      },
      error: () => this.error.set('Unable to save the supplier product.'),
    });
  }
}


