import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize, forkJoin, type Observable } from 'rxjs';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import type { PagedRequest } from '../../../../core/models/paged-result.model';
import type { EmployeeListItem } from '../../../administration/models/administration.model';
import { AdministrationApiService } from '../../../administration/services/administration-api.service';
import type { Branch } from '../../../organization/models/organization.model';
import { OrganizationApiService } from '../../../organization/services/organization-api.service';
import type { SKUListItem } from '../../../products/models/products.model';
import { ProductsApiService } from '../../../products/services/products-api.service';
import type { SavePurchaseRequisitionRequest } from '../../models/procurement.model';
import { ProcurementApiService } from '../../services/procurement-api.service';

@Component({
  selector: 'app-purchase-requisition-form-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './purchase-requisition-form-page.component.html',
  styleUrl: './purchase-requisition-form-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PurchaseRequisitionFormPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);
  private readonly api = inject(ProcurementApiService);
  private readonly organizationApi = inject(OrganizationApiService);
  private readonly administrationApi = inject(AdministrationApiService);
  private readonly productsApi = inject(ProductsApiService);
  protected readonly id = Number(this.route.snapshot.paramMap.get('id') ?? 0);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly error = signal('');
  protected readonly branches = signal<readonly Branch[]>([]);
  protected readonly employees = signal<readonly EmployeeListItem[]>([]);
  protected readonly skus = signal<readonly SKUListItem[]>([]);
  protected readonly form = this.fb.group({
    purchaseRequisitionNo: ['', Validators.required],
    branchID: [0, [Validators.required, Validators.min(1)]],
    requestedByEmployeeID: [0, [Validators.required, Validators.min(1)]],
    requiredDate: ['', Validators.required],
    reason: [''],
    items: this.fb.array([]),
  });
  protected get items(): FormArray {
    return this.form.controls.items as FormArray;
  }
  constructor() {
    this.loadLookups();
    this.addItem();
    if (this.id) {
      this.load();
    }
  }
  private loadLookups(): void {
    const request: PagedRequest = {
      pageNumber: 1,
      pageSize: 500,
      sortBy: 'EmployeeCode',
      sortDirection: 'asc',
    };
    forkJoin({
      branches: this.organizationApi.getBranches(),
      employees: this.administrationApi.getEmployees(request),
      skus: this.productsApi.getSKUs({ ...request, sortBy: 'SKUName' }),
    }).subscribe({
      next: (r) => {
        this.branches.set(r.branches);
        this.employees.set(r.employees.items);
        this.skus.set(r.skus.items);
      },
      error: () => this.error.set('Unable to load form lookups.'),
    });
  }
  private load(): void {
    this.loading.set(true);
    this.api
      .getPurchaseRequisition(this.id)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (r) => {
          const { items, ...header } = r; this.form.patchValue(header); this.items.clear(); items.forEach((item) => this.items.push(this.createItem(item)));
        },
        error: () => this.error.set('Unable to load the requisition.'),
      });
  }
  private createItem(value?: {
    skuID: number;
    requestedQuantity: number;
    estimatedUnitCost: number | null;
    note: string | null;
  }) {
    return this.fb.group({
      skuID: [value?.skuID ?? 0, [Validators.required, Validators.min(1)]],
      requestedQuantity: [
        value?.requestedQuantity ?? 1,
        [Validators.required, Validators.min(0.01)],
      ],
      estimatedUnitCost: [value?.estimatedUnitCost ?? null],
      note: [value?.note ?? ''],
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
  protected save(): void {
    if (this.form.invalid || !this.items.length) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const value = this.form.getRawValue();
    const request: SavePurchaseRequisitionRequest = {
      purchaseRequisitionNo: value.purchaseRequisitionNo ?? '',
      branchID: Number(value.branchID),
      requestedByEmployeeID: Number(value.requestedByEmployeeID),
      requiredDate: value.requiredDate ?? '',
      reason: value.reason || null,
      items: (value.items ?? []).map((item: any) => ({
        skuID: Number(item.skuID),
        requestedQuantity: Number(item.requestedQuantity),
        estimatedUnitCost:
          item.estimatedUnitCost === null || item.estimatedUnitCost === ''
            ? null
            : Number(item.estimatedUnitCost),
        note: item.note || null,
      })),
    };
    const operation: Observable<number | boolean> = this.id
      ? this.api.updatePurchaseRequisition(this.id, request)
      : this.api.createPurchaseRequisition(request);
    operation
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => this.router.navigate(['/procurement/purchase-requisitions']),
        error: () => this.error.set('Unable to save the requisition.'),
      });
  }
}


