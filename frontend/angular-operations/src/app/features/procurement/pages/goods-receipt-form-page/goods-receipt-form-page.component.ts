import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize, forkJoin, type Observable } from 'rxjs';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import type { PagedRequest } from '../../../../core/models/paged-result.model';
import type { EmployeeListItem } from '../../../administration/models/administration.model';
import { AdministrationApiService } from '../../../administration/services/administration-api.service';
import type { Warehouse } from '../../../inventory/models/inventory.model';
import { InventoryApiService } from '../../../inventory/services/inventory-api.service';
import type {
  PurchaseOrderDetails,
  PurchaseOrderListItem,
  SaveGoodsReceiptRequest,
} from '../../models/procurement.model';
import { ProcurementApiService } from '../../services/procurement-api.service';

@Component({
  selector: 'app-goods-receipt-form-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './goods-receipt-form-page.component.html',
  styleUrl: './goods-receipt-form-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GoodsReceiptFormPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  private readonly api = inject(ProcurementApiService);
  private readonly inventoryApi = inject(InventoryApiService);
  private readonly administrationApi = inject(AdministrationApiService);
  protected readonly id = Number(this.route.snapshot.paramMap.get('id') ?? 0);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly error = signal('');
  protected readonly purchaseOrders = signal<readonly PurchaseOrderListItem[]>([]);
  protected readonly warehouses = signal<readonly Warehouse[]>([]);
  protected readonly employees = signal<readonly EmployeeListItem[]>([]);
  protected readonly form = this.fb.group({
    goodsReceiptNo: ['', Validators.required],
    purchaseOrderID: [0, [Validators.required, Validators.min(1)]],
    warehouseID: [0, [Validators.required, Validators.min(1)]],
    receivedDate: ['', Validators.required],
    receivedByEmployeeID: [0, [Validators.required, Validators.min(1)]],
    supplierChallanNo: [''],
    items: this.fb.array([]),
  });
  protected get items(): FormArray {
    return this.form.controls.items as FormArray;
  }
  constructor() {
    this.loadLookups();
    if (this.id) {
      this.load();
    }
  }
  private loadLookups(): void {
    const request: PagedRequest = {
      pageNumber: 1,
      pageSize: 500,
      sortBy: 'OrderDate',
      sortDirection: 'desc',
    };
    forkJoin({
      purchaseOrders: this.api.getPurchaseOrders(request),
      warehouses: this.inventoryApi.getWarehouses(),
      employees: this.administrationApi.getEmployees({
        ...request,
        sortBy: 'EmployeeCode',
        sortDirection: 'asc',
      }),
    }).subscribe({
      next: (r) => {
        this.purchaseOrders.set(r.purchaseOrders.items.filter((x) => [3, 4].includes(x.status)));
        this.warehouses.set(r.warehouses.filter((x) => x.isActive));
        this.employees.set(r.employees.items);
      },
      error: () => this.error.set('Unable to load goods receipt lookups.'),
    });
  }
  private load(): void {
    this.loading.set(true);
    this.api
      .getGoodsReceipt(this.id)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (r) => {
          const { items, ...header } = r; this.form.patchValue(header); this.items.clear(); items.forEach((item) => this.items.push(this.createItem(item)));
        },
        error: () => this.error.set('Unable to load the goods receipt.'),
      });
  }
  protected loadPurchaseOrder(): void {
    const id = Number(this.form.controls.purchaseOrderID.value);
    if (!id) {
      return;
    }
    this.api.getPurchaseOrder(id).subscribe({
      next: (po) => {
        this.items.clear();
        po.items
          .filter((item) => item.receivedQuantity < item.orderedQuantity)
          .forEach((item) =>
            this.items.push(
              this.createItem({
                purchaseOrderItemID: item.purchaseOrderItemID,
                skuID: item.skuID,
                acceptedQuantity: item.orderedQuantity - item.receivedQuantity,
                rejectedQuantity: 0,
                batchNo: null,
                manufacturingDate: null,
                expiryDate: null,
                unitCost: item.unitCost,
                rejectionReason: null,
              }),
            ),
          );
      },
      error: () => this.error.set('Unable to load purchase order items.'),
    });
  }
  private createItem(value: any) {
    return this.fb.group({
      purchaseOrderItemID: [value.purchaseOrderItemID],
      skuID: [value.skuID],
      acceptedQuantity: [value.acceptedQuantity ?? 0, [Validators.required, Validators.min(0)]],
      rejectedQuantity: [value.rejectedQuantity ?? 0, [Validators.required, Validators.min(0)]],
      batchNo: [value.batchNo ?? ''],
      manufacturingDate: [value.manufacturingDate ?? ''],
      expiryDate: [value.expiryDate ?? ''],
      unitCost: [value.unitCost ?? 0, [Validators.required, Validators.min(0)]],
      rejectionReason: [value.rejectionReason ?? ''],
    });
  }
  protected save(): void {
    if (this.form.invalid || !this.items.length) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const v = this.form.getRawValue();
    const request: SaveGoodsReceiptRequest = {
      goodsReceiptNo: v.goodsReceiptNo ?? '',
      purchaseOrderID: Number(v.purchaseOrderID),
      warehouseID: Number(v.warehouseID),
      receivedDate: v.receivedDate ?? '',
      receivedByEmployeeID: Number(v.receivedByEmployeeID),
      supplierChallanNo: v.supplierChallanNo || null,
      items: (v.items ?? []).map((item: any) => ({
        purchaseOrderItemID: Number(item.purchaseOrderItemID),
        skuID: Number(item.skuID),
        acceptedQuantity: Number(item.acceptedQuantity),
        rejectedQuantity: Number(item.rejectedQuantity),
        batchNo: item.batchNo || null,
        manufacturingDate: item.manufacturingDate || null,
        expiryDate: item.expiryDate || null,
        unitCost: Number(item.unitCost),
        rejectionReason: item.rejectionReason || null,
      })),
    };
    const operation: Observable<number | boolean> = this.id
      ? this.api.updateGoodsReceipt(this.id, request)
      : this.api.createGoodsReceipt(request);
    operation
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => this.router.navigate(['/procurement/goods-receipts']),
        error: () => this.error.set('Unable to save the goods receipt.'),
      });
  }
}


