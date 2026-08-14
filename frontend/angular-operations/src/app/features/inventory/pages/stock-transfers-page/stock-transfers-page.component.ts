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
import type { SKUListItem } from '../../../products/models/products.model';
import { ProductsApiService } from '../../../products/services/products-api.service';
import {
  StockTransferStatus,
  type SaveStockTransferRequest,
  type StockTransferDetails,
  type StockTransferListItem,
  type Warehouse,
} from '../../models/inventory.model';
import { InventoryApiService } from '../../services/inventory-api.service';

@Component({
  selector: 'app-stock-transfers-page',
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
  templateUrl: './stock-transfers-page.component.html',
  styleUrl: './stock-transfers-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StockTransfersPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(InventoryApiService);
  private readonly productsApi = inject(ProductsApiService);
  protected readonly rows = signal<readonly StockTransferListItem[]>([]);
  protected readonly selected = signal<StockTransferDetails | null>(null);
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
    stockTransferNo: ['', Validators.required],
    fromWarehouseID: [0, [Validators.required, Validators.min(1)]],
    toWarehouseID: [0, [Validators.required, Validators.min(1)]],
    requestedAt: ['', Validators.required],
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
      sortBy: 'SKUName',
      sortDirection: 'asc',
    };
    forkJoin({
      warehouses: this.api.getWarehouses(),
      skus: this.productsApi.getSKUs(request),
    }).subscribe({
      next: (r) => {
        this.warehouses.set(r.warehouses.filter((x) => x.isActive));
        this.skus.set(r.skus.items);
      },
      error: () => this.error.set('Unable to load stock transfer lookups.'),
    });
  }
  protected load(page = this.pageNumber): void {
    this.pageNumber = page;
    this.loading.set(true);
    this.api
      .getStockTransfers({
        pageNumber: page,
        pageSize: this.pageSize,
        search: this.search,
        sortBy: 'RequestedAt',
        sortDirection: 'desc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (r) => {
          this.rows.set(r.items);
          this.totalCount.set(r.totalCount);
          this.totalPages.set(r.totalPages);
        },
        error: () => this.error.set('Unable to load stock transfers.'),
      });
  }
  private createItem(value?: any) {
    return this.fb.group({
      skuID: [value?.skuID ?? 0, [Validators.required, Validators.min(1)]],
      batchID: [value?.batchID ?? null],
      requestedQuantity: [
        value?.requestedQuantity ?? 1,
        [Validators.required, Validators.min(0.01)],
      ],
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
    if (Number(v.fromWarehouseID) === Number(v.toWarehouseID)) {
      this.error.set('Source and destination warehouses must be different.');
      return;
    }
    const request: SaveStockTransferRequest = {
      stockTransferNo: v.stockTransferNo ?? '',
      fromWarehouseID: Number(v.fromWarehouseID),
      toWarehouseID: Number(v.toWarehouseID),
      requestedAt: v.requestedAt ?? '',
      items: (v.items ?? []).map((item: any) => ({
        skuID: Number(item.skuID),
        batchID: item.batchID ? Number(item.batchID) : null,
        requestedQuantity: Number(item.requestedQuantity),
      })),
    };
    this.api.createStockTransfer(request).subscribe({
      next: (id) => {
        this.showCreate = false;
        this.items.clear();
        this.addItem();
        this.form.reset({ fromWarehouseID: 0, toWarehouseID: 0 });
        this.load(1);
        this.open(id);
      },
      error: () => this.error.set('Unable to create the stock transfer.'),
    });
  }
  protected open(id: number): void {
    this.api
      .getStockTransfer(id)
      .subscribe({
        next: (item) => this.selected.set(item),
        error: () => this.error.set('Unable to load stock transfer details.'),
      });
  }
  protected submit(id: number): void {
    this.api
      .submitStockTransfer(id)
      .subscribe({
        next: () => this.refreshSelected(id),
        error: () => this.error.set('Unable to submit the stock transfer.'),
      });
  }
  protected approve(id: number): void {
    this.api
      .approveStockTransfer(id)
      .subscribe({
        next: () => this.refreshSelected(id),
        error: () => this.error.set('Unable to approve the stock transfer.'),
      });
  }
  protected dispatch(item: StockTransferDetails): void {
    const note = window.prompt('Optional dispatch note') || null;
    const lines = item.items.map((line) => ({
      stockTransferItemID: line.stockTransferItemID,
      dispatchedQuantity: line.requestedQuantity,
    }));
    this.api
      .dispatchStockTransfer(item.stockTransferID, { items: lines, note })
      .subscribe({
        next: () => this.refreshSelected(item.stockTransferID),
        error: () => this.error.set('Unable to dispatch the stock transfer.'),
      });
  }
  protected receive(item: StockTransferDetails): void {
    const note = window.prompt('Optional receive note') || null;
    const lines = item.items.map((line) => ({
      stockTransferItemID: line.stockTransferItemID,
      receivedQuantity: line.dispatchedQuantity,
    }));
    this.api
      .receiveStockTransfer(item.stockTransferID, { items: lines, note })
      .subscribe({
        next: () => this.refreshSelected(item.stockTransferID),
        error: () => this.error.set('Unable to receive the stock transfer.'),
      });
  }
  private refreshSelected(id: number): void {
    this.load();
    this.open(id);
  }
  protected label(value: StockTransferStatus): string {
    return StockTransferStatus[value] ?? 'Unknown';
  }
  protected tone(value: StockTransferStatus): StatusBadgeTone {
    return value === StockTransferStatus.Received
      ? 'success'
      : value === StockTransferStatus.Cancelled
        ? 'danger'
        : value === StockTransferStatus.Submitted ||
            value === StockTransferStatus.Approved ||
            value === StockTransferStatus.Dispatched ||
            value === StockTransferStatus.PartiallyReceived
          ? 'warning'
          : 'info';
  }
}
