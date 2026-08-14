import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
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
import {
  DeliveryStatus,
  type DeliveryListItem,
  type InvoiceListItem,
  type PickListListItem,
} from '../../models/fulfilment.model';
import { FulfilmentApiService } from '../../services/fulfilment-api.service';

@Component({
  selector: 'app-deliveries-page',
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
  templateUrl: './deliveries-page.component.html',
  styleUrl: './deliveries-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DeliveriesPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly api = inject(FulfilmentApiService);
  private readonly inventoryApi = inject(InventoryApiService);
  protected readonly rows = signal<readonly DeliveryListItem[]>([]);
  protected readonly pickLists = signal<readonly PickListListItem[]>([]);
  protected readonly invoices = signal<readonly InvoiceListItem[]>([]);
  protected readonly warehouses = signal<readonly Warehouse[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal('');
  protected readonly totalCount = signal(0);
  protected readonly totalPages = signal(0);
  protected pageNumber = 1;
  protected readonly pageSize = 10;
  protected search = '';
  protected showCreate = Boolean(this.route.snapshot.queryParamMap.get('pickListID'));
  protected readonly form = this.fb.group({
    deliveryNo: ['', Validators.required],
    orderID: [0, [Validators.required, Validators.min(1)]],
    invoiceID: this.fb.control<number | null>(null),
    pickListID: [
      Number(this.route.snapshot.queryParamMap.get('pickListID')) || 0,
      [Validators.required, Validators.min(1)],
    ],
    warehouseID: [0, [Validators.required, Validators.min(1)]],
    plannedDeliveryDate: [''],
  });
  constructor() {
    this.load();
    forkJoin({
      picks: this.api.getPickLists({
        pageNumber: 1,
        pageSize: 500,
        sortBy: 'PickListID',
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
        this.pickLists.set(r.picks.items);
        this.invoices.set(r.invoices.items);
        this.warehouses.set(r.warehouses.filter((x) => x.isActive));
        const selected = r.picks.items.find(
          (x) => x.pickListID === Number(this.form.controls.pickListID.value),
        );
        if (selected)
          this.form.patchValue({
            orderID: selected.orderID,
            invoiceID: selected.invoiceID,
            warehouseID: selected.warehouseID,
          });
      },
      error: () => this.error.set('Unable to load delivery lookups.'),
    });
  }
  protected selectPickList(idValue: string): void {
    const selected = this.pickLists().find((x) => x.pickListID === Number(idValue));
    if (selected)
      this.form.patchValue({
        orderID: selected.orderID,
        invoiceID: selected.invoiceID,
        warehouseID: selected.warehouseID,
      });
  }
  protected load(page = this.pageNumber): void {
    this.pageNumber = page;
    this.loading.set(true);
    this.api
      .getDeliveries({
        pageNumber: page,
        pageSize: this.pageSize,
        search: this.search,
        sortBy: 'DeliveryID',
        sortDirection: 'desc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (r) => {
          this.rows.set(r.items);
          this.totalCount.set(r.totalCount);
          this.totalPages.set(r.totalPages);
        },
        error: () => this.error.set('Unable to load deliveries.'),
      });
  }
  protected create(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.getRawValue();
    this.api
      .createDelivery({
        deliveryNo: v.deliveryNo ?? '',
        orderID: Number(v.orderID),
        invoiceID: v.invoiceID ? Number(v.invoiceID) : null,
        pickListID: Number(v.pickListID),
        warehouseID: Number(v.warehouseID),
        plannedDeliveryDate: v.plannedDeliveryDate || null,
      })
      .subscribe({
        next: (id) => this.router.navigate(['/fulfilment/deliveries', id]),
        error: () => this.error.set('Unable to create the delivery.'),
      });
  }
  protected label(v: DeliveryStatus): string {
    return DeliveryStatus[v] ?? 'Unknown';
  }
  protected tone(v: DeliveryStatus): StatusBadgeTone {
    if (v === 5) return 'success';
    if ([6, 8].includes(v)) return 'danger';
    if ([3, 4, 7].includes(v)) return 'warning';
    return 'neutral';
  }
}
