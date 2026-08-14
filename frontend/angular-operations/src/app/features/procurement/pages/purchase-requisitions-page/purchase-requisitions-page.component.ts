import { DatePipe, DecimalPipe } from '@angular/common';
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
import {
  PurchaseRequisitionStatus,
  type PurchaseRequisitionListItem,
} from '../../models/procurement.model';
import { ProcurementApiService } from '../../services/procurement-api.service';

@Component({
  selector: 'app-purchase-requisitions-page',
  standalone: true,
  imports: [
    DatePipe,
    DecimalPipe,
    FormsModule,
    RouterLink,
    EmptyStateComponent,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './purchase-requisitions-page.component.html',
  styleUrl: './purchase-requisitions-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PurchaseRequisitionsPageComponent {
  private readonly api = inject(ProcurementApiService);
  protected readonly rows = signal<readonly PurchaseRequisitionListItem[]>([]);
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
    const request: PagedRequest = {
      pageNumber: page,
      pageSize: this.pageSize,
      search: this.search,
      sortBy: 'RequiredDate',
      sortDirection: 'desc',
    };
    this.api
      .getPurchaseRequisitions(request)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (r) => {
          this.rows.set(r.items);
          this.totalCount.set(r.totalCount);
          this.totalPages.set(r.totalPages);
        },
        error: () => this.error.set('Unable to load purchase requisitions.'),
      });
  }
  protected setStatus(id: number, status: PurchaseRequisitionStatus): void {
    const note = window.prompt('Optional status note') || null;
    this.api
      .changePurchaseRequisitionStatus(id, status, note)
      .subscribe({
        next: () => this.load(),
        error: () => this.error.set('Unable to change the requisition status.'),
      });
  }
  protected label(value: PurchaseRequisitionStatus): string {
    return PurchaseRequisitionStatus[value] ?? 'Unknown';
  }
  protected tone(value: PurchaseRequisitionStatus): StatusBadgeTone {
    return value === PurchaseRequisitionStatus.Approved ||
      value === PurchaseRequisitionStatus.Closed
      ? 'success'
      : value === PurchaseRequisitionStatus.Rejected ||
          value === PurchaseRequisitionStatus.Cancelled
        ? 'danger'
        : value === PurchaseRequisitionStatus.Submitted
          ? 'warning'
          : 'info';
  }
}
