import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { createEmptyPagedResult } from '../../../../core/models/paged-result.model';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { StatusBadgeComponent } from '../../../../shared/components/status-badge.component';
import { VISIT_TYPE_OPTIONS, optionLabel, type VisitListItem } from '../../models/marketing.model';
import { MarketingApiService } from '../../services/marketing-api.service';
@Component({
  selector: 'app-visits-list-page',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    LoadingPanelComponent,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './visits-list-page.component.html',
  styleUrl: './visits-list-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class VisitsListPageComponent {
  private readonly api = inject(MarketingApiService);
  protected readonly result = signal(createEmptyPagedResult<VisitListItem>());
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly search = signal('');
  protected readonly pageNumber = signal(1);
  constructor() {
    this.load();
  }
  protected load(page = this.pageNumber()): void {
    this.pageNumber.set(page);
    this.loading.set(true);
    this.api
      .getVisits({
        pageNumber: page,
        pageSize: 20,
        search: this.search(),
        sortBy: 'CheckInAt',
        sortDirection: 'desc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  protected typeLabel(value: number): string {
    return optionLabel(VISIT_TYPE_OPTIONS, value);
  }
  protected statusLabel(value: number): string {
    return ['Checked in', 'Completed', 'Cancelled'][value] ?? `Status ${value}`;
  }
}
