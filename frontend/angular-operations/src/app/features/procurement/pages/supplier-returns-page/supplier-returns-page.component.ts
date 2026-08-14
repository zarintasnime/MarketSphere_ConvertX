import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import { EmptyStateComponent } from '../../../../shared/components/empty-state.component';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { PaginationComponent } from '../../../../shared/components/pagination.component';
import {
  StatusBadgeComponent,
  type StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import type { PagedRequest } from '../../../../core/models/paged-result.model';
import type { Warehouse } from '../../../inventory/models/inventory.model';
import { InventoryApiService } from '../../../inventory/services/inventory-api.service';
import type { SKUListItem } from '../../../products/models/products.model';
import { ProductsApiService } from '../../../products/services/products-api.service';
import {
  SupplierReturnStatus,
  type SaveSupplierReturnRequest,
  type SupplierListItem,
  type SupplierReturnDetails,
  type SupplierReturnListItem,
} from '../../models/procurement.model';
import { ProcurementApiService } from '../../services/procurement-api.service';

@Component({
  selector: 'app-supplier-returns-page',
  standalone: true,
  imports: [
    DatePipe,
    DecimalPipe,
    FormsModule,
    ReactiveFormsModule,
    EmptyStateComponent,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './supplier-returns-page.component.html',
  styleUrl: './supplier-returns-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SupplierReturnsPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(ProcurementApiService);
  private readonly inventoryApi = inject(InventoryApiService);
  private readonly productsApi = inject(ProductsApiService);
  protected readonly rows = signal<readonly SupplierReturnListItem[]>([]);
  protected readonly selected = signal<SupplierReturnDetails | null>(null);
  protected readonly suppliers = signal<readonly SupplierListItem[]>([]);
  protected readonly warehouses = signal<readonly Warehouse[]>([]);
  protected readonly skus = signal<readonly SKUListItem[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal('');
  protected readonly totalCount = signal(0);
  protected readonly totalPages = signal(0);
  protected search = '';
  protected pageNumber = 1;
  protected readonly pageSize = 10;
  protected showCreate = false;
  protected readonly form = this.fb.group({
    supplierReturnNo: ['', Validators.required],
    supplierID: [0, [Validators.required, Validators.min(1)]],
    goodsReceiptID: [null as number | null],
    warehouseID: [0, [Validators.required, Validators.min(1)]],
    returnDate: ['', Validators.required],
    reason: ['', Validators.required],
    items: this.fb.array([]),
  });
  protected get items(): FormArray {
    return this.form.controls.items as FormArray;
  }
  constructor() {
    this.addItem();
    this.loadLookups();
    this.load();
  }
  private loadLookups(): void {
    const request: PagedRequest = {
      pageNumber: 1,
      pageSize: 500,
      sortBy: 'Name',
      sortDirection: 'asc',
    };
    forkJoin({
      suppliers: this.api.getSuppliers({ ...request, sortBy: 'SupplierName' }),
      warehouses: this.inventoryApi.getWarehouses(),
      skus: this.productsApi.getSKUs({ ...request, sortBy: 'SKUName' }),
    }).subscribe({
      next: (r) => {
        this.suppliers.set(r.suppliers.items);
        this.warehouses.set(r.warehouses.filter((x) => x.isActive));
        this.skus.set(r.skus.items);
      },
      error: () => this.error.set('Unable to load supplier return lookups.'),
    });
  }
  protected load(page = this.pageNumber): void {
    this.pageNumber = page;
    this.loading.set(true);
    this.api
      .getSupplierReturns({
        pageNumber: page,
        pageSize: this.pageSize,
        search: this.search,
        sortBy: 'ReturnDate',
        sortDirection: 'desc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (r) => {
          this.rows.set(r.items);
          this.totalCount.set(r.totalCount);
          this.totalPages.set(r.totalPages);
        },
        error: () => this.error.set('Unable to load supplier returns.'),
      });
  }
  private createItem(value?: any) {
    return this.fb.group({
      skuID: [value?.skuID ?? 0, [Validators.required, Validators.min(1)]],
      batchID: [value?.batchID ?? null],
      quantity: [value?.quantity ?? 1, [Validators.required, Validators.min(0.01)]],
      unitCost: [value?.unitCost ?? 0, [Validators.required, Validators.min(0)]],
      reason: [value?.reason ?? '', Validators.required],
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
  protected create(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.getRawValue();
    const request: SaveSupplierReturnRequest = {
      supplierReturnNo: v.supplierReturnNo ?? '',
      supplierID: Number(v.supplierID),
      goodsReceiptID: v.goodsReceiptID ? Number(v.goodsReceiptID) : null,
      warehouseID: Number(v.warehouseID),
      returnDate: v.returnDate ?? '',
      reason: v.reason ?? '',
      items: (v.items ?? []).map((item: any) => ({
        skuID: Number(item.skuID),
        batchID: item.batchID ? Number(item.batchID) : null,
        quantity: Number(item.quantity),
        unitCost: Number(item.unitCost),
        reason: item.reason ?? '',
      })),
    };
    this.api.createSupplierReturn(request).subscribe({
      next: () => {
        this.showCreate = false;
        this.items.clear();
        this.addItem();
        this.form.reset({ supplierID: 0, warehouseID: 0, goodsReceiptID: null });
        this.load(1);
      },
      error: () => this.error.set('Unable to create the supplier return.'),
    });
  }
  protected open(id: number): void {
    this.api
      .getSupplierReturn(id)
      .subscribe({
        next: (item) => this.selected.set(item),
        error: () => this.error.set('Unable to load supplier return details.'),
      });
  }
  protected setStatus(id: number, status: SupplierReturnStatus): void {
    this.api.changeSupplierReturnStatus(id, status).subscribe({
      next: () => {
        this.load();
        this.open(id);
      },
      error: () => this.error.set('Unable to change supplier return status.'),
    });
  }
  protected post(id: number): void {
    const note = window.prompt('Optional posting note') || null;
    this.api.postSupplierReturn(id, note).subscribe({
      next: () => {
        this.load();
        this.open(id);
      },
      error: () => this.error.set('Unable to post the supplier return.'),
    });
  }
  protected label(value: SupplierReturnStatus): string {
    return SupplierReturnStatus[value] ?? 'Unknown';
  }
  protected tone(value: SupplierReturnStatus): StatusBadgeTone {
    return value === SupplierReturnStatus.Posted
      ? 'success'
      : value === SupplierReturnStatus.Cancelled
        ? 'danger'
        : value === SupplierReturnStatus.Approved || value === SupplierReturnStatus.Submitted
          ? 'warning'
          : 'info';
  }
}
