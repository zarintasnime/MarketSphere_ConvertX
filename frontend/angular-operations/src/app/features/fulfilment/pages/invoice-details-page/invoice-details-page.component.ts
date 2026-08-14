import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import {
  StatusBadgeComponent,
  type StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import { InvoiceStatus, type InvoiceDetails } from '../../models/fulfilment.model';
import { FulfilmentApiService } from '../../services/fulfilment-api.service';

@Component({
  selector: 'app-invoice-details-page',
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
  templateUrl: './invoice-details-page.component.html',
  styleUrl: './invoice-details-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InvoiceDetailsPageComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(FulfilmentApiService);
  protected readonly item = signal<InvoiceDetails | null>(null);
  protected readonly loading = signal(true);
  protected readonly error = signal('');
  protected readonly success = signal('');
  protected nextStatus = InvoiceStatus.Issued;
  protected readonly options = Object.entries(InvoiceStatus).filter(([x]) =>
    Number.isNaN(Number(x)),
  );
  private readonly id = Number(this.route.snapshot.paramMap.get('id'));
  constructor() {
    this.refresh();
  }
  protected changeStatus(): void {
    this.api.changeInvoiceStatus(this.id, Number(this.nextStatus) as InvoiceStatus).subscribe({
      next: () => {
        this.success.set('Invoice status updated.');
        this.refresh();
      },
      error: () => this.error.set('Unable to update invoice status.'),
    });
  }
  private refresh(): void {
    this.api.getInvoice(this.id).subscribe({
      next: (x) => {
        this.item.set(x);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Unable to load invoice details.');
        this.loading.set(false);
      },
    });
  }
  protected label(v: InvoiceStatus): string {
    return InvoiceStatus[v] ?? 'Unknown';
  }
  protected tone(v: InvoiceStatus): StatusBadgeTone {
    if (v === 4) return 'success';
    if (v === 7) return 'danger';
    if ([2, 3, 5, 6].includes(v)) return 'warning';
    return 'neutral';
  }
}
