import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import {
  BrowserLocationService,
  type BrowserLocation,
} from '../../../../core/services/browser-location.service';
import type { BpSellOutItemRequest } from '../../../marketing/models/marketing.model';
import { FieldOperationsApiService } from '../../services/field-operations-api.service';
@Component({
  selector: 'app-bp-sell-out-capture-page',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './bp-sell-out-capture-page.component.html',
  styleUrl: './bp-sell-out-capture-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BpSellOutCapturePageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(FieldOperationsApiService);
  private readonly auth = inject(AuthService);
  private readonly location = inject(BrowserLocationService);
  protected readonly items = signal<readonly BpSellOutItemRequest[]>([]);
  protected readonly locationData = signal<BrowserLocation | null>(null);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly successMessage = signal('');
  protected readonly headerForm = this.fb.nonNullable.group({
    clientID: [0, [Validators.required, Validators.min(1)]],
    visitID: [null as number | null],
    campaignID: [null as number | null],
    sellOutDate: [new Date().toISOString().slice(0, 10), Validators.required],
  });
  protected readonly itemForm = this.fb.nonNullable.group({
    skuID: [0, [Validators.required, Validators.min(1)]],
    quantitySold: [1, [Validators.required, Validators.min(0.01)]],
    unitSellingPrice: [null as number | null],
  });
  protected addItem(): void {
    if (this.itemForm.invalid) return;
    this.items.update((items) => [...items, this.itemForm.getRawValue()]);
    this.itemForm.reset({ skuID: 0, quantitySold: 1, unitSellingPrice: null });
  }
  protected removeItem(index: number): void {
    this.items.update((items) => items.filter((_, itemIndex) => itemIndex !== index));
  }
  protected captureLocation(): void {
    this.location
      .getCurrentLocation()
      .then((value) => this.locationData.set(value))
      .catch((error: Error) => this.errorMessage.set(error.message));
  }
  protected submit(): void {
    const employeeID = this.auth.currentUser()?.employeeID;
    if (!employeeID) {
      this.errorMessage.set('The current account is not linked to an employee.');
      return;
    }
    if (this.headerForm.invalid || this.items().length === 0) {
      this.errorMessage.set('Complete the header and add at least one SKU.');
      return;
    }
    const header = this.headerForm.getRawValue();
    const gps = this.locationData();
    this.saving.set(true);
    this.api
      .createBpSellOut({
        employeeID,
        clientID: header.clientID,
        visitID: header.visitID,
        campaignID: header.campaignID,
        sellOutDate: header.sellOutDate,
        gpsLat: gps?.latitude ?? null,
        gpsLng: gps?.longitude ?? null,
        items: this.items(),
      })
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => {
          this.successMessage.set('The sell-out record was submitted.');
          this.items.set([]);
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}
