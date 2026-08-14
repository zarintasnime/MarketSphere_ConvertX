import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, forkJoin, type Observable } from 'rxjs';
import { EmptyStateComponent } from '../../../../shared/components/empty-state.component';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { StatusBadgeComponent } from '../../../../shared/components/status-badge.component';
import type { Branch } from '../../../organization/models/organization.model';
import { OrganizationApiService } from '../../../organization/services/organization-api.service';
import {
  WarehouseType,
  type SaveWarehouseRequest,
  type Warehouse,
} from '../../models/inventory.model';
import { InventoryApiService } from '../../services/inventory-api.service';

@Component({
  selector: 'app-warehouses-page',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    EmptyStateComponent,
    LoadingPanelComponent,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './warehouses-page.component.html',
  styleUrl: './warehouses-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class WarehousesPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(InventoryApiService);
  private readonly organizationApi = inject(OrganizationApiService);
  protected readonly rows = signal<readonly Warehouse[]>([]);
  protected readonly branches = signal<readonly Branch[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal('');
  protected editingID = 0;
  protected showForm = false;
  protected readonly types = Object.entries(WarehouseType)
    .filter(([key]) => Number.isNaN(Number(key)))
    .map(([label, value]) => ({ label, value: Number(value) }));
  protected readonly form = this.fb.group({
    branchID: [0, [Validators.required, Validators.min(1)]],
    warehouseCode: ['', Validators.required],
    warehouseName: ['', Validators.required],
    warehouseType: [WarehouseType.Main, Validators.required],
    address: [''],
  });
  constructor() {
    this.load();
  }
  protected load(): void {
    this.loading.set(true);
    forkJoin({ rows: this.api.getWarehouses(), branches: this.organizationApi.getBranches() })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (r) => {
          this.rows.set(r.rows);
          this.branches.set(r.branches);
        },
        error: () => this.error.set('Unable to load warehouses.'),
      });
  }
  protected startCreate(): void {
    this.editingID = 0;
    this.form.reset({
      branchID: 0,
      warehouseCode: '',
      warehouseName: '',
      warehouseType: WarehouseType.Main,
      address: '',
    });
    this.showForm = true;
  }
  protected edit(item: Warehouse): void {
    this.editingID = item.warehouseID;
    this.form.patchValue(item);
    this.showForm = true;
  }
  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.getRawValue();
    const request: SaveWarehouseRequest = {
      branchID: Number(v.branchID),
      warehouseCode: v.warehouseCode ?? '',
      warehouseName: v.warehouseName ?? '',
      warehouseType: Number(v.warehouseType) as WarehouseType,
      address: v.address || null,
    };
    const operation: Observable<number | boolean> = this.editingID
      ? this.api.updateWarehouse(this.editingID, request)
      : this.api.createWarehouse(request);
    operation.subscribe({
      next: () => {
        this.showForm = false;
        this.load();
      },
      error: () => this.error.set('Unable to save the warehouse.'),
    });
  }
  protected toggle(item: Warehouse): void {
    this.api
      .changeWarehouseStatus(item.warehouseID, !item.isActive)
      .subscribe({
        next: () => this.load(),
        error: () => this.error.set('Unable to change warehouse status.'),
      });
  }
  protected typeLabel(value: WarehouseType): string {
    return WarehouseType[value] ?? 'Unknown';
  }
}

