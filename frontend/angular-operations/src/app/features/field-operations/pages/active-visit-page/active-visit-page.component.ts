import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import {
  BrowserLocationService,
  type BrowserLocation,
} from '../../../../core/services/browser-location.service';
import type { FieldActiveVisit } from '../../models/field-operations.model';
import { FieldOperationsApiService } from '../../services/field-operations-api.service';
@Component({
  selector: 'app-active-visit-page',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './active-visit-page.component.html',
  styleUrl: './active-visit-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ActiveVisitPageComponent {
  private readonly api = inject(FieldOperationsApiService);
  private readonly location = inject(BrowserLocationService);
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  protected readonly item = signal<FieldActiveVisit | null>(null);
  protected readonly locationData = signal<BrowserLocation | null>(null);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly checkoutForm = this.fb.nonNullable.group({ note: [''] });
  protected readonly cancelForm = this.fb.nonNullable.group({ reason: ['', Validators.required] });
  constructor() {
    this.load();
  }
  protected load(): void {
    this.loading.set(true);
    this.api
      .getActiveVisit()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (item) => this.item.set(item),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  protected captureLocation(): void {
    this.location
      .getCurrentLocation()
      .then((value) => this.locationData.set(value))
      .catch((error: Error) => this.errorMessage.set(error.message));
  }
  protected checkOut(): void {
    const item = this.item();
    const gps = this.locationData();
    if (!item || !gps) {
      this.errorMessage.set('Capture the current location before check-out.');
      return;
    }
    this.saving.set(true);
    this.api
      .checkOut(item.visitID, {
        checkOutAt: new Date().toISOString(),
        checkOutGPSLat: gps.latitude,
        checkOutGPSLng: gps.longitude,
        note: this.checkoutForm.getRawValue().note || null,
      })
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => void this.router.navigate(['/field/home']),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  protected cancelVisit(): void {
    const item = this.item();
    if (!item || this.cancelForm.invalid) return;
    this.saving.set(true);
    this.api
      .cancelVisit(item.visitID, this.cancelForm.getRawValue().reason)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => void this.router.navigate(['/field/home']),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}
