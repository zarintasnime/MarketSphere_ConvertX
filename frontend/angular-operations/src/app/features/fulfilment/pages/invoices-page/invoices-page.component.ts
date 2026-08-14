import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
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
import type { OrderDetails, OrderListItem } from '../../../orders/models/orders.model';
import { OrdersApiService } from '../../../orders/services/orders-api.service';
import {
  InvoiceStatus,
  type CreateInvoiceRequest,
  type InvoiceListItem,
} from '../../models/fulfilment.model';
import { FulfilmentApiService } from '../../services/fulfilment-api.service';

@Component({
  selector: 'app-invoices-page',
  standalone: true,
  imports: [
    DatePipe,
    DecimalPipe,
    FormsModule,
    ReactiveFormsModule,
    RouterLink,
    EmptyStateComponent,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './invoices-page.component.html',
  styleUrl: './invoices-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InvoicesPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(FulfilmentApiService);
  private readonly ordersApi = inject(OrdersApiService);
  private readonly router = inject(Router);

  protected readonly rows = signal<readonly InvoiceListItem[]>([]);
  protected readonly orders = signal<readonly OrderListItem[]>([]);
  protected readonly selectedOrder = signal<OrderDetails | null>(null);

  protected readonly loading = signal(false);
  protected readonly error = signal('');

  protected readonly totalCount = signal(0);
  protected readonly totalPages = signal(0);

  protected pageNumber = 1;
  protected readonly pageSize = 10;

  protected search = '';
  protected showCreate = false;

  protected readonly form = this.fb.group({
    invoiceNo: ['', Validators.required],
    orderID: [0, [Validators.required, Validators.min(1)]],
    invoiceDate: ['', Validators.required],
    dueDate: [''],
    items: this.fb.array([]),
  });

  protected get items(): FormArray {
    return this.form.controls.items as FormArray;
  }

  constructor() {
    this.load();

    this.ordersApi
      .getOrders({
        pageNumber: 1,
        pageSize: 500,
        sortBy: 'OrderDate',
        sortDirection: 'desc',
      })
      .subscribe({
        next: (result) => {
          this.orders.set(result.items);
        },
        error: () => {
          this.error.set('Unable to load orders for invoice creation.');
        },
      });
  }

  protected load(page = this.pageNumber): void {
    this.pageNumber = page;
    this.loading.set(true);
    this.error.set('');

    this.api
      .getInvoices({
        pageNumber: page,
        pageSize: this.pageSize,
        search: this.search.trim(),
        sortBy: 'InvoiceDate',
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
          this.error.set('Unable to load invoices.');
        },
      });
  }

  protected toggleCreateForm(): void {
    this.showCreate = !this.showCreate;
    this.error.set('');

    if (!this.showCreate) {
      this.resetCreateForm();
      return;
    }

    this.form.patchValue({
      invoiceDate: this.getToday(),
    });
  }

  protected selectOrder(orderID: string): void {
    const id = Number(orderID);

    this.selectedOrder.set(null);
    this.items.clear();
    this.error.set('');

    if (!Number.isInteger(id) || id < 1) {
      return;
    }

    this.ordersApi.getOrder(id).subscribe({
      next: (order) => {
        this.selectedOrder.set(order);

        for (const line of order.items) {
          const approvedQuantity = Number(line.approvedQuantity ?? 0);

          const deliveredQuantity = Number(line.deliveredQuantity ?? 0);

          const orderedQuantity = Number(line.orderedQuantity ?? 0);

          const remainingQuantity = Math.max(0, approvedQuantity - deliveredQuantity);

          const invoiceQuantity =
            remainingQuantity > 0
              ? remainingQuantity
              : approvedQuantity > 0
                ? approvedQuantity
                : orderedQuantity;

          if (!Number.isFinite(invoiceQuantity) || invoiceQuantity <= 0) {
            continue;
          }

          this.items.push(
            this.fb.group({
              orderItemID: [Number(line.orderItemID), [Validators.required, Validators.min(1)]],
              skuCode: [line.skuCode],
              quantity: [invoiceQuantity, [Validators.required, Validators.min(0.01)]],
            }),
          );
        }

        if (this.items.length === 0) {
          this.error.set('The selected order has no invoiceable items.');
        }
      },
      error: () => {
        this.selectedOrder.set(null);
        this.items.clear();

        this.error.set('Unable to load order lines.');
      },
    });
  }

  protected create(): void {
    this.error.set('');

    if (this.form.invalid) {
      this.form.markAllAsTouched();

      this.error.set('Complete all required invoice fields.');

      return;
    }

    if (!this.selectedOrder()) {
      this.error.set('Select a valid order before creating the invoice.');

      return;
    }

    if (this.items.length === 0) {
      this.error.set('The selected order has no invoiceable items.');

      return;
    }

    const value = this.form.getRawValue();

    const requestItems = (value.items ?? [])
      .map((item: any) => ({
        orderItemID: Number(item.orderItemID),
        quantity: Number(item.quantity),
      }))
      .filter(
        (item) =>
          Number.isInteger(item.orderItemID) &&
          item.orderItemID > 0 &&
          Number.isFinite(item.quantity) &&
          item.quantity > 0,
      );

    if (requestItems.length === 0) {
      this.error.set('At least one valid invoice item is required.');

      return;
    }

    const request: CreateInvoiceRequest = {
      invoiceNo: value.invoiceNo?.trim() ?? '',
      orderID: Number(value.orderID),
      invoiceDate: value.invoiceDate ?? '',
      dueDate: value.dueDate || null,
      items: requestItems,
    };

    this.api.createInvoice(request).subscribe({
      next: (invoiceID) => {
        void this.router.navigate(['/fulfilment/invoices', invoiceID]);
      },
      error: () => {
        this.error.set('Unable to create the invoice.');
      },
    });
  }

  protected cancelCreate(): void {
    this.showCreate = false;
    this.error.set('');
    this.resetCreateForm();
  }

  protected label(value: InvoiceStatus): string {
    return InvoiceStatus[value] ?? 'Unknown';
  }

  protected tone(value: InvoiceStatus): StatusBadgeTone {
    if (value === 4) {
      return 'success';
    }

    if (value === 7) {
      return 'danger';
    }

    if ([2, 3, 5, 6].includes(value)) {
      return 'warning';
    }

    return 'neutral';
  }

  private resetCreateForm(): void {
    this.selectedOrder.set(null);
    this.items.clear();

    this.form.reset({
      invoiceNo: '',
      orderID: 0,
      invoiceDate: '',
      dueDate: '',
      items: [],
    });
  }

  private getToday(): string {
    const date = new Date();

    const year = date.getFullYear();

    const month = String(date.getMonth() + 1).padStart(2, '0');

    const day = String(date.getDate()).padStart(2, '0');

    return `${year}-${month}-${day}`;
  }
}
