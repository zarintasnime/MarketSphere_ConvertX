import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import {
  BrowserLocationService,
  type BrowserLocation,
} from '../../../../core/services/browser-location.service';
import { VISIT_TYPE_OPTIONS } from '../../../marketing/models/marketing.model';
import { FieldOperationsApiService } from '../../services/field-operations-api.service';
@Component({
  selector: 'app-field-visit-check-in-page',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './field-visit-check-in-page.component.html',
  styleUrl: './field-visit-check-in-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FieldVisitCheckInPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(FieldOperationsApiService);
  private readonly auth = inject(AuthService);
  private readonly location = inject(BrowserLocationService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  protected readonly visitTypes = VISIT_TYPE_OPTIONS;
  protected readonly locationData = signal<BrowserLocation | null>(null);
  protected readonly locating = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly form = this.fb.nonNullable.group({
    clientID: [
      Number(this.route.snapshot.queryParamMap.get('clientID') ?? 0),
      [Validators.required, Validators.min(1)],
    ],
    routeID: [this.numberOrNull(this.route.snapshot.queryParamMap.get('routeID'))],
    campaignID: [null as number | null],
    visitType: [0],
    note: [''],
  });
  protected captureLocation(): void {
    this.locating.set(true);
    this.errorMessage.set('');
    this.location
      .getCurrentLocation()
      .then((value) => this.locationData.set(value))
      .catch((error: Error) => this.errorMessage.set(error.message))
      .finally(() => this.locating.set(false));
  }
  protected submit(): void {
    const employeeID = this.auth.currentUser()?.employeeID;
    const location = this.locationData();
    if (!employeeID) {
      this.errorMessage.set('The current account is not linked to an employee.');
      return;
    }
    if (!location) {
      this.errorMessage.set('Capture the current location before check-in.');
      return;
    }
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    this.saving.set(true);
    this.api
      .checkIn({
        employeeID,
        clientID: value.clientID,
        routeID: value.routeID,
        campaignID: value.campaignID,
        visitType: value.visitType,
        checkInAt: new Date().toISOString(),
        checkInGPSLat: location.latitude,
        checkInGPSLng: location.longitude,
        accuracyMeters: location.accuracyMeters,
        note: value.note || null,
      })
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => void this.router.navigate(['/field/active-visit']),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  private numberOrNull(value: string | null): number | null {
    const parsed = Number(value);
    return Number.isFinite(parsed) && parsed > 0 ? parsed : null;
  }
}
