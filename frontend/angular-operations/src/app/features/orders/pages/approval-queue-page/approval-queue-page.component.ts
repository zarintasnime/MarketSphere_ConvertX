import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { EmptyStateComponent } from '../../../../shared/components/empty-state.component';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { PaginationComponent } from '../../../../shared/components/pagination.component';
import {
  StatusBadgeComponent,
  type StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import {
  ApprovalActionType,
  ApprovalRequestStatus,
  ApprovalType,
  type ApprovalRequest,
} from '../../models/orders.model';
import { OrdersApiService } from '../../services/orders-api.service';

@Component({
  selector: 'app-approval-queue-page',
  standalone: true,
  imports: [
    DatePipe,
    FormsModule,
    EmptyStateComponent,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './approval-queue-page.component.html',
  styleUrl: './approval-queue-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApprovalQueuePageComponent {
  private readonly api = inject(OrdersApiService);
  protected readonly rows = signal<readonly ApprovalRequest[]>([]);
  protected readonly selected = signal<ApprovalRequest | null>(null);
  protected readonly loading = signal(false);
  protected readonly error = signal('');
  protected readonly success = signal('');
  protected readonly totalCount = signal(0);
  protected readonly totalPages = signal(0);
  protected search = '';
  protected pageNumber = 1;
  protected readonly pageSize = 10;
  protected note = '';
  protected delegateToUserID: number | null = null;
  constructor() {
    this.load();
  }
  protected load(page = this.pageNumber): void {
    this.pageNumber = page;
    this.loading.set(true);
    this.api
      .getApprovalQueue({
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
        error: () => this.error.set('Unable to load the approval queue.'),
      });
  }
  protected open(id: number): void {
    this.api
      .getApproval(id)
      .subscribe({
        next: (x) => this.selected.set(x),
        error: () => this.error.set('Unable to load approval details.'),
      });
  }
  protected act(action: ApprovalActionType): void {
    const item = this.selected();
    if (!item) return;
    this.api
      .actOnApproval(item.approvalRequestID, {
        action,
        note: this.note.trim() || null,
        delegateToUserID: action === ApprovalActionType.Delegated ? this.delegateToUserID : null,
      })
      .subscribe({
        next: () => {
          this.success.set('Approval action recorded.');
          this.open(item.approvalRequestID);
          this.load();
        },
        error: () => this.error.set('Unable to record the approval action.'),
      });
  }
  protected cancel(): void {
    const item = this.selected();
    if (!item) return;
    this.api.cancelApproval(item.approvalRequestID, this.note.trim() || null).subscribe({
      next: () => {
        this.success.set('Approval request cancelled.');
        this.open(item.approvalRequestID);
        this.load();
      },
      error: () => this.error.set('Unable to cancel the approval request.'),
    });
  }
  protected statusLabel(v: ApprovalRequestStatus): string {
    return ApprovalRequestStatus[v] ?? 'Unknown';
  }
  protected typeLabel(v: ApprovalType): string {
    return ApprovalType[v] ?? 'Unknown';
  }
  protected actionLabel(v: ApprovalActionType): string {
    return ApprovalActionType[v] ?? 'Unknown';
  }
  protected tone(v: ApprovalRequestStatus): StatusBadgeTone {
    if (v === 3) return 'success';
    if (v === 4 || v === 5) return 'danger';
    if (v === 2) return 'warning';
    return 'neutral';
  }
}
