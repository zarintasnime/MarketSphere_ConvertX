import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import {
  CustomerPaymentStatus,
  PaymentAllocationType,
  PaymentMethod,
  type PaymentDetails,
} from '../../models/returns-payments.model';
import { ReturnsPaymentsApiService } from '../../services/returns-payments-api.service';
import type { InvoiceListItem } from '../../../fulfilment/models/fulfilment.model';
import { FulfilmentApiService } from '../../../fulfilment/services/fulfilment-api.service';

@Component({
  selector: 'app-payment-allocation-page',
  standalone: true,
  imports: [
    DatePipe,
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    LoadingPanelComponent,
    PageHeaderComponent,
  ],
  templateUrl: './payment-allocation-page.component.html',
  styleUrl: './payment-allocation-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PaymentAllocationPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ReturnsPaymentsApiService);
  private readonly fulfilmentApi = inject(FulfilmentApiService);
  protected readonly item = signal<PaymentDetails | null>(null);
  protected readonly invoices = signal<readonly InvoiceListItem[]>([]);
  protected readonly loading = signal(true);
  protected readonly error = signal('');
  protected readonly success = signal('');
  protected readonly form = this.fb.group({ allocations: this.fb.array([]) });
  protected get allocations(): FormArray {
    return this.form.controls.allocations as FormArray;
  }
  private readonly id = Number(this.route.snapshot.paramMap.get('id'));
  constructor() {
    this.refresh();
  }
  protected addAllocation(): void {
    this.allocations.push(
      this.fb.group({
        invoiceID: [0, [Validators.required, Validators.min(1)]],
        amount: [0, [Validators.required, Validators.min(0.01)]],
      }),
    );
  }
  protected removeAllocation(i: number): void {
    this.allocations.removeAt(i);
  }
  protected confirm(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const allocations = (this.form.getRawValue().allocations ?? []).map((x: any) => ({
      invoiceID: Number(x.invoiceID),
      amount: Number(x.amount),
    }));
    this.api.confirmPayment(this.id, { allocations }).subscribe({
      next: () => {
        this.success.set('Payment confirmed and allocated.');
        this.allocations.clear();
        this.refresh();
      },
      error: () => this.error.set('Unable to confirm the payment allocation.'),
    });
  }
  protected reverse(id: number): void {
    this.api.reverseAllocation(id).subscribe({
      next: () => {
        this.success.set('Payment allocation reversed.');
        this.refresh();
      },
      error: () => this.error.set('Unable to reverse the allocation.'),
    });
  }
  private refresh(): void {
    forkJoin({
      payment: this.api.getPayment(this.id),
      invoices: this.fulfilmentApi.getInvoices({
        pageNumber: 1,
        pageSize: 500,
        sortBy: 'InvoiceDate',
        sortDirection: 'desc',
      }),
    }).subscribe({
      next: (r) => {
        this.item.set(r.payment);
        this.invoices.set(
          r.invoices.items.filter((x) => x.clientID === r.payment.clientID && x.dueAmount > 0),
        );
        this.loading.set(false);
        if (!this.allocations.length && r.payment.status === CustomerPaymentStatus.Pending)
          this.addAllocation();
      },
      error: () => {
        this.error.set('Unable to load payment allocation details.');
        this.loading.set(false);
      },
    });
  }
  protected methodLabel(v: PaymentMethod): string {
    return PaymentMethod[v] ?? 'Unknown';
  }
  protected statusLabel(v: CustomerPaymentStatus): string {
    return CustomerPaymentStatus[v] ?? 'Unknown';
  }
  protected allocationLabel(v: PaymentAllocationType): string {
    return PaymentAllocationType[v] ?? 'Unknown';
  }
}
