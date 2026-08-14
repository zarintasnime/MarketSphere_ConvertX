import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { EmptyStateComponent } from '../../../../shared/components/empty-state.component';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { PaginationComponent } from '../../../../shared/components/pagination.component';
import {
  StatusBadgeComponent,
  type StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import type { ClientListItem } from '../../../crm/models/crm.model';
import { CrmApiService } from '../../../crm/services/crm-api.service';
import {
  CustomerPaymentStatus,
  PaymentMethod,
  type CreatePaymentRequest,
  type PaymentListItem,
} from '../../models/returns-payments.model';
import { ReturnsPaymentsApiService } from '../../services/returns-payments-api.service';

@Component({
  selector: 'app-payments-page',
  standalone: true,
  imports: [
    DatePipe,
    DecimalPipe,
    FormsModule,
    ReactiveFormsModule,
    RouterLink,
    EmptyStateComponent,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './payments-page.component.html',
  styleUrl: './payments-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PaymentsPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly api = inject(ReturnsPaymentsApiService);
  private readonly crmApi = inject(CrmApiService);
  protected readonly rows = signal<readonly PaymentListItem[]>([]);
  protected readonly clients = signal<readonly ClientListItem[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal('');
  protected readonly totalCount = signal(0);
  protected readonly totalPages = signal(0);
  protected pageNumber = 1;
  protected readonly pageSize = 10;
  protected search = '';
  protected showCreate = Boolean(this.route.snapshot.queryParamMap.get('clientID'));
  protected readonly methodOptions = Object.entries(PaymentMethod).filter(([x]) =>
    Number.isNaN(Number(x)),
  );
  protected readonly form = this.fb.group({
    paymentNo: ['', Validators.required],
    clientID: [
      Number(this.route.snapshot.queryParamMap.get('clientID')) || 0,
      [Validators.required, Validators.min(1)],
    ],
    paymentDate: ['', Validators.required],
    paymentMethod: [PaymentMethod.Cash],
    amount: [0, [Validators.required, Validators.min(0.01)]],
    referenceNo: [''],
    proofFileAttachmentID: [null],
  });
  constructor() {
    this.load();
    this.crmApi
      .getClients({ pageNumber: 1, pageSize: 500 })
      .subscribe((r) => this.clients.set(r.items));
  }
  protected load(page = this.pageNumber): void {
    this.pageNumber = page;
    this.loading.set(true);
    this.api
      .getPayments({
        pageNumber: page,
        pageSize: this.pageSize,
        search: this.search,
        sortBy: 'PaymentDate',
        sortDirection: 'desc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (r) => {
          this.rows.set(r.items);
          this.totalCount.set(r.totalCount);
          this.totalPages.set(r.totalPages);
        },
        error: () => this.error.set('Unable to load customer payments.'),
      });
  }
  protected create(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.getRawValue();
    const request: CreatePaymentRequest = {
      paymentNo: v.paymentNo ?? '',
      clientID: Number(v.clientID),
      paymentDate: v.paymentDate ?? '',
      paymentMethod: Number(v.paymentMethod) as PaymentMethod,
      amount: Number(v.amount),
      referenceNo: v.referenceNo?.trim() || null,
      proofFileAttachmentID: v.proofFileAttachmentID ? Number(v.proofFileAttachmentID) : null,
    };
    this.api
      .createPayment(request)
      .subscribe({
        next: (id) => this.router.navigate(['/returns-payments/payments', id, 'allocations']),
        error: () => this.error.set('Unable to create the customer payment.'),
      });
  }
  protected methodLabel(v: PaymentMethod): string {
    return PaymentMethod[v] ?? 'Unknown';
  }
  protected statusLabel(v: CustomerPaymentStatus): string {
    return CustomerPaymentStatus[v] ?? 'Unknown';
  }
  protected tone(v: CustomerPaymentStatus): StatusBadgeTone {
    if (v === 2) return 'success';
    if ([3, 4].includes(v)) return 'danger';
    return 'warning';
  }
}
