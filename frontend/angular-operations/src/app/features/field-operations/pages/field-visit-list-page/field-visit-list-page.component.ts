import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { createEmptyPagedResult } from '../../../../core/models/paged-result.model';
import type { FieldVisitListItem } from '../../models/field-operations.model';
import { FieldOperationsApiService } from '../../services/field-operations-api.service';
@Component({
  selector: 'app-field-visit-list-page',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './field-visit-list-page.component.html',
  styleUrl: './field-visit-list-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FieldVisitListPageComponent {
  private readonly api = inject(FieldOperationsApiService);
  protected readonly result = signal(createEmptyPagedResult<FieldVisitListItem>());
  protected readonly page = signal(1);
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal('');
  constructor() {
    this.load();
  }
  protected load(page = this.page()): void {
    this.page.set(page);
    this.loading.set(true);
    this.api
      .getMyVisits(page, 20)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  protected status(value: number): string {
    return ['Checked in', 'Completed', 'Cancelled'][value] ?? `Status ${value}`;
  }
}
