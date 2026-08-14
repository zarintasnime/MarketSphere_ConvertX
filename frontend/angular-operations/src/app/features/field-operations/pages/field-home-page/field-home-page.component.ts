import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import type { FieldWorkspaceSummary } from '../../models/field-operations.model';
import { FieldOperationsApiService } from '../../services/field-operations-api.service';
@Component({
  selector: 'app-field-home-page',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './field-home-page.component.html',
  styleUrl: './field-home-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FieldHomePageComponent {
  private readonly api = inject(FieldOperationsApiService);
  protected readonly summary = signal<FieldWorkspaceSummary | null>(null);
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal('');
  constructor() {
    this.load();
  }
  protected load(): void {
    this.loading.set(true);
    this.api
      .getSummary()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (summary) => this.summary.set(summary),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}
