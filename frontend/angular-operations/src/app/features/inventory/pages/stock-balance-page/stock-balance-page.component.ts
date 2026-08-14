import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import { EmptyStateComponent } from '../../../../shared/components/empty-state.component';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import type { PagedRequest } from '../../../../core/models/paged-result.model';
import type { SKUListItem } from '../../../products/models/products.model';
import { ProductsApiService } from '../../../products/services/products-api.service';
import type { StockBalance, Warehouse } from '../../models/inventory.model';
import { InventoryApiService } from '../../services/inventory-api.service';

@Component({
  selector: 'app-stock-balance-page',
  standalone: true,
  imports: [
    DatePipe,
    DecimalPipe,
    FormsModule,
    EmptyStateComponent,
    LoadingPanelComponent,
    PageHeaderComponent,
  ],
  templateUrl: './stock-balance-page.component.html',
  styleUrl: './stock-balance-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StockBalancePageComponent {
  private readonly api = inject(InventoryApiService);
  private readonly productsApi = inject(ProductsApiService);
  protected readonly rows = signal<readonly StockBalance[]>([]);
  protected readonly warehouses = signal<readonly Warehouse[]>([]);
  protected readonly skus = signal<readonly SKUListItem[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal('');
  protected warehouseID: number | null = null;
  protected skuID: number | null = null;
  protected batchID: number | null = null;
  protected includeZero = false;
  protected includeExpired = false;
  protected readonly totals = computed(() =>
    this.rows().reduce(
      (sum, item) => ({
        onHand: sum.onHand + item.onHandQuantity,
        reserved: sum.reserved + item.reservedQuantity,
        available: sum.available + item.availableQuantity,
      }),
      { onHand: 0, reserved: 0, available: 0 },
    ),
  );
  constructor() {
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
        this.load();
      },
      error: () => this.error.set('Unable to load stock lookups.'),
    });
  }
  protected load(): void {
    this.loading.set(true);
    this.api
      .getStockBalances({
        warehouseID: this.warehouseID,
        skuID: this.skuID,
        batchID: this.batchID,
        includeZero: this.includeZero,
        includeExpired: this.includeExpired,
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (rows) => this.rows.set(rows),
        error: () => this.error.set('Unable to load stock balances.'),
      });
  }
}
