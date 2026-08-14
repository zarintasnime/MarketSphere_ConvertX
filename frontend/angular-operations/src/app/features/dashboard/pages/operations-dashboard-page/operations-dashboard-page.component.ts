import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { MetricCardComponent } from '../../../../shared/components/metric-card.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import type { DashboardQuickLink, OperationsDashboardSummary } from '../../models/dashboard.model';
import { DashboardApiService } from '../../services/dashboard-api.service';

@Component({
  selector: 'app-operations-dashboard-page',
  standalone: true,
  imports: [RouterLink, LoadingPanelComponent, MetricCardComponent, PageHeaderComponent],
  templateUrl: './operations-dashboard-page.component.html',
  styleUrl: './operations-dashboard-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OperationsDashboardPageComponent {
  private readonly dashboardApi = inject(DashboardApiService);
  private readonly auth = inject(AuthService);

  protected readonly loading = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly summary = signal<OperationsDashboardSummary | null>(null);
  protected readonly currentUser = this.auth.currentUser;

  protected readonly quickLinks: readonly DashboardQuickLink[] = [
    {
      label: 'Users',
      description: 'Create and maintain application users.',
      route: '/administration/users',
      permission: 'users.view',
    },
    {
      label: 'Roles',
      description: 'Manage roles and permission assignments.',
      route: '/administration/roles',
      permission: 'roles.view',
    },
    {
      label: 'Employees',
      description: 'Maintain employee access scope.',
      route: '/administration/employees',
      permission: 'employees.view',
    },
    {
      label: 'Branches',
      description: 'Review company branch information.',
      route: '/organization/branches',
      permission: 'organization.view',
    },
    {
      label: 'Geography',
      description: 'Maintain regions, areas, and territories.',
      route: '/organization/geography',
      permission: 'geography.view',
    },
    {
      label: 'Routes',
      description: 'Maintain field routes and assignments.',
      route: '/organization/routes',
      permission: 'routes.view',
    },
  ];

  protected readonly visibleQuickLinks = computed(() =>
    this.quickLinks.filter((link) => !link.permission || this.auth.hasPermission(link.permission)),
  );

  constructor() {
    this.loadSummary();
  }

  protected loadSummary(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.dashboardApi
      .getSummary()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (summary) => this.summary.set(summary),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}
