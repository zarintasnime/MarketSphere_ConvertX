import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import type { PagedRequest } from '../../../../core/models/paged-result.model';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import {
  StatusBadgeComponent,
  type StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import type { EmployeeListItem } from '../../../administration/models/administration.model';
import { AdministrationApiService } from '../../../administration/services/administration-api.service';
import type { Warehouse } from '../../../inventory/models/inventory.model';
import { InventoryApiService } from '../../../inventory/services/inventory-api.service';
import {
  ReturnConditionStatus,
  ReturnDisposition,
  ReturnRequestStatus,
  ReturnResolutionType,
  type ResolveReturnRequest,
  type ReturnDetails,
  type ReturnItem,
} from '../../models/returns-payments.model';
import { ReturnsPaymentsApiService } from '../../services/returns-payments-api.service';

interface ResolutionLine {
  returnItemID: number;
  approvedQuantity: number;
  receivedQuantity: number;
  conditionStatus: ReturnConditionStatus;
  inspectionResult: string;
  disposition: ReturnDisposition;
  restockQuantity: number;
  quarantineQuantity: number;
  damageQuantity: number;
  replacementQuantity: number;
  creditAmount: number;
}
@Component({
  selector: 'app-return-details-page',
  standalone: true,
  imports: [
    DatePipe,
    FormsModule,
    LoadingPanelComponent,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './return-details-page.component.html',
  styleUrl: './return-details-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ReturnDetailsPageComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ReturnsPaymentsApiService);
  private readonly adminApi = inject(AdministrationApiService);
  private readonly inventoryApi = inject(InventoryApiService);
  protected readonly item = signal<ReturnDetails | null>(null);
  protected readonly employees = signal<readonly EmployeeListItem[]>([]);
  protected readonly warehouses = signal<readonly Warehouse[]>([]);
  protected readonly loading = signal(true);
  protected readonly error = signal('');
  protected readonly success = signal('');
  protected warehouseID = 0;
  protected resolvedByEmployeeID = 0;
  protected resolutionType = ReturnResolutionType.Restock;
  protected resolutionNote = '';
  protected creditNoteNo = '';
  protected readonly lines = new Map<number, ResolutionLine>();
  protected readonly conditionOptions = Object.entries(ReturnConditionStatus).filter(([x]) =>
    Number.isNaN(Number(x)),
  );
  protected readonly dispositionOptions = Object.entries(ReturnDisposition).filter(([x]) =>
    Number.isNaN(Number(x)),
  );
  protected readonly resolutionOptions = Object.entries(ReturnResolutionType).filter(([x]) =>
    Number.isNaN(Number(x)),
  );
  private readonly id = Number(this.route.snapshot.paramMap.get('id'));
  constructor() {
    this.refresh();
    const request: PagedRequest = { pageNumber: 1, pageSize: 500 };
    this.adminApi.getEmployees(request).subscribe((r) => this.employees.set(r.items));
    this.inventoryApi
      .getWarehouses()
      .subscribe((x) => this.warehouses.set(x.filter((w) => w.isActive)));
  }
  protected value(line: ReturnItem): ResolutionLine {
    if (!this.lines.has(line.returnItemID))
      this.lines.set(line.returnItemID, {
        returnItemID: line.returnItemID,
        approvedQuantity: line.approvedQuantity || line.requestedQuantity,
        receivedQuantity: line.receivedQuantity || line.approvedQuantity || line.requestedQuantity,
        conditionStatus: line.conditionStatus || ReturnConditionStatus.Saleable,
        inspectionResult: line.inspectionResult || '',
        disposition: line.disposition || ReturnDisposition.Pending,
        restockQuantity: line.restockQuantity || 0,
        quarantineQuantity: line.quarantineQuantity || 0,
        damageQuantity: line.damageQuantity || 0,
        replacementQuantity: line.replacementQuantity || 0,
        creditAmount: line.creditAmount || 0,
      });
    return this.lines.get(line.returnItemID)!;
  }
  protected approve(): void {
    const request = {
      items:
        this.item()?.items.map((x) => ({
          returnItemID: x.returnItemID,
          approvedQuantity: Number(this.value(x).approvedQuantity),
        })) ?? [],
    };
    this.api.approveReturn(this.id, request).subscribe({
      next: () => {
        this.success.set('Return request approved.');
        this.refresh();
      },
      error: () => this.error.set('Unable to approve the return request.'),
    });
  }
  protected resolve(): void {
    if (!this.warehouseID || !this.resolvedByEmployeeID) {
      this.error.set('Select the warehouse and resolving employee.');
      return;
    }
    const request: ResolveReturnRequest = {
      warehouseID: this.warehouseID,
      resolvedByEmployeeID: this.resolvedByEmployeeID,
      resolutionType: Number(this.resolutionType) as ReturnResolutionType,
      resolutionNote: this.resolutionNote.trim(),
      creditNoteNo: this.creditNoteNo.trim() || null,
      items:
        this.item()?.items.map((x) => {
          const v = this.value(x);
          return {
            returnItemID: x.returnItemID,
            receivedQuantity: Number(v.receivedQuantity),
            conditionStatus: Number(v.conditionStatus) as ReturnConditionStatus,
            inspectionResult: v.inspectionResult.trim() || null,
            disposition: Number(v.disposition) as ReturnDisposition,
            restockQuantity: Number(v.restockQuantity),
            quarantineQuantity: Number(v.quarantineQuantity),
            damageQuantity: Number(v.damageQuantity),
            replacementQuantity: Number(v.replacementQuantity),
            creditAmount: Number(v.creditAmount),
          };
        }) ?? [],
    };
    this.api.resolveReturn(this.id, request).subscribe({
      next: () => {
        this.success.set('Return request resolved.');
        this.refresh();
      },
      error: () => this.error.set('Unable to resolve the return request.'),
    });
  }
  private refresh(): void {
    this.api.getReturn(this.id).subscribe({
      next: (x) => {
        this.item.set(x);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Unable to load return details.');
        this.loading.set(false);
      },
    });
  }
  protected statusLabel(v: ReturnRequestStatus): string {
    return ReturnRequestStatus[v] ?? 'Unknown';
  }
  protected tone(v: ReturnRequestStatus): StatusBadgeTone {
    if ([7, 8].includes(v)) return 'success';
    if (v === 4) return 'danger';
    if ([2, 3, 5, 6].includes(v)) return 'warning';
    return 'neutral';
  }
}
