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
import { AdministrationApiService } from '../../services/administration-api.service';
import { UserListItem, UserStatus, userStatusLabel } from '../../models/administration.model';

@Component({
  selector: 'app-users-list-page',
  standalone: true,
  imports: [
    FormsModule,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './users-list-page.component.html',
  styleUrl: './users-list-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UsersListPageComponent {
  private readonly api = inject(AdministrationApiService);
  private readonly auth = inject(AuthService);
  protected readonly router = inject(Router);

  protected readonly loading = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly result = signal(createEmptyPagedResult<UserListItem>(1, 10));
  protected readonly search = signal('');
  protected readonly canCreate = computed(() => this.auth.hasPermission('users.create'));
  protected readonly canUpdate = computed(() => this.auth.hasPermission('users.update'));
  protected readonly canChangeStatus = computed(() =>
    this.auth.hasPermission('users.change_status'),
  );

  constructor() {
    this.load();
  }

  protected load(pageNumber = this.result().pageNumber): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.api
      .getUsers({
        pageNumber,
        pageSize: this.result().pageSize,
        search: this.search(),
        sortBy: 'FullName',
        sortDirection: 'asc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected edit(user: UserListItem): void {
    if (this.canUpdate()) void this.router.navigate(['/administration/users', user.userID, 'edit']);
  }

  protected changeStatus(user: UserListItem): void {
    const nextStatus = user.status === UserStatus.Active ? UserStatus.Disabled : UserStatus.Active;
    if (!confirm(`Change ${user.fullName} to ${userStatusLabel(nextStatus)}?`)) return;
    this.api.changeUserStatus(user.userID, nextStatus).subscribe({
      next: () => this.load(),
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected statusLabel(status: UserStatus): string {
    return userStatusLabel(status);
  }
  protected statusTone(status: UserStatus): StatusBadgeTone {
    return status === UserStatus.Active
      ? 'success'
      : status === UserStatus.Locked
        ? 'danger'
        : status === UserStatus.Invited
          ? 'info'
          : 'neutral';
  }
}
