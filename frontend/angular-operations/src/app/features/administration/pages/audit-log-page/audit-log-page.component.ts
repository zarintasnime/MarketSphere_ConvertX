import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';

import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { createEmptyPagedResult } from '../../../../core/models/paged-result.model';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { PaginationComponent } from '../../../../shared/components/pagination.component';
import type { AuditLogItem } from '../../models/administration.model';
import { AdministrationApiService } from '../../services/administration-api.service';

@Component({
  selector: 'app-audit-log-page',
  standalone: true,
  imports: [DatePipe, FormsModule, LoadingPanelComponent, PageHeaderComponent, PaginationComponent],
  templateUrl: './audit-log-page.component.html',
  styleUrl: './audit-log-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AuditLogPageComponent {
  private readonly api = inject(AdministrationApiService);
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly search = signal('');
  protected readonly result = signal(createEmptyPagedResult<AuditLogItem>(1, 15));
  protected readonly expandedID = signal<number | null>(null);

  constructor() {
    this.load();
  }

  protected load(pageNumber = this.result().pageNumber): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.api
      .getAuditLogs({
        pageNumber,
        pageSize: this.result().pageSize,
        search: this.search(),
        sortBy: 'CreatedAt',
        sortDirection: 'desc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected toggle(log: AuditLogItem): void {
    this.expandedID.set(this.expandedID() === log.auditLogID ? null : log.auditLogID);
  }
  protected prettyJson(value: string | null): string {
    if (!value) return 'No value';
    try {
      return JSON.stringify(JSON.parse(value), null, 2);
    } catch {
      return value;
    }
  }
}
