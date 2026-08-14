import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { createEmptyPagedResult } from '../../../../core/models/paged-result.model';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { PaginationComponent } from '../../../../shared/components/pagination.component';
import {
  StatusBadgeComponent,
  StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import {
  EmployeeListItem,
  EmployeeStatus,
  employeeStatusLabel,
} from '../../models/administration.model';
import { AdministrationApiService } from '../../services/administration-api.service';

@Component({
  selector: 'app-employees-list-page',
  standalone: true,
  imports: [
    FormsModule,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './employees-list-page.component.html',
  styleUrl: './employees-list-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EmployeesListPageComponent {
  private readonly api = inject(AdministrationApiService);
  private readonly auth = inject(AuthService);
  protected readonly router = inject(Router);

  protected readonly loading = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly result = signal(createEmptyPagedResult<EmployeeListItem>(1, 10));
  protected readonly search = signal('');
  protected readonly canCreate = computed(() => this.auth.hasPermission('employees.create'));
  protected readonly canUpdate = computed(() => this.auth.hasPermission('employees.update'));

  constructor() {
    this.load();
  }

  protected load(pageNumber = this.result().pageNumber): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.api
      .getEmployees({
        pageNumber,
        pageSize: this.result().pageSize,
        search: this.search(),
        sortBy: 'EmployeeCode',
        sortDirection: 'asc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected edit(employee: EmployeeListItem): void {
    if (this.canUpdate())
      void this.router.navigate(['/administration/employees', employee.employeeID, 'edit']);
  }

  protected statusLabel(status: EmployeeStatus): string {
    return employeeStatusLabel(status);
  }
  protected statusTone(status: EmployeeStatus): StatusBadgeTone {
    return status === EmployeeStatus.Active
      ? 'success'
      : status === EmployeeStatus.Suspended
        ? 'warning'
        : status === EmployeeStatus.Terminated
          ? 'danger'
          : 'neutral';
  }
}
