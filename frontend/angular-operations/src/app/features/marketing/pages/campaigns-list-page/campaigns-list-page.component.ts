import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { createEmptyPagedResult } from '../../../../core/models/paged-result.model';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { StatusBadgeComponent } from '../../../../shared/components/status-badge.component';
import {
  CAMPAIGN_STATUS_OPTIONS,
  SALES_CHANNEL_OPTIONS,
  optionLabel,
  type CampaignListItem,
} from '../../models/marketing.model';
import { MarketingApiService } from '../../services/marketing-api.service';

@Component({
  selector: 'app-campaigns-list-page',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    LoadingPanelComponent,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './campaigns-list-page.component.html',
  styleUrl: './campaigns-list-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CampaignsListPageComponent {
  private readonly api = inject(MarketingApiService);
  private readonly auth = inject(AuthService);
  protected readonly result = signal(createEmptyPagedResult<CampaignListItem>());
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly search = signal('');
  protected readonly pageNumber = signal(1);
  protected readonly pageSize = 15;
  protected readonly canManage = computed(() =>
    this.auth.hasPermission('marketing.campaigns.manage'),
  );
  protected readonly canApprove = computed(() =>
    this.auth.hasPermission('marketing.campaigns.approve'),
  );

  constructor() {
    this.load();
  }
  protected load(pageNumber = this.pageNumber()): void {
    this.pageNumber.set(pageNumber);
    this.loading.set(true);
    this.errorMessage.set('');
    this.api
      .getCampaigns({
        pageNumber,
        pageSize: this.pageSize,
        search: this.search(),
        sortBy: 'StartDate',
        sortDirection: 'desc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  protected applySearch(): void {
    this.load(1);
  }
  protected changeStatus(item: CampaignListItem, status: number): void {
    this.api
      .changeCampaignStatus(item.campaignID, status, null)
      .subscribe({
        next: () => this.load(),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  protected statusLabel(value: number): string {
    return optionLabel(CAMPAIGN_STATUS_OPTIONS, value);
  }
  protected channelLabel(value: number): string {
    return optionLabel(SALES_CHANNEL_OPTIONS, value);
  }
  protected tone(value: number): 'neutral' | 'info' | 'warning' | 'success' | 'danger' {
    return value === 3 ? 'success' : value === 5 ? 'danger' : value === 1 ? 'warning' : 'info';
  }
}
