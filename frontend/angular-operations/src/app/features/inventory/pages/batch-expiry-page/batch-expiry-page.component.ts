import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { EmptyStateComponent } from '../../../../shared/components/empty-state.component';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import {
  StatusBadgeComponent,
  type StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import type { PagedRequest } from '../../../../core/models/paged-result.model';
import type { SKUListItem } from '../../../products/models/products.model';
import { ProductsApiService } from '../../../products/services/products-api.service';
import { BatchStatus, type Batch } from '../../models/inventory.model';
import { InventoryApiService } from '../../services/inventory-api.service';

@Component({
  selector: 'app-batch-expiry-page',
  standalone: true,
  imports: [
    DatePipe,
    DecimalPipe,
    FormsModule,
    EmptyStateComponent,
    LoadingPanelComponent,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './batch-expiry-page.component.html',
  styleUrl: './batch-expiry-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BatchExpiryPageComponent {
  private readonly api = inject(InventoryApiService);
  private readonly productsApi = inject(ProductsApiService);
  protected readonly rows = signal<readonly Batch[]>([]);
  protected readonly skus = signal<readonly SKUListItem[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal('');
  protected skuID: number | null = null;
  protected includeExpired = true;
  protected warningDays = 90;
  protected readonly filteredRows = computed(() =>
    this.rows()
      .filter((item) => {
        const days = this.daysToExpiry(item.expiryDate);
        return days === null || days <= this.warningDays;
      })
      .sort((a, b) => (a.expiryDate ?? '9999').localeCompare(b.expiryDate ?? '9999')),
  );
  constructor() {
    const request: PagedRequest = {
      pageNumber: 1,
      pageSize: 500,
      sortBy: 'SKUName',
      sortDirection: 'asc',
    };
    this.productsApi.getSKUs(request).subscribe({
      next: (r) => {
        this.skus.set(r.items);
        this.load();
      },
      error: () => this.error.set('Unable to load SKUs.'),
    });
  }
  protected load(): void {
    this.loading.set(true);
    this.api
      .getBatches(this.skuID, this.includeExpired)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (rows) => this.rows.set(rows),
        error: () => this.error.set('Unable to load batches.'),
      });
  }
  protected daysToExpiry(value: string | null): number | null {
    if (!value) {
      return null;
    }
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const expiry = new Date(value);
    expiry.setHours(0, 0, 0, 0);
    return Math.ceil((expiry.getTime() - today.getTime()) / 86400000);
  }
  protected expiryLabel(value: string | null): string {
    const days = this.daysToExpiry(value);
    if (days === null) {
      return 'No expiry';
    }
    if (days < 0) {
      return `${Math.abs(days)} days expired`;
    }
    if (days === 0) {
      return 'Expires today';
    }
    return `${days} days remaining`;
  }
  protected expiryTone(value: string | null): StatusBadgeTone {
    const days = this.daysToExpiry(value);
    if (days === null) {
      return 'neutral';
    }
    if (days < 0) {
      return 'danger';
    }
    if (days <= 30) {
      return 'danger';
    }
    if (days <= 90) {
      return 'warning';
    }
    return 'success';
  }
  protected statusLabel(value: BatchStatus): string {
    return BatchStatus[value] ?? 'Unknown';
  }
}
