import { DatePipe } from '@angular/common';
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
  GoodsReceiptStatus,
  QualityCheckStatus,
  type GoodsReceiptListItem,
} from '../../models/procurement.model';
import { ProcurementApiService } from '../../services/procurement-api.service';

@Component({
  selector: 'app-goods-receipts-page',
  standalone: true,
  imports: [
    DatePipe,
    FormsModule,
    RouterLink,
    EmptyStateComponent,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './goods-receipts-page.component.html',
  styleUrl: './goods-receipts-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GoodsReceiptsPageComponent {
  private readonly api = inject(ProcurementApiService);
  protected readonly rows = signal<readonly GoodsReceiptListItem[]>([]);
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
    this.api
      .getGoodsReceipts({
        pageNumber: page,
        pageSize: this.pageSize,
        search: this.search,
        sortBy: 'ReceivedDate',
        sortDirection: 'desc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (r) => {
          this.rows.set(r.items);
          this.totalCount.set(r.totalCount);
          this.totalPages.set(r.totalPages);
        },
        error: () => this.error.set('Unable to load goods receipts.'),
      });
  }
  protected qualityCheck(id: number): void {
    const raw = window.prompt('Quality status: 2 Passed, 3 Partially Accepted, 4 Failed', '2');
    const status = Number(raw) as QualityCheckStatus;
    if (![2, 3, 4].includes(status)) {
      return;
    }
    this.api
      .completeGoodsReceiptQualityCheck(id, status)
      .subscribe({
        next: () => this.load(),
        error: () => this.error.set('Unable to complete the quality check.'),
      });
  }
  protected post(id: number): void {
    const note = window.prompt('Optional posting note') || null;
    this.api
      .postGoodsReceipt(id, note)
      .subscribe({
        next: () => this.load(),
        error: () => this.error.set('Unable to post the goods receipt.'),
      });
  }
  protected statusLabel(value: GoodsReceiptStatus): string {
    return GoodsReceiptStatus[value] ?? 'Unknown';
  }
  protected qualityLabel(value: QualityCheckStatus): string {
    return QualityCheckStatus[value] ?? 'Unknown';
  }
  protected tone(value: GoodsReceiptStatus): StatusBadgeTone {
    return value === GoodsReceiptStatus.Posted || value === GoodsReceiptStatus.Approved
      ? 'success'
      : value === GoodsReceiptStatus.Rejected
        ? 'danger'
        : value === GoodsReceiptStatus.QualityCheck
          ? 'warning'
          : 'info';
  }
}
