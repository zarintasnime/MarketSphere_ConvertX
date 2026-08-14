import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
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
  ReturnRequestStatus,
  ReturnResolutionType,
  type CreateReturnRequest,
  type ReturnListItem,
} from '../../models/returns-payments.model';
import { ReturnsPaymentsApiService } from '../../services/returns-payments-api.service';

@Component({
  selector: 'app-return-requests-page',
  standalone: true,
  imports: [
    DatePipe,
    FormsModule,
    ReactiveFormsModule,
    RouterLink,
    EmptyStateComponent,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './return-requests-page.component.html',
  styleUrl: './return-requests-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ReturnRequestsPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly api = inject(ReturnsPaymentsApiService);
  private readonly crmApi = inject(CrmApiService);
  protected readonly rows = signal<readonly ReturnListItem[]>([]);
  protected readonly clients = signal<readonly ClientListItem[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal('');
  protected readonly totalCount = signal(0);
  protected readonly totalPages = signal(0);
  protected pageNumber = 1;
  protected readonly pageSize = 10;
  protected search = '';
  protected showCreate = Boolean(this.route.snapshot.queryParamMap.get('deliveryID'));
  protected readonly form = this.fb.group({
    returnNo: ['', Validators.required],
    clientID: [0, [Validators.required, Validators.min(1)]],
    orderID: [
      Number(this.route.snapshot.queryParamMap.get('orderID')) || 0,
      [Validators.required, Validators.min(1)],
    ],
    invoiceID: [Number(this.route.snapshot.queryParamMap.get('invoiceID')) || null],
    deliveryID: [Number(this.route.snapshot.queryParamMap.get('deliveryID')) || null],
    complaintID: [null],
    requestDate: ['', Validators.required],
    returnReason: ['', Validators.required],
    description: [''],
    items: this.fb.array([]),
  });
  protected get items(): FormArray {
    return this.form.controls.items as FormArray;
  }
  constructor() {
    this.addItem();
    this.load();
    this.crmApi
      .getClients({ pageNumber: 1, pageSize: 500 })
      .subscribe((r) => this.clients.set(r.items));
  }
  private createLine() {
    return this.fb.group({
      deliveryItemID: [0, [Validators.required, Validators.min(1)]],
      requestedQuantity: [1, [Validators.required, Validators.min(0.01)]],
    });
  }
  protected addItem(): void {
    this.items.push(this.createLine());
  }
  protected removeItem(i: number): void {
    if (this.items.length > 1) this.items.removeAt(i);
  }
  protected load(page = this.pageNumber): void {
    this.pageNumber = page;
    this.loading.set(true);
    this.api
      .getReturns({
        pageNumber: page,
        pageSize: this.pageSize,
        search: this.search,
        sortBy: 'RequestDate',
        sortDirection: 'desc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (r) => {
          this.rows.set(r.items);
          this.totalCount.set(r.totalCount);
          this.totalPages.set(r.totalPages);
        },
        error: () => this.error.set('Unable to load customer returns.'),
      });
  }
  protected create(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.getRawValue();
    const request: CreateReturnRequest = {
      returnNo: v.returnNo ?? '',
      clientID: Number(v.clientID),
      orderID: Number(v.orderID),
      invoiceID: v.invoiceID ? Number(v.invoiceID) : null,
      deliveryID: v.deliveryID ? Number(v.deliveryID) : null,
      complaintID: v.complaintID ? Number(v.complaintID) : null,
      requestDate: v.requestDate ?? '',
      returnReason: v.returnReason ?? '',
      description: v.description?.trim() || null,
      items: (v.items ?? []).map((x: any) => ({
        deliveryItemID: Number(x.deliveryItemID),
        requestedQuantity: Number(x.requestedQuantity),
      })),
    };
    this.api.createReturn(request).subscribe({
      next: (id) => this.router.navigate(['/returns-payments/returns', id]),
      error: () => this.error.set('Unable to create the customer return.'),
    });
  }
  protected statusLabel(v: ReturnRequestStatus): string {
    return ReturnRequestStatus[v] ?? 'Unknown';
  }
  protected resolutionLabel(v: ReturnResolutionType | null): string {
    return v ? ReturnResolutionType[v] : 'Pending';
  }
  protected tone(v: ReturnRequestStatus): StatusBadgeTone {
    if ([7, 8].includes(v)) return 'success';
    if (v === 4) return 'danger';
    if ([2, 3, 5, 6].includes(v)) return 'warning';
    return 'neutral';
  }
}
