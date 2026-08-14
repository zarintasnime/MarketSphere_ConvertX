import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import {
  CLIENT_LIFECYCLE_OPTIONS,
  CLIENT_RISK_OPTIONS,
  CLIENT_TYPE_OPTIONS,
  SALES_CHANNEL_OPTIONS,
  ClientDetails,
  ClientLifecycleStatus,
  ClientRiskStatus,
  ClientType,
  SalesChannel,
  optionLabel,
} from '../../models/crm.model';
import { CrmApiService } from '../../services/crm-api.service';

@Component({
  selector: 'app-client-details-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './client-details-page.component.html',
  styleUrl: './client-details-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ClientDetailsPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(CrmApiService);
  private readonly auth = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);

  protected readonly clientID = Number(this.route.snapshot.paramMap.get('clientID'));
  protected readonly client = signal<ClientDetails | null>(null);
  protected readonly loading = signal(false);
  protected readonly busy = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly canManage = computed(() => this.auth.hasPermission('crm.clients.manage'));
  protected readonly canManageCredit = computed(() =>
    this.auth.hasPermission('crm.client_credit.manage'),
  );
  protected readonly lifecycleOptions = CLIENT_LIFECYCLE_OPTIONS;

  protected readonly contactForm = this.fb.nonNullable.group({
    contactName: ['', Validators.required],
    designation: [''],
    phone: [''],
    email: ['', Validators.email],
    isPrimary: [false],
    isActive: [true],
  });
  protected readonly creditForm = this.fb.nonNullable.group({
    creditLimit: [0, Validators.min(0)],
    creditDays: [0, Validators.min(0)],
    currentDue: [0, Validators.min(0)],
    isBlocked: [false],
    blockReason: [''],
  });
  protected readonly lifecycleForm = this.fb.nonNullable.group({
    lifecycleStatus: [ClientLifecycleStatus.Active],
    reason: [''],
  });
  protected readonly segmentForm = this.fb.nonNullable.group({
    clientSegmentID: [0, Validators.min(1)],
    effectiveTo: [''],
  });
  protected readonly createSegmentForm = this.fb.nonNullable.group({
    segmentCode: ['', Validators.required],
    segmentName: ['', Validators.required],
    segmentType: [1],
    description: [''],
    isSystemSegment: [false],
    isActive: [true],
  });

  constructor() {
    this.load();
  }

  protected addContact(): void {
    if (this.contactForm.invalid) {
      this.contactForm.markAllAsTouched();
      return;
    }
    this.busy.set(true);
    const value = this.contactForm.getRawValue();
    this.api
      .addClientContact(this.clientID, {
        ...value,
        designation: value.designation || null,
        phone: value.phone || null,
        email: value.email || null,
      })
      .pipe(finalize(() => this.busy.set(false)))
      .subscribe({
        next: () => {
          this.contactForm.reset({
            contactName: '',
            designation: '',
            phone: '',
            email: '',
            isPrimary: false,
            isActive: true,
          });
          this.load();
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected saveCredit(): void {
    if (this.creditForm.invalid) return;
    this.busy.set(true);
    const value = this.creditForm.getRawValue();
    this.api
      .setClientCreditProfile(this.clientID, { ...value, blockReason: value.blockReason || null })
      .pipe(finalize(() => this.busy.set(false)))
      .subscribe({
        next: () => this.load(),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected changeLifecycle(): void {
    const value = this.lifecycleForm.getRawValue();
    this.busy.set(true);
    this.api
      .changeClientLifecycle(this.clientID, {
        lifecycleStatus: value.lifecycleStatus,
        reason: value.reason || null,
      })
      .pipe(finalize(() => this.busy.set(false)))
      .subscribe({
        next: () => this.load(),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected assignSegment(): void {
    if (this.segmentForm.invalid) return;
    const value = this.segmentForm.getRawValue();
    this.api
      .assignClientSegment(this.clientID, {
        clientSegmentID: value.clientSegmentID,
        effectiveTo: value.effectiveTo || null,
      })
      .subscribe({
        next: () => {
          this.segmentForm.reset({ clientSegmentID: 0, effectiveTo: '' });
          this.load();
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected createSegment(): void {
    if (this.createSegmentForm.invalid) return;
    const value = this.createSegmentForm.getRawValue();
    this.api.createClientSegment({ ...value, description: value.description || null }).subscribe({
      next: (segmentID) => {
        this.segmentForm.controls.clientSegmentID.setValue(segmentID);
        this.createSegmentForm.reset({
          segmentCode: '',
          segmentName: '',
          segmentType: 1,
          description: '',
          isSystemSegment: false,
          isActive: true,
        });
      },
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected endSegment(assignmentID: number): void {
    const date = new Date().toISOString();
    this.api.endClientSegmentAssignment(assignmentID, date).subscribe({
      next: () => this.load(),
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected clientTypeLabel(value: ClientType): string {
    return optionLabel(CLIENT_TYPE_OPTIONS, value);
  }
  protected channelLabel(value: SalesChannel): string {
    return optionLabel(SALES_CHANNEL_OPTIONS, value);
  }
  protected lifecycleLabel(value: ClientLifecycleStatus): string {
    return optionLabel(CLIENT_LIFECYCLE_OPTIONS, value);
  }
  protected riskLabel(value: ClientRiskStatus): string {
    return optionLabel(CLIENT_RISK_OPTIONS, value);
  }

  private load(): void {
    this.loading.set(true);
    this.api
      .getClient(this.clientID)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (item) => {
          this.client.set(item);
          this.lifecycleForm.setValue({ lifecycleStatus: item.lifecycleStatus, reason: '' });
          this.creditForm.setValue({
            creditLimit: item.creditProfile?.creditLimit ?? 0,
            creditDays: item.creditProfile?.creditDays ?? 0,
            currentDue: item.creditProfile?.currentDue ?? 0,
            isBlocked: item.creditProfile?.isBlocked ?? false,
            blockReason: item.creditProfile?.blockReason ?? '',
          });
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}
