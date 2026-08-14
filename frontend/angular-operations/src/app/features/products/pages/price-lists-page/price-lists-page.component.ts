import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { createEmptyPagedResult } from '../../../../core/models/paged-result.model';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { PaginationComponent } from '../../../../shared/components/pagination.component';
import {
  StatusBadgeComponent,
  StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import {
  PRICE_LIST_STATUS_OPTIONS,
  SALES_CHANNEL_OPTIONS,
  PriceListListItem,
  PriceListStatus,
  SalesChannel,
  optionLabel,
} from '../../models/products.model';
import { ProductsApiService } from '../../services/products-api.service';

@Component({
  selector: 'app-price-lists-page',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './price-lists-page.component.html',
  styleUrl: './price-lists-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PriceListsPageComponent {
  private readonly api = inject(ProductsApiService);
  private readonly auth = inject(AuthService);
  protected readonly router = inject(Router);

  protected readonly result = signal(createEmptyPagedResult<PriceListListItem>(1, 10));
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly search = signal('');
  protected readonly canManage = computed(() =>
    this.auth.hasPermission('pricing.price_lists.manage'),
  );

  constructor() {
    this.load();
  }

  protected load(pageNumber = this.result().pageNumber): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.api
      .getPriceLists({
        pageNumber,
        pageSize: this.result().pageSize,
        search: this.search(),
        sortBy: 'EffectiveFrom',
        sortDirection: 'desc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected changeStatus(item: PriceListListItem, status: PriceListStatus): void {
    this.api.changePriceListStatus(item.priceListID, status).subscribe({
      next: () => this.load(),
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected channelLabel(value: SalesChannel): string {
    return optionLabel(SALES_CHANNEL_OPTIONS, value);
  }
  protected statusLabel(value: PriceListStatus): string {
    return optionLabel(PRICE_LIST_STATUS_OPTIONS, value);
  }
  protected statusTone(value: PriceListStatus): StatusBadgeTone {
    return value === PriceListStatus.Active
      ? 'success'
      : value === PriceListStatus.Draft
        ? 'info'
        : value === PriceListStatus.Expired
          ? 'danger'
          : 'neutral';
  }
}
