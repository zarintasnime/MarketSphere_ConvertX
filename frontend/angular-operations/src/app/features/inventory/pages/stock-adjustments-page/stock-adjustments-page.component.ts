import { DatePipe } from '@angular/common';
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
import type { EmployeeListItem } from '../../../administration/models/administration.model';
import { AdministrationApiService } from '../../../administration/services/administration-api.service';
import type { SKUListItem } from '../../../products/models/products.model';
import { ProductsApiService } from '../../../products/services/products-api.service';
import {
  StockAdjustmentStatus,
  type SaveStockAdjustmentRequest,
  type StockAdjustmentDetails,
  type StockAdjustmentListItem,
  type Warehouse,
} from '../../models/inventory.model';
import { InventoryApiService } from '../../services/inventory-api.service';

@Component({
  selector: 'app-stock-adjustments-page',
  standalone: true,
  imports: [
    DatePipe,
    FormsModule,
    ReactiveFormsModule,
    EmptyStateComponent,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './stock-adjustments-page.component.html',
  styleUrl: './stock-adjustments-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StockAdjustmentsPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(InventoryApiService);
  private readonly productsApi = inject(ProductsApiService);
  private readonly administrationApi = inject(AdministrationApiService);
  protected readonly rows = signal<readonly StockAdjustmentListItem[]>([]);
  protected readonly selected = signal<StockAdjustmentDetails | null>(null);
  protected readonly warehouses = signal<readonly Warehouse[]>([]);
  protected readonly skus = signal<readonly SKUListItem[]>([]);
  protected readonly employees = signal<readonly EmployeeListItem[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal('');
  protected readonly totalCount = signal(0);
  protected readonly totalPages = signal(0);
  protected search = '';
  protected pageNumber = 1;
  protected readonly pageSize = 10;
  protected showCreate = false;
  protected readonly form = this.fb.group({
    stockAdjustmentNo: ['', Validators.required],
    warehouseID: [0, [Validators.required, Validators.min(1)]],
    adjustmentDate: ['', Validators.required],
    reason: ['', Validators.required],
    performedByEmployeeID: [0, [Validators.required, Validators.min(1)]],
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
      warehouses: this.api.getWarehouses(),
      skus: this.productsApi.getSKUs({ ...request, sortBy: 'SKUName' }),
      employees: this.administrationApi.getEmployees({ ...request, sortBy: 'EmployeeCode' }),
    }).subscribe({
      next: (r) => {
        this.warehouses.set(r.warehouses.filter((x) => x.isActive));
        this.skus.set(r.skus.items);
        this.employees.set(r.employees.items);
      },
      error: () => this.error.set('Unable to load stock adjustment lookups.'),
    });
  }
  protected load(page = this.pageNumber): void {
    this.pageNumber = page;
    this.loading.set(true);
    this.api
      .getStockAdjustments({
        pageNumber: page,
        pageSize: this.pageSize,
        search: this.search,
        sortBy: 'AdjustmentDate',
        sortDirection: 'desc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (r) => {
          this.rows.set(r.items);
          this.totalCount.set(r.totalCount);
          this.totalPages.set(r.totalPages);
        },
        error: () => this.error.set('Unable to load stock adjustments.'),
      });
  }
  private createItem(value?: any) {
    return this.fb.group({
      skuID: [value?.skuID ?? 0, [Validators.required, Validators.min(1)]],
      batchID: [value?.batchID ?? null],
      adjustmentQuantity: [value?.adjustmentQuantity ?? 0, Validators.required],
      unitCost: [value?.unitCost ?? null],
      note: [value?.note ?? ''],
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
    const request: SaveStockAdjustmentRequest = {
      stockAdjustmentNo: v.stockAdjustmentNo ?? '',
      warehouseID: Number(v.warehouseID),
      adjustmentDate: v.adjustmentDate ?? '',
      reason: v.reason ?? '',
      performedByEmployeeID: Number(v.performedByEmployeeID),
      items: (v.items ?? []).map((item: any) => ({
        skuID: Number(item.skuID),
        batchID: item.batchID ? Number(item.batchID) : null,
        adjustmentQuantity: Number(item.adjustmentQuantity),
        unitCost: item.unitCost === null || item.unitCost === '' ? null : Number(item.unitCost),
        note: item.note || null,
      })),
    };
    this.api.createStockAdjustment(request).subscribe({
      next: (id) => {
        this.showCreate = false;
        this.items.clear();
        this.addItem();
        this.form.reset({ warehouseID: 0, performedByEmployeeID: 0 });
        this.load(1);
        this.open(id);
      },
      error: () => this.error.set('Unable to create the stock adjustment.'),
    });
  }
  protected open(id: number): void {
    this.api
      .getStockAdjustment(id)
      .subscribe({
        next: (item) => this.selected.set(item),
        error: () => this.error.set('Unable to load stock adjustment details.'),
      });
  }
  protected setStatus(id: number, status: StockAdjustmentStatus): void {
    this.api
      .changeStockAdjustmentStatus(id, status)
      .subscribe({
        next: () => this.refreshSelected(id),
        error: () => this.error.set('Unable to change stock adjustment status.'),
      });
  }
  protected post(id: number): void {
    const note = window.prompt('Optional posting note') || null;
    this.api
      .postStockAdjustment(id, note)
      .subscribe({
        next: () => this.refreshSelected(id),
        error: () => this.error.set('Unable to post the stock adjustment.'),
      });
  }
  private refreshSelected(id: number): void {
    this.load();
    this.open(id);
  }
  protected label(value: StockAdjustmentStatus): string {
    return StockAdjustmentStatus[value] ?? 'Unknown';
  }
  protected tone(value: StockAdjustmentStatus): StatusBadgeTone {
    return value === StockAdjustmentStatus.Posted
      ? 'success'
      : value === StockAdjustmentStatus.Rejected || value === StockAdjustmentStatus.Cancelled
        ? 'danger'
        : value === StockAdjustmentStatus.Submitted || value === StockAdjustmentStatus.Approved
          ? 'warning'
          : 'info';
  }
}
