import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import {
  StatusBadgeComponent,
  type StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import type { Warehouse } from '../../../inventory/models/inventory.model';
import { InventoryApiService } from '../../../inventory/services/inventory-api.service';
import {
  AppliedBenefitType,
  CreditCheckStatus,
  OrderSource,
  OrderStatus,
  SalesChannel,
  type AppliedOffer,
  type ApplyOfferRequest,
  type OrderDetails,
} from '../../models/orders.model';
import { OrdersApiService } from '../../services/orders-api.service';

@Component({
  selector: 'app-order-details-page',
  standalone: true,
  imports: [
    DatePipe,
    DecimalPipe,
    FormsModule,
    RouterLink,
    LoadingPanelComponent,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './order-details-page.component.html',
  styleUrl: './order-details-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OrderDetailsPageComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(OrdersApiService);
  private readonly inventoryApi = inject(InventoryApiService);
  protected readonly item = signal<OrderDetails | null>(null);
  protected readonly offers = signal<readonly AppliedOffer[]>([]);
  protected readonly warehouses = signal<readonly Warehouse[]>([]);
  protected readonly loading = signal(true);
  protected readonly error = signal('');
  protected readonly success = signal('');
  protected warehouseID = 0;
  protected reservationExpiresAt = '';
  protected nextStatus = OrderStatus.Cancelled;
  protected campaignOfferID = 0;
  protected orderItemID: number | null = null;
  protected benefitType = AppliedBenefitType.PercentageDiscount;
  protected benefitAmount: number | null = null;
  protected freeSKUID: number | null = null;
  protected freeQuantity: number | null = null;
  protected ruleSnapshotJson = '{}';
  protected readonly statusOptions = Object.entries(OrderStatus).filter(([x]) =>
    Number.isNaN(Number(x)),
  );
  protected readonly benefitOptions = Object.entries(AppliedBenefitType).filter(([x]) =>
    Number.isNaN(Number(x)),
  );
  private readonly id = Number(this.route.snapshot.paramMap.get('id'));
  constructor() {
    forkJoin({
      order: this.api.getOrder(this.id),
      offers: this.api.getAppliedOffers(this.id),
      warehouses: this.inventoryApi.getWarehouses(),
    }).subscribe({
      next: (r) => {
        this.item.set(r.order);
        this.offers.set(r.offers);
        this.warehouses.set(r.warehouses.filter((x) => x.isActive));
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Unable to load order details.');
        this.loading.set(false);
      },
    });
  }
  protected submit(): void {
    this.api.submitOrder(this.id).subscribe({
      next: () => {
        this.success.set('Order submitted.');
        this.refresh();
      },
      error: () => this.error.set('Unable to submit the order.'),
    });
  }
  protected approveAndReserve(): void {
    if (!this.warehouseID) {
      this.error.set('Select a warehouse.');
      return;
    }
    this.api
      .approveAndReserveOrder(this.id, {
        warehouseID: this.warehouseID,
        approvalRequestID: this.item()?.approvalRequestID ?? null,
        reservationExpiresAt: this.reservationExpiresAt || null,
      })
      .subscribe({
        next: () => {
          this.success.set('Order approved and stock reserved.');
          this.refresh();
        },
        error: () => this.error.set('Unable to approve and reserve the order.'),
      });
  }
  protected changeStatus(): void {
    this.api.changeOrderStatus(this.id, Number(this.nextStatus) as OrderStatus).subscribe({
      next: () => {
        this.success.set('Order status updated.');
        this.refresh();
      },
      error: () => this.error.set('Unable to update order status.'),
    });
  }
  protected applyOffer(): void {
    if (!this.campaignOfferID) {
      this.error.set('Enter a campaign offer ID.');
      return;
    }
    const request: ApplyOfferRequest = {
      quotationID: null,
      quotationItemID: null,
      orderID: this.id,
      orderItemID: this.orderItemID,
      campaignOfferID: this.campaignOfferID,
      benefitType: Number(this.benefitType) as AppliedBenefitType,
      benefitAmount: this.benefitAmount,
      freeSKUID: this.freeSKUID,
      freeQuantity: this.freeQuantity,
      ruleSnapshotJson: this.ruleSnapshotJson || '{}',
      usageCount: 1,
    };
    this.api.applyOffer(request).subscribe({
      next: () => {
        this.success.set('Offer applied.');
        this.refreshOffers();
      },
      error: () => this.error.set('Unable to apply the offer.'),
    });
  }
  protected removeOffer(id: number): void {
    this.api.removeAppliedOffer(id).subscribe({
      next: () => {
        this.success.set('Offer removed.');
        this.refreshOffers();
      },
      error: () => this.error.set('Unable to remove the offer.'),
    });
  }
  private refresh(): void {
    this.api.getOrder(this.id).subscribe((x) => this.item.set(x));
    this.refreshOffers();
  }
  private refreshOffers(): void {
    this.api.getAppliedOffers(this.id).subscribe((x) => this.offers.set(x));
  }
  protected orderLabel(value: OrderStatus): string {
    return OrderStatus[value] ?? 'Unknown';
  }
  protected channelLabel(value: SalesChannel): string {
    return SalesChannel[value] ?? 'Unknown';
  }
  protected sourceLabel(value: OrderSource): string {
    return OrderSource[value] ?? 'Unknown';
  }
  protected creditLabel(value: CreditCheckStatus): string {
    return CreditCheckStatus[value] ?? 'Unknown';
  }
  protected benefitLabel(value: AppliedBenefitType): string {
    return AppliedBenefitType[value] ?? 'Unknown';
  }
  protected tone(status: OrderStatus): StatusBadgeTone {
    if ([4, 5, 6, 7, 8, 9, 11].includes(status)) return 'success';
    if ([12, 13].includes(status)) return 'danger';
    if ([2, 3].includes(status)) return 'warning';
    return 'neutral';
  }
}
