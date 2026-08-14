import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize, forkJoin, type Observable } from 'rxjs';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import type { PagedRequest } from '../../../../core/models/paged-result.model';
import type { Branch } from '../../../organization/models/organization.model';
import { OrganizationApiService } from '../../../organization/services/organization-api.service';
import type { SKUListItem } from '../../../products/models/products.model';
import { ProductsApiService } from '../../../products/services/products-api.service';
import type {
  PurchaseRequisitionListItem,
  SavePurchaseOrderRequest,
  SupplierListItem,
} from '../../models/procurement.model';
import { ProcurementApiService } from '../../services/procurement-api.service';

@Component({
  selector: 'app-purchase-order-form-page',
  standalone: true,
  imports: [DecimalPipe, ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './purchase-order-form-page.component.html',
  styleUrl: './purchase-order-form-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PurchaseOrderFormPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  private readonly api = inject(ProcurementApiService);
  private readonly organizationApi = inject(OrganizationApiService);
  private readonly productsApi = inject(ProductsApiService);
  protected readonly id = Number(this.route.snapshot.paramMap.get('id') ?? 0);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly error = signal('');
  protected readonly suppliers = signal<readonly SupplierListItem[]>([]);
  protected readonly requisitions = signal<readonly PurchaseRequisitionListItem[]>([]);
  protected readonly branches = signal<readonly Branch[]>([]);
  protected readonly skus = signal<readonly SKUListItem[]>([]);
  protected readonly form = this.fb.group({
    purchaseOrderNo: ['', Validators.required],
    supplierID: [0, [Validators.required, Validators.min(1)]],
    purchaseRequisitionID: [null as number | null],
    branchID: [0, [Validators.required, Validators.min(1)]],
    orderDate: ['', Validators.required],
    expectedDeliveryDate: [''],
    items: this.fb.array([]),
  });
  protected get items(): FormArray {
    return this.form.controls.items as FormArray;
  }
  constructor() {
    this.loadLookups();
    this.addItem();
    if (this.id) {
      this.load();
    }
  }
  private loadLookups(): void {
    const request: PagedRequest = {
      pageNumber: 1,
      pageSize: 500,
      sortBy: 'CreatedAt',
      sortDirection: 'desc',
    };
    forkJoin({
      suppliers: this.api.getSuppliers({
        ...request,
        sortBy: 'SupplierName',
        sortDirection: 'asc',
      }),
      requisitions: this.api.getPurchaseRequisitions(request),
      branches: this.organizationApi.getBranches(),
      skus: this.productsApi.getSKUs({ ...request, sortBy: 'SKUName', sortDirection: 'asc' }),
    }).subscribe({
      next: (r) => {
        this.suppliers.set(r.suppliers.items);
        this.requisitions.set(r.requisitions.items.filter((x) => x.status === 3));
        this.branches.set(r.branches);
        this.skus.set(r.skus.items);
      },
      error: () => this.error.set('Unable to load purchase order lookups.'),
    });
  }
  private load(): void {
    this.loading.set(true);
    this.api
      .getPurchaseOrder(this.id)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (r) => {
          const { items, ...header } = r; this.form.patchValue(header); this.items.clear(); items.forEach((item) => this.items.push(this.createItem(item)));
        },
        error: () => this.error.set('Unable to load the purchase order.'),
      });
  }
  private createItem(value?: {
    skuID: number;
    orderedQuantity: number;
    unitCost: number;
    discountAmount: number;
    taxAmount: number;
  }) {
    return this.fb.group({
      skuID: [value?.skuID ?? 0, [Validators.required, Validators.min(1)]],
      orderedQuantity: [value?.orderedQuantity ?? 1, [Validators.required, Validators.min(0.01)]],
      unitCost: [value?.unitCost ?? 0, [Validators.required, Validators.min(0)]],
      discountAmount: [value?.discountAmount ?? 0, [Validators.required, Validators.min(0)]],
      taxAmount: [value?.taxAmount ?? 0, [Validators.required, Validators.min(0)]],
    });
  }
  protected addItem(): void {
    this.items.push(this.createItem());
  }
  protected removeItem(index: number): void {
    if (this.items.length > 1) {
      this.items.removeAt(index);
    }
  }
  protected lineTotal(index: number): number {
    const v = this.items.at(index).value;
    return (
      Number(v.orderedQuantity ?? 0) * Number(v.unitCost ?? 0) -
      Number(v.discountAmount ?? 0) +
      Number(v.taxAmount ?? 0)
    );
  }
  protected netTotal(): number {
    return this.items.controls.reduce((sum, _, index) => sum + this.lineTotal(index), 0);
  }
  protected save(): void {
    if (this.form.invalid || !this.items.length) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const value = this.form.getRawValue();
    const request: SavePurchaseOrderRequest = {
      purchaseOrderNo: value.purchaseOrderNo ?? '',
      supplierID: Number(value.supplierID),
      purchaseRequisitionID: value.purchaseRequisitionID
        ? Number(value.purchaseRequisitionID)
        : null,
      branchID: Number(value.branchID),
      orderDate: value.orderDate ?? '',
      expectedDeliveryDate: value.expectedDeliveryDate || null,
      items: (value.items ?? []).map((item: any) => ({
        skuID: Number(item.skuID),
        orderedQuantity: Number(item.orderedQuantity),
        unitCost: Number(item.unitCost),
        discountAmount: Number(item.discountAmount),
        taxAmount: Number(item.taxAmount),
      })),
    };
    const operation: Observable<number | boolean> = this.id
      ? this.api.updatePurchaseOrder(this.id, request)
      : this.api.createPurchaseOrder(request);
    operation
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => this.router.navigate(['/procurement/purchase-orders']),
        error: () => this.error.set('Unable to save the purchase order.'),
      });
  }
}


