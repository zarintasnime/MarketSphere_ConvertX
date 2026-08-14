import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import { EmptyStateComponent } from '../../../../shared/components/empty-state.component';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import type { PagedRequest } from '../../../../core/models/paged-result.model';
import type { SKUListItem } from '../../../products/models/products.model';
import { ProductsApiService } from '../../../products/services/products-api.service';
import {
  StockMovementType,
  type StockMovement,
  type Warehouse,
} from '../../models/inventory.model';
import { InventoryApiService } from '../../services/inventory-api.service';

@Component({
  selector: 'app-stock-movement-page',
  standalone: true,
  imports: [
    DatePipe,
    DecimalPipe,
    FormsModule,
    EmptyStateComponent,
    LoadingPanelComponent,
    PageHeaderComponent,
  ],
  templateUrl: './stock-movement-page.component.html',
  styleUrl: './stock-movement-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StockMovementPageComponent {
  private readonly api = inject(InventoryApiService);
  private readonly productsApi = inject(ProductsApiService);
  protected readonly rows = signal<readonly StockMovement[]>([]);
  protected readonly warehouses = signal<readonly Warehouse[]>([]);
  protected readonly skus = signal<readonly SKUListItem[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal('');
  protected warehouseID: number | null = null;
  protected skuID: number | null = null;
  protected from: string | null = null;
  protected to: string | null = null;
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
        this.warehouses.set(r.warehouses);
        this.skus.set(r.skus.items);
        this.load();
      },
      error: () => this.error.set('Unable to load movement lookups.'),
    });
  }
  protected load(): void {
    this.loading.set(true);
    this.api
      .getMovements(this.warehouseID, this.skuID, this.from, this.to)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (rows) => this.rows.set(rows),
        error: () => this.error.set('Unable to load stock movements.'),
      });
  }
  protected label(value: StockMovementType): string {
    return StockMovementType[value] ?? 'Unknown';
  }
}
