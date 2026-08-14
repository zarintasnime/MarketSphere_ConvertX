import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import type { PagedRequest } from '../../../../core/models/paged-result.model';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import {
  StatusBadgeComponent,
  type StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import type { EmployeeListItem } from '../../../administration/models/administration.model';
import { AdministrationApiService } from '../../../administration/services/administration-api.service';
import {
  DeliveryStatus,
  type CompleteDeliveryItemRequest,
  type DeliveryDetails,
  type DeliveryItem,
} from '../../models/fulfilment.model';
import { FulfilmentApiService } from '../../services/fulfilment-api.service';

@Component({
  selector: 'app-delivery-details-page',
  standalone: true,
  imports: [
    DatePipe,
    FormsModule,
    RouterLink,
    LoadingPanelComponent,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './delivery-details-page.component.html',
  styleUrl: './delivery-details-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DeliveryDetailsPageComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(FulfilmentApiService);
  private readonly adminApi = inject(AdministrationApiService);
  protected readonly item = signal<DeliveryDetails | null>(null);
  protected readonly employees = signal<readonly EmployeeListItem[]>([]);
  protected readonly loading = signal(true);
  protected readonly error = signal('');
  protected readonly success = signal('');
  protected deliveredByEmployeeID = 0;
  protected completionStatus = DeliveryStatus.Delivered;
  protected receiverName = '';
  protected receiverPhone = '';
  protected failureReason = '';
  protected rescheduledDate = '';
  protected readonly completionOptions = [
    DeliveryStatus.PartiallyDelivered,
    DeliveryStatus.Delivered,
    DeliveryStatus.Failed,
    DeliveryStatus.Rescheduled,
    DeliveryStatus.Cancelled,
  ];
  protected readonly lineValues = new Map<number, CompleteDeliveryItemRequest>();
  private readonly id = Number(this.route.snapshot.paramMap.get('id'));
  constructor() {
    this.refresh();
    const request: PagedRequest = { pageNumber: 1, pageSize: 500 };
    this.adminApi.getEmployees(request).subscribe((r) => this.employees.set(r.items));
  }
  protected value(line: DeliveryItem): CompleteDeliveryItemRequest {
    if (!this.lineValues.has(line.deliveryItemID))
      this.lineValues.set(line.deliveryItemID, {
        deliveryItemID: line.deliveryItemID,
        quantityDelivered: line.quantityDelivered || line.quantityDispatched,
        quantityRejectedAtDelivery: line.quantityRejectedAtDelivery || 0,
      });
    return this.lineValues.get(line.deliveryItemID)!;
  }
  protected dispatch(): void {
    if (!this.deliveredByEmployeeID) {
      this.error.set('Select the delivery employee.');
      return;
    }
    this.api.dispatchDelivery(this.id, this.deliveredByEmployeeID).subscribe({
      next: () => {
        this.success.set('Delivery dispatched.');
        this.refresh();
      },
      error: () => this.error.set('Unable to dispatch the delivery.'),
    });
  }
  protected complete(): void {
    const request = {
      status: Number(this.completionStatus) as DeliveryStatus,
      receiverName: this.receiverName.trim() || null,
      receiverPhone: this.receiverPhone.trim() || null,
      failureReason: this.failureReason.trim() || null,
      rescheduledDate: this.rescheduledDate || null,
      items: this.item()?.items.map((x) => this.value(x)) ?? [],
    };
    this.api.completeDelivery(this.id, request).subscribe({
      next: () => {
        this.success.set('Delivery completion recorded.');
        this.refresh();
      },
      error: () => this.error.set('Unable to complete the delivery.'),
    });
  }
  private refresh(): void {
    this.api.getDelivery(this.id).subscribe({
      next: (x) => {
        this.item.set(x);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Unable to load delivery details.');
        this.loading.set(false);
      },
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
