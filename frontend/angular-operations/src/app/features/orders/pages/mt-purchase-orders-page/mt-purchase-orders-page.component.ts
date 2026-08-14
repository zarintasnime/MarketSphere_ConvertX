import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize, forkJoin } from 'rxjs';
import type { PagedRequest } from '../../../../core/models/paged-result.model';
import { EmptyStateComponent } from '../../../../shared/components/empty-state.component';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { PaginationComponent } from '../../../../shared/components/pagination.component';
import {
  StatusBadgeComponent,
  type StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import type { EmployeeListItem } from '../../../administration/models/administration.model';
import { AdministrationApiService } from '../../../administration/services/administration-api.service';
import type { ClientListItem } from '../../../crm/models/crm.model';
import { CrmApiService } from '../../../crm/services/crm-api.service';
import type { SKUListItem } from '../../../products/models/products.model';
import { ProductsApiService } from '../../../products/services/products-api.service';
import {
  ModernTradePurchaseOrderStatus,
  type ModernTradePurchaseOrderListItem,
  type SaveModernTradePurchaseOrderRequest,
} from '../../models/orders.model';
import { OrdersApiService } from '../../services/orders-api.service';

@Component({
  selector: 'app-mt-purchase-orders-page',
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
  templateUrl: './mt-purchase-orders-page.component.html',
  styleUrl: './mt-purchase-orders-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MtPurchaseOrdersPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(OrdersApiService);
  private readonly crmApi = inject(CrmApiService);
  private readonly productsApi = inject(ProductsApiService);
  private readonly adminApi = inject(AdministrationApiService);
  private readonly router = inject(Router);
  protected readonly rows = signal<readonly ModernTradePurchaseOrderListItem[]>([]);
  protected readonly clients = signal<readonly ClientListItem[]>([]);
  protected readonly skus = signal<readonly SKUListItem[]>([]);
  protected readonly employees = signal<readonly EmployeeListItem[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal('');
  protected readonly success = signal('');
  protected readonly totalCount = signal(0);
  protected readonly totalPages = signal(0);
  protected pageNumber = 1;
  protected readonly pageSize = 10;
  protected search = '';
  protected showCreate = false;
  protected readonly form = this.fb.group({
    clientID: [0, [Validators.required, Validators.min(1)]],
    poNumber: ['', Validators.required],
    poDate: ['', Validators.required],
    receivedDate: ['', Validators.required],
    uploadedByEmployeeID: [0, [Validators.required, Validators.min(1)]],
    duplicateHash: [''],
    requestedDeliveryDate: [''],
    items: this.fb.array([]),
  });
  protected get items(): FormArray {
    return this.form.controls.items as FormArray;
  }
  constructor() {
    this.addItem();
    this.loadLookups();
    this.load();
  }
  private loadLookups(): void {
    const request: PagedRequest = {
      pageNumber: 1,
      pageSize: 500,
      sortBy: 'Name',
      sortDirection: 'asc',
    };
    forkJoin({
      clients: this.crmApi.getClients(request),
      skus: this.productsApi.getSKUs(request),
      employees: this.adminApi.getEmployees(request),
    }).subscribe({
      next: (r) => {
        this.clients.set(r.clients.items);
        this.skus.set(r.skus.items);
        this.employees.set(r.employees.items);
      },
      error: () => this.error.set('Unable to load purchase order lookups.'),
    });
  }
  protected load(page = this.pageNumber): void {
    this.pageNumber = page;
    this.loading.set(true);
    this.error.set('');
    this.api
      .getModernTradePurchaseOrders({
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
        error: () => this.error.set('Unable to load modern trade purchase orders.'),
      });
  }
  private createItem() {
    return this.fb.group({
      externalItemCode: [''],
      externalItemName: ['', Validators.required],
      skuID: [null],
      orderedQuantity: [1, [Validators.required, Validators.min(0.01)]],
      agreedUnitPrice: [null],
      discount: [null],
      note: [''],
    });
  }
  protected addItem(): void {
    this.items.push(this.createItem());
  }
  protected removeItem(index: number): void {
    if (this.items.length > 1) {
      this.items.removeAt(index);
    }
  }
  protected create(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.getRawValue();
    const request: SaveModernTradePurchaseOrderRequest = {
      clientID: Number(v.clientID),
      poNumber: v.poNumber ?? '',
      poDate: v.poDate ?? '',
      receivedDate: v.receivedDate ?? '',
      uploadedByEmployeeID: Number(v.uploadedByEmployeeID),
      duplicateHash: v.duplicateHash?.trim() || null,
      requestedDeliveryDate: v.requestedDeliveryDate || null,
      items: (v.items ?? []).map((x: any) => ({
        externalItemCode: x.externalItemCode?.trim() || null,
        externalItemName: x.externalItemName?.trim() || null,
        skuID: x.skuID ? Number(x.skuID) : null,
        orderedQuantity: Number(x.orderedQuantity),
        agreedUnitPrice:
          x.agreedUnitPrice === null || x.agreedUnitPrice === '' ? null : Number(x.agreedUnitPrice),
        discount: x.discount === null || x.discount === '' ? null : Number(x.discount),
        note: x.note?.trim() || null,
      })),
    };
    this.api.createModernTradePurchaseOrder(request).subscribe({
      next: (id) => {
        this.success.set('Modern trade purchase order created successfully.');
        this.showCreate = false;
        this.router.navigate(['/orders/mt-purchase-orders', id]);
      },
      error: () => this.error.set('Unable to create the modern trade purchase order.'),
    });
  }
  protected label(status: ModernTradePurchaseOrderStatus): string {
    return ModernTradePurchaseOrderStatus[status] ?? 'Unknown';
  }
  protected tone(status: ModernTradePurchaseOrderStatus): StatusBadgeTone {
    if (
      status === ModernTradePurchaseOrderStatus.Verified ||
      status === ModernTradePurchaseOrderStatus.Converted
    )
      return 'success';
    if (
      status === ModernTradePurchaseOrderStatus.Rejected ||
      status === ModernTradePurchaseOrderStatus.Cancelled
    )
      return 'danger';
    if (status === ModernTradePurchaseOrderStatus.Submitted) return 'warning';
    return 'neutral';
  }
}
