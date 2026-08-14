import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize, forkJoin, of } from 'rxjs';
import { EmptyStateComponent } from '../../../../shared/components/empty-state.component';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import {
  ReturnRequestStatus,
  ReturnResolutionType,
  type CreditResolutionRow,
  type ReturnListItem,
} from '../../models/returns-payments.model';
import { ReturnsPaymentsApiService } from '../../services/returns-payments-api.service';

@Component({
  selector: 'app-credit-notes-page',
  standalone: true,
  imports: [
    DatePipe,
    DecimalPipe,
    FormsModule,
    RouterLink,
    EmptyStateComponent,
    LoadingPanelComponent,
    PageHeaderComponent,
  ],
  templateUrl: './credit-notes-page.component.html',
  styleUrl: './credit-notes-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CreditNotesPageComponent {
  private readonly api = inject(ReturnsPaymentsApiService);
  protected readonly rows = signal<readonly CreditResolutionRow[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal('');
  protected search = '';
  constructor() {
    this.load();
  }
  protected load(): void {
    this.loading.set(true);
    this.error.set('');
    this.api
      .getReturns({
        pageNumber: 1,
        pageSize: 500,
        search: this.search,
        sortBy: 'RequestDate',
        sortDirection: 'desc',
      })
      .subscribe({
        next: (result) =>
          this.loadDetails(
            result.items.filter(
              (x) =>
                x.resolutionType === ReturnResolutionType.Credit ||
                x.resolutionType === ReturnResolutionType.Mixed,
            ),
          ),
        error: () => {
          this.error.set('Unable to load credit resolutions.');
          this.loading.set(false);
        },
      });
  }
  private loadDetails(items: readonly ReturnListItem[]): void {
    if (!items.length) {
      this.rows.set([]);
      this.loading.set(false);
      return;
    }
    forkJoin(items.map((item) => this.api.getReturn(item.returnRequestID)))
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (details) =>
          this.rows.set(
            details.map((x) => ({
              returnRequestID: x.returnRequestID,
              returnNo: x.returnNo,
              clientID: x.clientID,
              invoiceID: x.invoiceID,
              resolvedAt: x.resolvedAt,
              resolutionType: x.resolutionType,
              totalCreditAmount: x.items.reduce((sum, line) => sum + line.creditAmount, 0),
              status: x.status,
            })),
          ),
        error: () => this.error.set('Unable to load credit resolution details.'),
      });
  }
  protected resolutionLabel(v: ReturnResolutionType | null): string {
    return v ? ReturnResolutionType[v] : 'Pending';
  }
  protected statusLabel(v: ReturnRequestStatus): string {
    return ReturnRequestStatus[v] ?? 'Unknown';
  }
}
