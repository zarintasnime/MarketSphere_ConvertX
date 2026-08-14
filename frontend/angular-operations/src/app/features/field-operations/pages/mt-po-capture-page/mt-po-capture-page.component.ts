import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, switchMap } from 'rxjs';
import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import type { MtPoItemDraft } from '../../models/field-operations.model';
import { FieldOperationsApiService } from '../../services/field-operations-api.service';
@Component({
  selector: 'app-mt-po-capture-page',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './mt-po-capture-page.component.html',
  styleUrl: './mt-po-capture-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MtPoCapturePageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(FieldOperationsApiService);
  private readonly auth = inject(AuthService);
  protected readonly items = signal<readonly MtPoItemDraft[]>([]);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly successMessage = signal('');
  protected readonly headerForm = this.fb.nonNullable.group({
    clientID: [0, [Validators.required, Validators.min(1)]],
    poNumber: ['', Validators.required],
    poDate: [new Date().toISOString().slice(0, 10), Validators.required],
    receivedDate: [new Date().toISOString().slice(0, 10), Validators.required],
    requestedDeliveryDate: [null as string | null],
  });
  protected readonly itemForm = this.fb.nonNullable.group({
    externalItemCode: [''],
    externalItemName: ['', Validators.required],
    skuID: [null as number | null],
    orderedQuantity: [1, [Validators.required, Validators.min(0.01)]],
    agreedUnitPrice: [null as number | null],
    discount: [null as number | null],
    note: [''],
  });
  protected addItem(): void {
    if (this.itemForm.invalid) {
      this.itemForm.markAllAsTouched();
      return;
    }
    this.items.update((items) => [...items, this.itemForm.getRawValue()]);
    this.itemForm.reset({
      externalItemCode: '',
      externalItemName: '',
      skuID: null,
      orderedQuantity: 1,
      agreedUnitPrice: null,
      discount: null,
      note: '',
    });
  }
  protected removeItem(index: number): void {
    this.items.update((items) => items.filter((_, itemIndex) => itemIndex !== index));
  }
  protected submit(): void {
    const employeeID = this.auth.currentUser()?.employeeID;
    if (!employeeID) {
      this.errorMessage.set('The current account is not linked to an employee.');
      return;
    }
    if (this.headerForm.invalid || this.items().length === 0) {
      this.errorMessage.set('Complete the PO header and add at least one item.');
      return;
    }
    const header = this.headerForm.getRawValue();
    this.saving.set(true);
    this.successMessage.set('');
    this.api
      .createModernTradePurchaseOrder({
        clientID: header.clientID,
        poNumber: header.poNumber.trim(),
        poDate: new Date(header.poDate).toISOString(),
        receivedDate: new Date(header.receivedDate).toISOString(),
        uploadedByEmployeeID: employeeID,
        duplicateHash: null,
        requestedDeliveryDate: header.requestedDeliveryDate
          ? new Date(header.requestedDeliveryDate).toISOString()
          : null,
        items: this.items(),
      })
      .pipe(
        switchMap((id) => this.api.submitModernTradePurchaseOrder(id)),
        finalize(() => this.saving.set(false)),
      )
      .subscribe({
        next: () => {
          this.successMessage.set('The MT purchase order was created and submitted.');
          this.headerForm.reset({
            clientID: 0,
            poNumber: '',
            poDate: new Date().toISOString().slice(0, 10),
            receivedDate: new Date().toISOString().slice(0, 10),
            requestedDeliveryDate: null,
          });
          this.items.set([]);
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}
