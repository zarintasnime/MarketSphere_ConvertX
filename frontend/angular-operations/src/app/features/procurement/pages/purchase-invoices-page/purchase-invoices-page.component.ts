import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { EmptyStateComponent } from '../../../../shared/components/empty-state.component';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { PaginationComponent } from '../../../../shared/components/pagination.component';
import {
  StatusBadgeComponent,
  type StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import type { PagedRequest } from '../../../../core/models/paged-result.model';
import {
  PaymentMethod,
  PurchaseInvoiceStatus,
  SupplierInvoicePaymentStatus,
  SupplierPaymentStatus,
  type PurchaseInvoice,
  type SavePurchaseInvoiceRequest,
  type SupplierListItem,
  type SupplierPayment,
} from '../../models/procurement.model';
import { ProcurementApiService } from '../../services/procurement-api.service';

@Component({
  selector: 'app-purchase-invoices-page',
  standalone: true,
  imports: [
    DatePipe,
    DecimalPipe,
    FormsModule,
    ReactiveFormsModule,
    EmptyStateComponent,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './purchase-invoices-page.component.html',
  styleUrl: './purchase-invoices-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PurchaseInvoicesPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(ProcurementApiService);
  protected readonly rows = signal<readonly PurchaseInvoice[]>([]);
  protected readonly suppliers = signal<readonly SupplierListItem[]>([]);
  protected readonly payments = signal<readonly SupplierPayment[]>([]);
  protected readonly selectedInvoice = signal<PurchaseInvoice | null>(null);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly error = signal('');
  protected readonly totalCount = signal(0);
  protected readonly totalPages = signal(0);
  protected search = '';
  protected pageNumber = 1;
  protected readonly pageSize = 10;
  protected showCreate = false;
  protected readonly invoiceForm = this.fb.group({
    purchaseInvoiceNo: ['', Validators.required],
    supplierID: [0, [Validators.required, Validators.min(1)]],
    purchaseOrderID: [null as number | null],
    goodsReceiptID: [null as number | null],
    invoiceDate: ['', Validators.required],
    dueDate: [''],
    grossAmount: [0, [Validators.required, Validators.min(0)]],
    discountAmount: [0, [Validators.required, Validators.min(0)]],
    taxAmount: [0, [Validators.required, Validators.min(0)]],
  });
  protected readonly paymentForm = this.fb.group({
    paymentNo: ['', Validators.required],
    paymentDate: ['', Validators.required],
    paymentMethod: [PaymentMethod.BankTransfer, Validators.required],
    amount: [0, [Validators.required, Validators.min(0.01)]],
    referenceNo: [''],
  });
  protected readonly paymentMethods = Object.entries(PaymentMethod)
    .filter(([key]) => Number.isNaN(Number(key)))
    .map(([label, value]) => ({ label, value: Number(value) }));
  constructor() {
    this.loadSuppliers();
    this.load();
  }
  private loadSuppliers(): void {
    this.api
      .getSuppliers({ pageNumber: 1, pageSize: 500, sortBy: 'SupplierName', sortDirection: 'asc' })
      .subscribe({
        next: (r) => this.suppliers.set(r.items),
        error: () => this.error.set('Unable to load suppliers.'),
      });
  }
  protected load(page = this.pageNumber): void {
    this.pageNumber = page;
    this.loading.set(true);
    this.api
      .getPurchaseInvoices({
        pageNumber: page,
        pageSize: this.pageSize,
        search: this.search,
        sortBy: 'InvoiceDate',
        sortDirection: 'desc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (r) => {
          this.rows.set(r.items);
          this.totalCount.set(r.totalCount);
          this.totalPages.set(r.totalPages);
        },
        error: () => this.error.set('Unable to load purchase invoices.'),
      });
  }
  protected createInvoice(): void {
    if (this.invoiceForm.invalid) {
      this.invoiceForm.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const v = this.invoiceForm.getRawValue();
    const request: SavePurchaseInvoiceRequest = {
      purchaseInvoiceNo: v.purchaseInvoiceNo ?? '',
      supplierID: Number(v.supplierID),
      purchaseOrderID: v.purchaseOrderID ? Number(v.purchaseOrderID) : null,
      goodsReceiptID: v.goodsReceiptID ? Number(v.goodsReceiptID) : null,
      invoiceDate: v.invoiceDate ?? '',
      dueDate: v.dueDate || null,
      grossAmount: Number(v.grossAmount),
      discountAmount: Number(v.discountAmount),
      taxAmount: Number(v.taxAmount),
    };
    this.api
      .createPurchaseInvoice(request)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => {
          this.showCreate = false;
          this.invoiceForm.reset({
            supplierID: 0,
            purchaseOrderID: null,
            goodsReceiptID: null,
            grossAmount: 0,
            discountAmount: 0,
            taxAmount: 0,
          });
          this.load(1);
        },
        error: () => this.error.set('Unable to create the purchase invoice.'),
      });
  }
  protected confirm(item: PurchaseInvoice): void {
    this.api
      .confirmPurchaseInvoice(item.purchaseInvoiceID)
      .subscribe({
        next: () => this.load(),
        error: () => this.error.set('Unable to confirm the invoice.'),
      });
  }
  protected openPayments(item: PurchaseInvoice): void {
    this.selectedInvoice.set(item);
    this.api
      .getSupplierPayments(item.purchaseInvoiceID)
      .subscribe({
        next: (rows) => this.payments.set(rows),
        error: () => this.error.set('Unable to load supplier payments.'),
      });
  }
  protected createPayment(): void {
    const invoice = this.selectedInvoice();
    if (!invoice || this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      return;
    }
    const v = this.paymentForm.getRawValue();
    this.api
      .createSupplierPayment({
        purchaseInvoiceID: invoice.purchaseInvoiceID,
        paymentNo: v.paymentNo ?? '',
        paymentDate: v.paymentDate ?? '',
        paymentMethod: Number(v.paymentMethod) as PaymentMethod,
        amount: Number(v.amount),
        referenceNo: v.referenceNo || null,
      })
      .subscribe({
        next: () => {
          this.paymentForm.reset({ paymentMethod: PaymentMethod.BankTransfer, amount: 0 });
          this.openPayments(invoice);
          this.load();
        },
        error: () => this.error.set('Unable to create the supplier payment.'),
      });
  }
  protected setPaymentStatus(item: SupplierPayment, status: SupplierPaymentStatus): void {
    this.api.changeSupplierPaymentStatus(item.supplierPaymentID, status).subscribe({
      next: () => {
        const invoice = this.selectedInvoice();
        if (invoice) {
          this.openPayments(invoice);
        }
        this.load();
      },
      error: () => this.error.set('Unable to change payment status.'),
    });
  }
  protected invoiceStatus(value: PurchaseInvoiceStatus): string {
    return PurchaseInvoiceStatus[value] ?? 'Unknown';
  }
  protected paymentStatus(value: SupplierInvoicePaymentStatus): string {
    return SupplierInvoicePaymentStatus[value] ?? 'Unknown';
  }
  protected tone(value: SupplierInvoicePaymentStatus): StatusBadgeTone {
    return value === SupplierInvoicePaymentStatus.Paid
      ? 'success'
      : value === SupplierInvoicePaymentStatus.PartiallyPaid
        ? 'warning'
        : 'danger';
  }
}
