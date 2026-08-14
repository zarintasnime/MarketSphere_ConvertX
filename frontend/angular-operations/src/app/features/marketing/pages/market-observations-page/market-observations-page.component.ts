import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, type Observable } from 'rxjs';
import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { createEmptyPagedResult } from '../../../../core/models/paged-result.model';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import type { MarketObservationListItem } from '../../models/marketing.model';
import { MarketingApiService } from '../../services/marketing-api.service';
@Component({
  selector: 'app-market-observations-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './market-observations-page.component.html',
  styleUrl: './market-observations-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MarketObservationsPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(MarketingApiService);
  private readonly auth = inject(AuthService);
  protected readonly result = signal(createEmptyPagedResult<MarketObservationListItem>());
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly editingID = signal<number | null>(null);
  protected readonly canManage = computed(() =>
    this.auth.hasPermission('marketing.market_observations.manage'),
  );
  protected readonly form = this.fb.nonNullable.group({
    visitID: [0, [Validators.required, Validators.min(1)]],
    clientID: [0, [Validators.required, Validators.min(1)]],
    employeeID: [
      this.auth.currentUser()?.employeeID ?? 0,
      [Validators.required, Validators.min(1)],
    ],
    observationType: [0],
    skuID: [null as number | null],
    availabilityStatus: [null as number | null],
    facingCount: [null as number | null],
    planogramScore: [null as number | null],
    displayScore: [null as number | null],
    competitorBrand: [''],
    competitorProduct: [''],
    competitorPrice: [null as number | null],
    competitorOffer: [''],
    note: [''],
  });
  constructor() {
    this.load();
  }
  protected load(): void {
    this.loading.set(true);
    this.api
      .getMarketObservations({ pageNumber: 1, pageSize: 100, sortDirection: 'desc' })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  protected edit(item: MarketObservationListItem): void {
    this.editingID.set(item.marketObservationID);
    this.form.patchValue(item as any);
  }
  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const request = (() => {
      const value = this.form.getRawValue();
      return {
        ...value,
        competitorBrand: value.competitorBrand || null,
        competitorProduct: value.competitorProduct || null,
        competitorOffer: value.competitorOffer || null,
        note: value.note || null,
      };
    })();
    const id = this.editingID();
    const operation: Observable<number | boolean> = id
      ? this.api.updateMarketObservation(id, request)
      : this.api.createMarketObservation(request);
    operation.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.reset();
        this.load();
      },
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }
  protected remove(item: MarketObservationListItem): void {
    if (!confirm('Delete this record?')) return;
    this.api
      .deleteMarketObservation(item.marketObservationID)
      .subscribe({
        next: () => this.load(),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  protected reset(): void {
    this.editingID.set(null);
    this.form.reset({
      visitID: 0,
      clientID: 0,
      employeeID: this.auth.currentUser()?.employeeID ?? 0,
      observationType: 0,
      skuID: null,
      availabilityStatus: null,
      facingCount: null,
      planogramScore: null,
      displayScore: null,
      competitorBrand: '',
      competitorProduct: '',
      competitorPrice: null,
      competitorOffer: '',
      note: '',
    });
  }
}

