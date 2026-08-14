import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
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
  CreditCheckStatus,
  OrderSource,
  OrderStatus,
  SalesChannel,
  type OrderListItem,
} from '../../models/orders.model';
import { OrdersApiService } from '../../services/orders-api.service';

@Component({
  selector: 'app-orders-list-page',
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
  templateUrl: './orders-list-page.component.html',
  styleUrl: './orders-list-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OrdersListPageComponent {
  private readonly api = inject(OrdersApiService);

  private readonly router = inject(Router);

  protected readonly rows = signal<readonly OrderListItem[]>([]);

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

    this.api
      .getOrders({
        pageNumber: page,
        pageSize: this.pageSize,
        search: this.search.trim(),
        sortBy: 'OrderDate',
        sortDirection: 'desc',
      })
      .pipe(
        finalize(() => {
          this.loading.set(false);
        }),
      )
      .subscribe({
        next: (result) => {
          this.rows.set(result.items);

          this.totalCount.set(result.totalCount);

          this.totalPages.set(result.totalPages);
        },
        error: () => {
          this.error.set('Unable to load sales orders.');
        },
      });
  }

  protected createOrder(): void {
    void this.router.navigate(['/orders/new']);
  }

  protected orderLabel(value: OrderStatus): string {
    return OrderStatus[value] ?? 'Unknown';
  }

  protected sourceLabel(value: OrderSource): string {
    return OrderSource[value] ?? 'Unknown';
  }

  protected channelLabel(value: SalesChannel): string {
    return SalesChannel[value] ?? 'Unknown';
  }

  protected creditLabel(value: CreditCheckStatus): string {
    return CreditCheckStatus[value] ?? 'Unknown';
  }

  protected tone(status: OrderStatus): StatusBadgeTone {
    if ([4, 5, 6, 7, 8, 9, 11].includes(status)) {
      return 'success';
    }

    if ([12, 13].includes(status)) {
      return 'danger';
    }

    if ([2, 3].includes(status)) {
      return 'warning';
    }

    return 'neutral';
  }
}
