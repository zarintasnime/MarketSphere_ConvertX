import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import type { PagedRequest } from '../../../../core/models/paged-result.model';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import type { EmployeeListItem } from '../../../administration/models/administration.model';
import { AdministrationApiService } from '../../../administration/services/administration-api.service';
import type { ClientListItem, QuotationListItem } from '../../../crm/models/crm.model';
import { CrmApiService } from '../../../crm/services/crm-api.service';
import type { Warehouse } from '../../../inventory/models/inventory.model';
import { InventoryApiService } from '../../../inventory/services/inventory-api.service';
import type { SKUListItem } from '../../../products/models/products.model';
import { ProductsApiService } from '../../../products/services/products-api.service';
import {
  SalesChannel,
  type ConvertModernTradePurchaseOrderRequest,
  type ConvertQuotationToOrderRequest,
  type ModernTradePurchaseOrderListItem,
  type SaveRegularOrderRequest,
} from '../../models/orders.model';
import { OrdersApiService } from '../../services/orders-api.service';

type OrderMode = 'regular' | 'quotation' | 'mt';
@Component({
  selector: 'app-order-form-page',
  standalone: true,
  imports: [ReactiveFormsModule, PageHeaderComponent],
  templateUrl: './order-form-page.component.html',
  styleUrl: './order-form-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OrderFormPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly api = inject(OrdersApiService);
  private readonly crmApi = inject(CrmApiService);
  private readonly productsApi = inject(ProductsApiService);
  private readonly adminApi = inject(AdministrationApiService);
  private readonly inventoryApi = inject(InventoryApiService);
  protected readonly clients = signal<readonly ClientListItem[]>([]);
  protected readonly skus = signal<readonly SKUListItem[]>([]);
  protected readonly employees = signal<readonly EmployeeListItem[]>([]);
  protected readonly quotations = signal<readonly QuotationListItem[]>([]);
  protected readonly mtOrders = signal<readonly ModernTradePurchaseOrderListItem[]>([]);
  protected readonly warehouses = signal<readonly Warehouse[]>([]);
  protected readonly error = signal('');
  protected mode: OrderMode =
    (this.route.snapshot.queryParamMap.get('mode') as OrderMode) || 'regular';
  protected readonly channelOptions = Object.entries(SalesChannel).filter(([k]) =>
    Number.isNaN(Number(k)),
  );
  protected readonly form = this.fb.group({
    orderNo: ['', Validators.required],
    clientID: [0],
    employeeID: [null],
    channel: [SalesChannel.GeneralTrade],
    campaignID: [null],
    priceListID: [null],
    orderDate: ['', Validators.required],
    requestedDeliveryDate: [''],
    deliveryAddressSnapshot: ['', Validators.required],
    quotationID: [Number(this.route.snapshot.queryParamMap.get('quotationID')) || 0],
    modernTradePurchaseOrderID: [
      Number(this.route.snapshot.queryParamMap.get('mtPurchaseOrderID')) || 0,
    ],
    items: this.fb.array([]),
  });
  protected get items(): FormArray {
    return this.form.controls.items as FormArray;
  }
  constructor() {
    this.addItem();
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
      quotations: this.crmApi.getQuotations(request),
      mtOrders: this.api.getModernTradePurchaseOrders(request),
      warehouses: this.inventoryApi.getWarehouses(),
    }).subscribe({
      next: (r) => {
        this.clients.set(r.clients.items);
        this.skus.set(r.skus.items);
        this.employees.set(r.employees.items);
        this.quotations.set(r.quotations.items);
        this.mtOrders.set(r.mtOrders.items.filter((x) => x.status === 3));
        this.warehouses.set(r.warehouses);
      },
      error: () => this.error.set('Unable to load order form lookups.'),
    });
  }
  private createItem() {
    return this.fb.group({
      skuID: [0, [Validators.required, Validators.min(1)]],
      orderedQuantity: [1, [Validators.required, Validators.min(0.01)]],
      freeQuantity: [0, [Validators.min(0)]],
      unitPrice: [0, [Validators.required, Validators.min(0)]],
      discountPercent: [0, [Validators.min(0), Validators.max(100)]],
      taxAmount: [0, [Validators.min(0)]],
    });
  }
  protected addItem(): void {
    this.items.push(this.createItem());
  }
  protected removeItem(i: number): void {
    if (this.items.length > 1) this.items.removeAt(i);
  }
  protected setMode(mode: OrderMode): void {
    this.mode = mode;
    this.error.set('');
  }
  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.getRawValue();
    if (this.mode === 'regular') {
      const request: SaveRegularOrderRequest = {
        orderNo: v.orderNo ?? '',
        clientID: Number(v.clientID),
        employeeID: v.employeeID ? Number(v.employeeID) : null,
        channel: Number(v.channel) as SalesChannel,
        campaignID: v.campaignID ? Number(v.campaignID) : null,
        priceListID: v.priceListID ? Number(v.priceListID) : null,
        orderDate: v.orderDate ?? '',
        requestedDeliveryDate: v.requestedDeliveryDate || null,
        deliveryAddressSnapshot: v.deliveryAddressSnapshot ?? '',
        items: (v.items ?? []).map((x: any) => ({
          skuID: Number(x.skuID),
          orderedQuantity: Number(x.orderedQuantity),
          freeQuantity: Number(x.freeQuantity),
          unitPrice: Number(x.unitPrice),
          discountPercent: Number(x.discountPercent),
          taxAmount: Number(x.taxAmount),
        })),
      };
      this.api
        .createRegularOrder(request)
        .subscribe({
          next: (id) => this.router.navigate(['/orders', id]),
          error: () => this.error.set('Unable to create the regular order.'),
        });
      return;
    }
    if (this.mode === 'quotation') {
      const request: ConvertQuotationToOrderRequest = {
        orderNo: v.orderNo ?? '',
        quotationID: Number(v.quotationID),
        employeeID: v.employeeID ? Number(v.employeeID) : null,
        orderDate: v.orderDate ?? '',
        requestedDeliveryDate: v.requestedDeliveryDate || null,
        deliveryAddressSnapshot: v.deliveryAddressSnapshot ?? '',
      };
      this.api
        .convertQuotation(request)
        .subscribe({
          next: (id) => this.router.navigate(['/orders', id]),
          error: () => this.error.set('Unable to convert the quotation.'),
        });
      return;
    }
    const request: ConvertModernTradePurchaseOrderRequest = {
      orderNo: v.orderNo ?? '',
      modernTradePurchaseOrderID: Number(v.modernTradePurchaseOrderID),
      employeeID: v.employeeID ? Number(v.employeeID) : null,
      priceListID: v.priceListID ? Number(v.priceListID) : null,
      orderDate: v.orderDate ?? '',
      deliveryAddressSnapshot: v.deliveryAddressSnapshot ?? '',
    };
    this.api
      .convertModernTradePurchaseOrder(request)
      .subscribe({
        next: (id) => this.router.navigate(['/orders', id]),
        error: () => this.error.set('Unable to convert the modern trade purchase order.'),
      });
  }
}
