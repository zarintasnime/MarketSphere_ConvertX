import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { finalize } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { AutofocusDirective } from '../../../../shared/directives/autofocus.directive';

const strongPasswordValidator: ValidatorFn = (
  control: AbstractControl,
): ValidationErrors | null => {
  const value = control.value || '';
  const isStrong =
    value.length >= 8 &&
    /[A-Z]/.test(value) &&
    /[a-z]/.test(value) &&
    /\d/.test(value) &&
    /[^A-Za-z0-9]/.test(value);

  return isStrong ? null : { weakPassword: true };
};

@Component({
  selector: 'app-change-password-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, AutofocusDirective],
  templateUrl: './change-password-page.component.html',
  styleUrl: './change-password-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ChangePasswordPageComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly currentUser = this.authService.currentUser;
  protected readonly isSubmitting = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly successMessage = signal('');

  protected readonly form = this.formBuilder.nonNullable.group({
    currentPassword: ['', [Validators.required]],
    newPassword: ['', [Validators.required, strongPasswordValidator]],
    confirmPassword: ['', [Validators.required]],
  });

  protected submit(): void {
    this.errorMessage.set('');
    this.successMessage.set('');

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const values = this.form.getRawValue();

    if (values.newPassword !== values.confirmPassword) {
      this.form.controls.confirmPassword.setErrors({ passwordMismatch: true });
      return;
    }

    this.isSubmitting.set(true);

    this.authService
      .changePassword({
        currentPassword: values.currentPassword,
        newPassword: values.newPassword,
      })
      .pipe(
        finalize(() => this.isSubmitting.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.form.reset();
          this.successMessage.set('Password changed successfully.');
        },
        error: (error: unknown) => {
          this.errorMessage.set(getApiErrorMessage(error, 'The password could not be changed.'));
        },
      });
  }

  protected logout(): void {
    this.authService.logout();
    void this.router.navigate(['/auth/login']);
  }
}
