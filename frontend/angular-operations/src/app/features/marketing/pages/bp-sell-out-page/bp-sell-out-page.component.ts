import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { createEmptyPagedResult } from '../../../../core/models/paged-result.model';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { StatusBadgeComponent } from '../../../../shared/components/status-badge.component';
import type { BpSellOutListItem } from '../../models/marketing.model';
import { MarketingApiService } from '../../services/marketing-api.service';
@Component({
  selector: 'app-bp-sell-out-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent, StatusBadgeComponent],
  templateUrl: './bp-sell-out-page.component.html',
  styleUrl: './bp-sell-out-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BpSellOutPageComponent {
  private readonly api = inject(MarketingApiService);
  private readonly auth = inject(AuthService);
  private readonly fb = inject(FormBuilder);
  protected readonly result = signal(createEmptyPagedResult<BpSellOutListItem>());
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly canVerify = computed(() =>
    this.auth.hasPermission('marketing.bp_sell_out.verify'),
  );
  protected readonly form = this.fb.nonNullable.group({
    recordID: [0, [Validators.required, Validators.min(1)]],
    verificationStatus: [1],
    note: [''],
  });
  constructor() {
    this.load();
  }
  protected load(): void {
    this.loading.set(true);
    this.api
      .getBpSellOut({ pageNumber: 1, pageSize: 100, sortBy: 'SellOutDate', sortDirection: 'desc' })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  protected verify(): void {
    if (this.form.invalid || !this.canVerify()) return;
    const value = this.form.getRawValue();
    const employeeID = this.auth.currentUser()?.employeeID;
    if (!employeeID) {
      this.errorMessage.set('The current account is not linked to an employee.');
      return;
    }
    this.saving.set(true);
    this.api
      .verifyBpSellOut(value.recordID, employeeID, value.verificationStatus, value.note || null)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => {
          this.form.reset({ recordID: 0, verificationStatus: 1, note: '' });
          this.load();
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  protected statusLabel(value: number): string {
    return ['Pending', 'Verified', 'Rejected'][value] ?? `Status ${value}`;
  }
}
