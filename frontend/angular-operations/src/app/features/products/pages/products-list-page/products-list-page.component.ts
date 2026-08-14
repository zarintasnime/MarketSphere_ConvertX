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
import { StatusBadgeComponent } from '../../../../shared/components/status-badge.component';
import {
  PRODUCT_TYPE_OPTIONS,
  ProductListItem,
  ProductType,
  optionLabel,
} from '../../models/products.model';
import { ProductsApiService } from '../../services/products-api.service';

@Component({
  selector: 'app-products-list-page',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './products-list-page.component.html',
  styleUrl: './products-list-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProductsListPageComponent {
  private readonly api = inject(ProductsApiService);
  private readonly auth = inject(AuthService);
  protected readonly router = inject(Router);

  protected readonly result = signal(createEmptyPagedResult<ProductListItem>(1, 10));
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly search = signal('');
  protected readonly canManage = computed(() =>
    this.auth.hasPermission('products.products.manage'),
  );

  constructor() {
    this.load();
  }

  protected load(pageNumber = this.result().pageNumber): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.api
      .getProducts({
        pageNumber,
        pageSize: this.result().pageSize,
        search: this.search(),
        sortBy: 'ProductName',
        sortDirection: 'asc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected edit(item: ProductListItem): void {
    if (this.canManage()) void this.router.navigate(['/products', item.productID, 'edit']);
  }

  protected toggleStatus(item: ProductListItem): void {
    this.api.setProductStatus(item.productID, !item.isActive).subscribe({
      next: () => this.load(),
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected typeLabel(value: ProductType): string {
    return optionLabel(PRODUCT_TYPE_OPTIONS, value);
  }
}
