import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import type { PagedRequest } from '../../../../core/models/paged-result.model';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import {
  StatusBadgeComponent,
  type StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import type { EmployeeListItem } from '../../../administration/models/administration.model';
import { AdministrationApiService } from '../../../administration/services/administration-api.service';
import type { SKUListItem } from '../../../products/models/products.model';
import { ProductsApiService } from '../../../products/services/products-api.service';
import {
  ModernTradePurchaseOrderStatus,
  type ModernTradePurchaseOrderDetails,
} from '../../models/orders.model';
import { OrdersApiService } from '../../services/orders-api.service';

@Component({
  selector: 'app-mt-po-details-page',
  standalone: true,
  imports: [
    DatePipe,
    FormsModule,
    RouterLink,
    LoadingPanelComponent,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './mt-po-details-page.component.html',
  styleUrl: './mt-po-details-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MtPoDetailsPageComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(OrdersApiService);
  private readonly productsApi = inject(ProductsApiService);
  private readonly adminApi = inject(AdministrationApiService);
  protected readonly item = signal<ModernTradePurchaseOrderDetails | null>(null);
  protected readonly skus = signal<readonly SKUListItem[]>([]);
  protected readonly employees = signal<readonly EmployeeListItem[]>([]);
  protected readonly loading = signal(true);
  protected readonly error = signal('');
  protected readonly success = signal('');
  protected verifyEmployeeID = 0;
  protected verificationNote = '';
  protected rejectionReason = '';
  private readonly id = Number(this.route.snapshot.paramMap.get('id'));
  constructor() {
    const request: PagedRequest = {
      pageNumber: 1,
      pageSize: 500,
      sortBy: 'Name',
      sortDirection: 'asc',
    };
    forkJoin({
      item: this.api.getModernTradePurchaseOrder(this.id),
      skus: this.productsApi.getSKUs(request),
      employees: this.adminApi.getEmployees(request),
    }).subscribe({
      next: (r) => {
        this.item.set(r.item);
        this.skus.set(r.skus.items);
        this.employees.set(r.employees.items);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Unable to load the modern trade purchase order.');
        this.loading.set(false);
      },
    });
  }
  protected map(lineID: number, skuID: string): void {
    const id = Number(skuID);
    if (!id) return;
    this.api.mapModernTradePurchaseOrderItem(lineID, id).subscribe({
      next: () => {
        this.success.set('Item mapping saved.');
        this.refresh();
      },
      error: () => this.error.set('Unable to map the item.'),
    });
  }
  protected submit(): void {
    this.api.submitModernTradePurchaseOrder(this.id).subscribe({
      next: () => {
        this.success.set('Purchase order submitted.');
        this.refresh();
      },
      error: () => this.error.set('Unable to submit the purchase order.'),
    });
  }
  protected verify(approve: boolean): void {
    if (!this.verifyEmployeeID) {
      this.error.set('Select the verifying employee.');
      return;
    }
    this.api
      .verifyModernTradePurchaseOrder(this.id, {
        approve,
        verifiedByEmployeeID: this.verifyEmployeeID,
        note: this.verificationNote.trim() || null,
        rejectionReason: approve
          ? null
          : this.rejectionReason.trim() || 'Rejected during verification.',
      })
      .subscribe({
        next: () => {
          this.success.set(approve ? 'Purchase order verified.' : 'Purchase order rejected.');
          this.refresh();
        },
        error: () => this.error.set('Unable to complete verification.'),
      });
  }
  private refresh(): void {
    this.api.getModernTradePurchaseOrder(this.id).subscribe((x) => this.item.set(x));
  }
  protected label(status: ModernTradePurchaseOrderStatus): string {
    return ModernTradePurchaseOrderStatus[status] ?? 'Unknown';
  }
  protected tone(status: ModernTradePurchaseOrderStatus): StatusBadgeTone {
    if (status === 3 || status === 5) return 'success';
    if (status === 4 || status === 6) return 'danger';
    if (status === 2) return 'warning';
    return 'neutral';
  }
}
