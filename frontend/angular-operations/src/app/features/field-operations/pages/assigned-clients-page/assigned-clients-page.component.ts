import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { createEmptyPagedResult } from '../../../../core/models/paged-result.model';
import type { FieldAssignedClient } from '../../models/field-operations.model';
import { FieldOperationsApiService } from '../../services/field-operations-api.service';
@Component({
  selector: 'app-assigned-clients-page',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './assigned-clients-page.component.html',
  styleUrl: './assigned-clients-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AssignedClientsPageComponent {
  private readonly api = inject(FieldOperationsApiService);
  private readonly router = inject(Router);
  protected readonly result = signal(createEmptyPagedResult<FieldAssignedClient>());
  protected readonly search = signal('');
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
      .getAssignedClients(page, 20, this.search())
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  protected startVisit(client: FieldAssignedClient): void {
    void this.router.navigate(['/field/visit/check-in'], {
      queryParams: { clientID: client.clientID, routeID: client.routeID },
    });
  }
}
