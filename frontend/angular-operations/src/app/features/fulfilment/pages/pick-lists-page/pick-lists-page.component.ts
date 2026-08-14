import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize, forkJoin } from 'rxjs';
import { EmptyStateComponent } from '../../../../shared/components/empty-state.component';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { PaginationComponent } from '../../../../shared/components/pagination.component';
import {
  StatusBadgeComponent,
  type StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import type { Warehouse } from '../../../inventory/models/inventory.model';
import { InventoryApiService } from '../../../inventory/services/inventory-api.service';
import type { OrderListItem } from '../../../orders/models/orders.model';
import { OrdersApiService } from '../../../orders/services/orders-api.service';
import {
  PickListStatus,
  type InvoiceListItem,
  type PickListListItem,
} from '../../models/fulfilment.model';
import { FulfilmentApiService } from '../../services/fulfilment-api.service';

@Component({
  selector: 'app-pick-lists-page',
  standalone: true,
  imports: [
    DatePipe,
    FormsModule,
    ReactiveFormsModule,
    RouterLink,
    EmptyStateComponent,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './pick-lists-page.component.html',
  styleUrl: './pick-lists-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PickListsPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(FulfilmentApiService);
  private readonly ordersApi = inject(OrdersApiService);
  private readonly inventoryApi = inject(InventoryApiService);
  private readonly router = inject(Router);
  protected readonly rows = signal<readonly PickListListItem[]>([]);
  protected readonly orders = signal<readonly OrderListItem[]>([]);
  protected readonly invoices = signal<readonly InvoiceListItem[]>([]);
  protected readonly warehouses = signal<readonly Warehouse[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal('');
  protected readonly totalCount = signal(0);
  protected readonly totalPages = signal(0);
  protected pageNumber = 1;
  protected readonly pageSize = 10;
  protected search = '';
  protected showCreate = false;
  protected readonly form = this.fb.group({
    pickListNo: ['', Validators.required],
    orderID: [0, [Validators.required, Validators.min(1)]],
    invoiceID: [null],
    warehouseID: [0, [Validators.required, Validators.min(1)]],
    waveNo: [''],
    note: [''],
  });
  constructor() {
    this.load();
    forkJoin({
      orders: this.ordersApi.getOrders({
        pageNumber: 1,
        pageSize: 500,
        sortBy: 'OrderDate',
        sortDirection: 'desc',
      }),
      invoices: this.api.getInvoices({
        pageNumber: 1,
        pageSize: 500,
        sortBy: 'InvoiceDate',
        sortDirection: 'desc',
      }),
      warehouses: this.inventoryApi.getWarehouses(),
    }).subscribe({
      next: (r) => {
        this.orders.set(r.orders.items);
        this.invoices.set(r.invoices.items);
        this.warehouses.set(r.warehouses.filter((x) => x.isActive));
      },
      error: () => this.error.set('Unable to load pick list lookups.'),
    });
  }
  protected load(page = this.pageNumber): void {
    this.pageNumber = page;
    this.loading.set(true);
    this.api
      .getPickLists({
        pageNumber: page,
        pageSize: this.pageSize,
        search: this.search,
        sortBy: 'PickListID',
        sortDirection: 'desc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (r) => {
          this.rows.set(r.items);
          this.totalCount.set(r.totalCount);
          this.totalPages.set(r.totalPages);
        },
        error: () => this.error.set('Unable to load pick lists.'),
      });
  }
  protected create(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.getRawValue();
    this.api
      .createPickList({
        pickListNo: v.pickListNo ?? '',
        orderID: Number(v.orderID),
        invoiceID: v.invoiceID ? Number(v.invoiceID) : null,
        warehouseID: Number(v.warehouseID),
        waveNo: v.waveNo?.trim() || null,
        note: v.note?.trim() || null,
      })
      .subscribe({
        next: (id) => this.router.navigate(['/fulfilment/pick-lists', id]),
        error: () => this.error.set('Unable to create the pick list.'),
      });
  }
  protected label(v: PickListStatus): string {
    return PickListStatus[v] ?? 'Unknown';
  }
  protected tone(v: PickListStatus): StatusBadgeTone {
    if (v === 6) return 'success';
    if (v === 7) return 'danger';
    if ([2, 3, 4, 5].includes(v)) return 'warning';
    return 'neutral';
  }
}
