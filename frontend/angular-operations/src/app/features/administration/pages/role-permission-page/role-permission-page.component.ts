import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';

import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import type { PermissionItem, RoleDetails } from '../../models/administration.model';
import { AdministrationApiService } from '../../services/administration-api.service';

interface PermissionGroup {
  moduleName: string;
  permissions: readonly PermissionItem[];
}

@Component({
  selector: 'app-role-permission-page',
  standalone: true,
  imports: [LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './role-permission-page.component.html',
  styleUrl: './role-permission-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RolePermissionPageComponent {
  private readonly api = inject(AdministrationApiService);
  protected readonly router = inject(Router);
  private readonly roleID = Number(inject(ActivatedRoute).snapshot.paramMap.get('roleID'));

  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly role = signal<RoleDetails | null>(null);
  protected readonly selectedPermissionIDs = signal<readonly number[]>([]);
  protected readonly groups = computed<readonly PermissionGroup[]>(() => {
    const permissions = this.role()?.permissions ?? [];
    const map = new Map<string, PermissionItem[]>();
    permissions.forEach((permission) =>
      map.set(permission.moduleName, [...(map.get(permission.moduleName) ?? []), permission]),
    );
    return [...map.entries()].map(([moduleName, items]) => ({ moduleName, permissions: items }));
  });

  constructor() {
    this.load();
  }

  protected toggle(permissionID: number, checked: boolean): void {
    const selected = this.selectedPermissionIDs();
    this.selectedPermissionIDs.set(
      checked ? [...selected, permissionID] : selected.filter((id) => id !== permissionID),
    );
  }

  protected toggleGroup(group: PermissionGroup, checked: boolean): void {
    const ids = new Set(this.selectedPermissionIDs());
    group.permissions.forEach((permission) =>
      checked ? ids.add(permission.permissionID) : ids.delete(permission.permissionID),
    );
    this.selectedPermissionIDs.set([...ids]);
  }

  protected isSelected(permissionID: number): boolean {
    return this.selectedPermissionIDs().includes(permissionID);
  }
  protected isGroupSelected(group: PermissionGroup): boolean {
    return (
      group.permissions.length > 0 &&
      group.permissions.every((permission) => this.isSelected(permission.permissionID))
    );
  }

  protected save(): void {
    this.saving.set(true);
    this.errorMessage.set('');
    this.api
      .updateRolePermissions(this.roleID, this.selectedPermissionIDs())
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => this.load(),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  private load(): void {
    this.loading.set(true);
    this.api
      .getRole(this.roleID)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (role) => {
          this.role.set(role);
          this.selectedPermissionIDs.set(
            role.permissions
              .filter((permission) => permission.isAllowed)
              .map((permission) => permission.permissionID),
          );
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}
