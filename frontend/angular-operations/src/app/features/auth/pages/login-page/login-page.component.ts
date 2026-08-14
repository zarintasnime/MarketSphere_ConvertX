import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { AutofocusDirective } from '../../../../shared/directives/autofocus.directive';

const FIELD_ROLE_CODES = [
  'SALES_OFFICER',
  'MT_EXECUTIVE',
  'BUSINESS_PROMOTER',
  'MERCHANDISER',
] as const;

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, AutofocusDirective],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginPageComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly isSubmitting = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly showPassword = signal(false);

  protected readonly form = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email, Validators.maxLength(256)]],
    password: ['', [Validators.required, Validators.maxLength(200)]],
  });

  protected get sessionExpired(): boolean {
    return this.route.snapshot.queryParamMap.get('sessionExpired') === 'true';
  }

  protected togglePasswordVisibility(): void {
    this.showPassword.update((value) => !value);
  }

  protected submit(): void {
    this.errorMessage.set('');
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.authService
      .login(this.form.getRawValue())
      .pipe(
        finalize(() => this.isSubmitting.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (session) => {
          const safeReturnUrl = this.getSafeReturnUrl();
          const isFieldUser = FIELD_ROLE_CODES.some((role) =>
            session.user.roleCodes.includes(role),
          );
          const target = session.user.mustChangePassword
            ? '/auth/change-password'
            : safeReturnUrl || (isFieldUser ? '/field/home' : '/dashboard');
          void this.router.navigateByUrl(target);
        },
        error: (error: unknown) => {
          this.errorMessage.set(
            getApiErrorMessage(error, 'Login failed. Check your email and password.'),
          );
        },
      });
  }

  private getSafeReturnUrl(): string | null {
    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
    if (!returnUrl || !returnUrl.startsWith('/') || returnUrl.startsWith('//')) return null;
    if (returnUrl.startsWith('/auth/login')) return null;
    return returnUrl;
  }
}

