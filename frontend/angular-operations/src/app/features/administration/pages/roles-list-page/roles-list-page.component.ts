import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable, finalize } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { StatusBadgeComponent } from '../../../../shared/components/status-badge.component';
import type { RoleListItem } from '../../models/administration.model';
import { AdministrationApiService } from '../../services/administration-api.service';

@Component({
  selector: 'app-roles-list-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent, StatusBadgeComponent],
  templateUrl: './roles-list-page.component.html',
  styleUrl: './roles-list-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RolesListPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(AdministrationApiService);
  private readonly auth = inject(AuthService);
  protected readonly router = inject(Router);

  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly roles = signal<readonly RoleListItem[]>([]);
  protected readonly selectedRoleID = signal<number | null>(null);
  protected readonly canCreate = computed(() => this.auth.hasPermission('roles.create'));
  protected readonly canUpdate = computed(() => this.auth.hasPermission('roles.update'));
  protected readonly canManagePermissions = computed(() =>
    this.auth.hasPermission('roles.manage_permissions'),
  );

  protected readonly form = this.fb.nonNullable.group({
    roleCode: ['', [Validators.required, Validators.maxLength(50)]],
    roleName: ['', [Validators.required, Validators.maxLength(100)]],
    description: [''],
    roleLevel: [100, [Validators.required, Validators.min(1), Validators.max(1000)]],
    isActive: [true],
  });

  constructor() {
    this.load();
  }

  protected select(role: RoleListItem): void {
    if (!this.canUpdate()) return;
    this.loading.set(true);
    this.api
      .getRole(role.roleID)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (details) => {
          this.selectedRoleID.set(details.roleID);
          this.form.patchValue({
            roleCode: details.roleCode,
            roleName: details.roleName,
            description: details.description ?? '',
            roleLevel: details.roleLevel,
            isActive: details.isActive,
          });
          this.form.controls.roleCode.disable();
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected newRole(): void {
    this.selectedRoleID.set(null);
    this.form.reset({
      roleCode: '',
      roleName: '',
      description: '',
      roleLevel: 100,
      isActive: true,
    });
    this.form.controls.roleCode.enable();
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const raw = this.form.getRawValue();
    const roleID = this.selectedRoleID();
    let request$: Observable<unknown>;

    if (roleID) {
      request$ = this.api.updateRole(roleID, {
        roleName: raw.roleName,
        description: raw.description || null,
        roleLevel: raw.roleLevel,
        isActive: raw.isActive,
      });
    } else {
      request$ = this.api.createRole({
        roleCode: raw.roleCode,
        roleName: raw.roleName,
        description: raw.description || null,
        roleLevel: raw.roleLevel,
      });
    }

    this.saving.set(true);
    this.errorMessage.set('');
    request$.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.newRole();
        this.load();
      },
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected openPermissions(role: RoleListItem): void {
    void this.router.navigate(['/administration/roles', role.roleID, 'permissions']);
  }

  private load(): void {
    this.loading.set(true);
    this.api
      .getRoles()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (roles) => this.roles.set(roles),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}
