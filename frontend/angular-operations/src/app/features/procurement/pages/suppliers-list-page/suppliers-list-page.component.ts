import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { EmptyStateComponent } from '../../../../shared/components/empty-state.component';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { PaginationComponent } from '../../../../shared/components/pagination.component';
import {
  StatusBadgeComponent,
  type StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import type { PagedRequest } from '../../../../core/models/paged-result.model';
import { SupplierStatus, type SupplierListItem } from '../../models/procurement.model';
import { ProcurementApiService } from '../../services/procurement-api.service';

@Component({
  selector: 'app-suppliers-list-page',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    EmptyStateComponent,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './suppliers-list-page.component.html',
  styleUrl: './suppliers-list-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SuppliersListPageComponent {
  private readonly api = inject(ProcurementApiService);
  protected readonly suppliers = signal<readonly SupplierListItem[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal('');
  protected readonly totalCount = signal(0);
  protected readonly totalPages = signal(0);
  protected search = '';
  protected pageNumber = 1;
  protected readonly pageSize = 10;

  constructor() {
    this.load();
  }

  protected load(page = this.pageNumber): void {
    this.pageNumber = page;
    this.loading.set(true);
    this.error.set('');
    const request: PagedRequest = {
      pageNumber: page,
      pageSize: this.pageSize,
      search: this.search,
      sortBy: 'SupplierName',
      sortDirection: 'asc',
    };
    this.api
      .getSuppliers(request)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => {
          this.suppliers.set(result.items);
          this.totalCount.set(result.totalCount);
          this.totalPages.set(result.totalPages);
        },
        error: () => this.error.set('Unable to load suppliers.'),
      });
  }

  protected changeStatus(item: SupplierListItem): void {
    const next =
      item.status === SupplierStatus.Active ? SupplierStatus.Inactive : SupplierStatus.Active;
    this.api
      .changeSupplierStatus(item.supplierID, next)
      .subscribe({
        next: () => this.load(),
        error: () => this.error.set('Unable to change supplier status.'),
      });
  }

  protected statusLabel(status: SupplierStatus): string {
    return SupplierStatus[status] ?? 'Unknown';
  }
  protected statusTone(status: SupplierStatus): StatusBadgeTone {
    return status === SupplierStatus.Active
      ? 'success'
      : status === SupplierStatus.Suspended
        ? 'warning'
        : 'neutral';
  }
}
