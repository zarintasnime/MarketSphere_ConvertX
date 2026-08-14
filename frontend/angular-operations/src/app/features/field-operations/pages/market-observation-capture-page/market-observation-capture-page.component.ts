import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { OBSERVATION_TYPE_OPTIONS } from '../../../marketing/models/marketing.model';
import { FieldOperationsApiService } from '../../services/field-operations-api.service';
@Component({
  selector: 'app-market-observation-capture-page',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './market-observation-capture-page.component.html',
  styleUrl: './market-observation-capture-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MarketObservationCapturePageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(FieldOperationsApiService);
  private readonly auth = inject(AuthService);
  protected readonly options = OBSERVATION_TYPE_OPTIONS;
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly successMessage = signal('');
  protected readonly form = this.fb.nonNullable.group({
    visitID: [0, [Validators.required, Validators.min(1)]],
    clientID: [0, [Validators.required, Validators.min(1)]],
    observationType: [0],
    skuID: [null as number | null],
    availabilityStatus: [null as number | null],
    facingCount: [null as number | null],
    planogramScore: [null as number | null],
    displayScore: [null as number | null],
    competitorBrand: [''],
    competitorProduct: [''],
    competitorPrice: [null as number | null],
    competitorOffer: [''],
    note: [''],
  });
  protected submit(): void {
    const employeeID = this.auth.currentUser()?.employeeID;
    if (!employeeID) {
      this.errorMessage.set('The current account is not linked to an employee.');
      return;
    }
    if (this.form.invalid) return;
    const value = this.form.getRawValue();
    this.saving.set(true);
    this.api
      .createMarketObservation({
        ...value,
        employeeID,
        competitorBrand: value.competitorBrand || null,
        competitorProduct: value.competitorProduct || null,
        competitorOffer: value.competitorOffer || null,
        note: value.note || null,
      })
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => {
          this.successMessage.set('The market observation was submitted.');
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}
