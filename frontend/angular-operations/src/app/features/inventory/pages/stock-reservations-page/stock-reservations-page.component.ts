import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { EmptyStateComponent } from '../../../../shared/components/empty-state.component';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import {
  StatusBadgeComponent,
  type StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import { StockReservationStatus, type StockReservation } from '../../models/inventory.model';
import { InventoryApiService } from '../../services/inventory-api.service';

@Component({
  selector: 'app-stock-reservations-page',
  standalone: true,
  imports: [
    DatePipe,
    DecimalPipe,
    FormsModule,
    EmptyStateComponent,
    LoadingPanelComponent,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './stock-reservations-page.component.html',
  styleUrl: './stock-reservations-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StockReservationsPageComponent {
  private readonly api = inject(InventoryApiService);
  protected readonly rows = signal<readonly StockReservation[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal('');
  protected orderItemID: number | null = null;
  constructor() {
    this.load();
  }
  protected load(): void {
    this.loading.set(true);
    this.api
      .getReservations(this.orderItemID)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (rows) => this.rows.set(rows),
        error: () => this.error.set('Unable to load stock reservations.'),
      });
  }
  protected label(value: StockReservationStatus): string {
    return StockReservationStatus[value] ?? 'Unknown';
  }
  protected tone(value: StockReservationStatus): StatusBadgeTone {
    return value === StockReservationStatus.Active
      ? 'success'
      : value === StockReservationStatus.Consumed
        ? 'info'
        : value === StockReservationStatus.Expired || value === StockReservationStatus.Cancelled
          ? 'danger'
          : 'neutral';
  }
}
