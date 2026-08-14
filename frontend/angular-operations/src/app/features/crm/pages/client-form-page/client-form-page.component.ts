import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize, type Observable } from 'rxjs';

import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import {
  CLIENT_LIFECYCLE_OPTIONS,
  CLIENT_RISK_OPTIONS,
  CLIENT_TYPE_OPTIONS,
  SALES_CHANNEL_OPTIONS,
  ClientLifecycleStatus,
  ClientRiskStatus,
  ClientType,
  SalesChannel,
} from '../../models/crm.model';
import { CrmApiService } from '../../services/crm-api.service';

@Component({
  selector: 'app-client-form-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './client-form-page.component.html',
  styleUrl: './client-form-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ClientFormPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(CrmApiService);
  private readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);

  protected readonly clientID = Number(this.route.snapshot.paramMap.get('clientID')) || null;
  protected readonly isEdit = computed(() => this.clientID !== null);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly clientTypeOptions = CLIENT_TYPE_OPTIONS;
  protected readonly channelOptions = SALES_CHANNEL_OPTIONS;
  protected readonly lifecycleOptions = CLIENT_LIFECYCLE_OPTIONS;
  protected readonly riskOptions = CLIENT_RISK_OPTIONS;

  protected readonly form = this.fb.nonNullable.group({
    clientCode: ['', [Validators.required, Validators.maxLength(50)]],
    clientName: ['', [Validators.required, Validators.maxLength(200)]],
    clientType: [ClientType.Outlet, Validators.required],
    channel: [SalesChannel.GeneralTrade, Validators.required],
    phone: [''],
    email: ['', Validators.email],
    address: ['', [Validators.required, Validators.maxLength(500)]],
    gpsLat: this.fb.control<number | null>(null),
    gpsLng: this.fb.control<number | null>(null),
    regionID: this.fb.control<number | null>(null),
    areaID: this.fb.control<number | null>(null),
    territoryID: this.fb.control<number | null>(null),
    lifecycleStatus: [ClientLifecycleStatus.Active, Validators.required],
    riskStatus: [ClientRiskStatus.Normal, Validators.required],
    isActive: [true],
  });

  constructor() {
    if (this.clientID) this.load();
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const value = this.form.getRawValue();
    const request = { ...value, phone: value.phone || null, email: value.email || null };
    const operation: Observable<number | boolean> = this.clientID
      ? this.api.updateClient(this.clientID, request)
      : this.api.createClient(request);
    operation.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: (result) => {
        const id = this.clientID ?? Number(result);
        void this.router.navigate(['/crm/clients', id || '']);
      },
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  private load(): void {
    this.loading.set(true);
    this.api
      .getClient(this.clientID!)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (item) =>
          this.form.setValue({
            clientCode: item.clientCode,
            clientName: item.clientName,
            clientType: item.clientType,
            channel: item.channel,
            phone: item.phone ?? '',
            email: item.email ?? '',
            address: item.address,
            gpsLat: item.gpsLat,
            gpsLng: item.gpsLng,
            regionID: item.regionID,
            areaID: item.areaID,
            territoryID: item.territoryID,
            lifecycleStatus: item.lifecycleStatus,
            riskStatus: item.riskStatus,
            isActive: item.isActive,
          }),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}

