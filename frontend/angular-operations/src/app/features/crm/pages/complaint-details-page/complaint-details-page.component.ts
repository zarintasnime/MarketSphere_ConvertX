import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize, forkJoin, of, type Observable } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { StatusBadgeComponent } from '../../../../shared/components/status-badge.component';
import {
  COMPLAINT_CATEGORY_OPTIONS,
  COMPLAINT_PRIORITY_OPTIONS,
  COMPLAINT_STATUS_OPTIONS,
  ClientListItem,
  ComplaintDetails,
  ComplaintPriority,
  ComplaintStatus,
  optionLabel,
} from '../../models/crm.model';
import { CrmApiService } from '../../services/crm-api.service';

@Component({
  selector: 'app-complaint-details-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    LoadingPanelComponent,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './complaint-details-page.component.html',
  styleUrl: './complaint-details-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ComplaintDetailsPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(CrmApiService);
  private readonly auth = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);

  protected readonly complaintID = Number(this.route.snapshot.paramMap.get('complaintID')) || null;
  protected readonly isNew = computed(() => this.complaintID === null);
  protected readonly canManage = computed(() => this.auth.hasPermission('crm.complaints.manage'));
  protected readonly details = signal<ComplaintDetails | null>(null);
  protected readonly clients = signal<readonly ClientListItem[]>([]);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly categoryOptions = COMPLAINT_CATEGORY_OPTIONS;
  protected readonly priorityOptions = COMPLAINT_PRIORITY_OPTIONS;
  protected readonly statusOptions = COMPLAINT_STATUS_OPTIONS;

  protected readonly form = this.fb.nonNullable.group({
    complaintNo: ['', Validators.required],
    clientID: [0, Validators.min(1)],
    orderID: this.fb.control<number | null>(null),
    invoiceID: this.fb.control<number | null>(null),
    deliveryID: this.fb.control<number | null>(null),
    complaintCategory: [COMPLAINT_CATEGORY_OPTIONS[0].value],
    priority: [ComplaintPriority.Normal],
    subject: ['', Validators.required],
    details: ['', Validators.required],
    assignedEmployeeID: this.fb.control<number | null>(null),
    slaDueAt: [''],
  });
  protected readonly statusForm = this.fb.nonNullable.group({
    status: [ComplaintStatus.Open],
    resolutionNote: [''],
    satisfactionScore: this.fb.control<number | null>(null),
  });

  constructor() {
    this.load();
  }

  protected save(): void {
    if (!this.canManage() || this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const value = this.form.getRawValue();
    const request = { ...value, slaDueAt: value.slaDueAt || null };
    const operation: Observable<number | boolean> = this.complaintID
      ? this.api.updateComplaint(this.complaintID, request)
      : this.api.createComplaint(request);
    operation.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: (result) => {
        const id = this.complaintID ?? Number(result);
        if (this.isNew()) void this.router.navigate(['/crm/complaints', id]);
        else this.load();
      },
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected changeStatus(): void {
    if (!this.complaintID || !this.canManage()) return;
    this.saving.set(true);
    const value = this.statusForm.getRawValue();
    this.api
      .changeComplaintStatus(
        this.complaintID,
        value.status,
        value.resolutionNote || null,
        value.satisfactionScore,
      )
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => this.load(),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected statusLabel(value: ComplaintStatus): string {
    return optionLabel(COMPLAINT_STATUS_OPTIONS, value);
  }

  private load(): void {
    this.loading.set(true);
    const details$ = this.complaintID
      ? this.api.getComplaint(this.complaintID)
      : of<ComplaintDetails | null>(null);
    forkJoin({
      details: details$,
      clients: this.api.getClients({
        pageNumber: 1,
        pageSize: 300,
        sortBy: 'ClientName',
        sortDirection: 'asc',
      }),
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: ({ details, clients }) => {
          this.clients.set(clients.items);
          if (!details) return;
          this.details.set(details);
          this.form.setValue({
            complaintNo: details.complaintNo,
            clientID: details.clientID,
            orderID: details.orderID,
            invoiceID: details.invoiceID,
            deliveryID: details.deliveryID,
            complaintCategory: details.complaintCategory,
            priority: details.priority,
            subject: details.subject,
            details: details.details,
            assignedEmployeeID: details.assignedEmployeeID,
            slaDueAt: details.slaDueAt ? details.slaDueAt.slice(0, 16) : '',
          });
          this.statusForm.setValue({
            status: details.status,
            resolutionNote: details.resolutionNote ?? '',
            satisfactionScore: details.satisfactionScore,
          });
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}

