import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, forkJoin, finalize, of, switchMap } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { AdministrationApiService } from '../../services/administration-api.service';
import type { RoleListItem, UserDetails } from '../../models/administration.model';

@Component({
  selector: 'app-user-form-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './user-form-page.component.html',
  styleUrl: './user-form-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserFormPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(AdministrationApiService);
  private readonly auth = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);

  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly roles = signal<readonly RoleListItem[]>([]);
  protected readonly userID = Number(this.route.snapshot.paramMap.get('userID')) || null;
  protected readonly isEdit = computed(() => this.userID !== null);
  protected readonly canAssignRoles = computed(
    () => !this.isEdit() || this.auth.hasPermission('users.assign_roles'),
  );

  protected readonly form = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.maxLength(150)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(256)]],
    phone: [''],
    temporaryPassword: [''],
    activateImmediately: [true],
    roleIDs: this.fb.nonNullable.control<number[]>([]),
  });

  constructor() {
    this.load();
  }

  protected toggleRole(roleID: number, checked: boolean): void {
    const current = this.form.controls.roleIDs.value;
    this.form.controls.roleIDs.setValue(
      checked ? [...current, roleID] : current.filter((id) => id !== roleID),
    );
  }

  protected isRoleSelected(roleID: number): boolean {
    return this.form.controls.roleIDs.value.includes(roleID);
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    if (!this.isEdit() && !this.form.controls.temporaryPassword.value) {
      this.form.controls.temporaryPassword.setErrors({ required: true });
      return;
    }

    this.saving.set(true);
    this.errorMessage.set('');
    const value = this.form.getRawValue();
    let request$: Observable<unknown>;

    if (this.userID) {
      const update$ = this.api.updateUser(this.userID, {
        fullName: value.fullName,
        email: value.email,
        phone: value.phone || null,
      });
      request$ = this.canAssignRoles()
        ? update$.pipe(switchMap(() => this.api.assignUserRoles(this.userID!, value.roleIDs)))
        : update$;
    } else {
      request$ = this.api.createUser({
        fullName: value.fullName,
        email: value.email,
        phone: value.phone || null,
        temporaryPassword: value.temporaryPassword,
        activateImmediately: value.activateImmediately,
        roleIDs: value.roleIDs,
      });
    }

    request$.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => void this.router.navigate(['/administration/users']),
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  private load(): void {
    this.loading.set(true);
    const user$: Observable<UserDetails | null> = this.userID
      ? this.api.getUser(this.userID)
      : of(null);
    const roles$: Observable<readonly RoleListItem[]> = this.auth.hasPermission('roles.view')
      ? this.api.getRoles()
      : of<readonly RoleListItem[]>([]);
    forkJoin({ roles: roles$, user: user$ })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: ({ roles, user }) => {
          this.roles.set(roles);
          if (user)
            this.form.patchValue({
              fullName: user.fullName,
              email: user.email,
              phone: user.phone ?? '',
              roleIDs: [...user.roleIDs],
            });
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}
