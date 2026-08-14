import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize, forkJoin, of, type Observable } from 'rxjs';

import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import type { PriceListListItem, SKUListItem } from '../../../products/models/products.model';
import { ProductsApiService } from '../../../products/services/products-api.service';
import type {
  ClientListItem,
  OpportunityListItem,
  QuotationDetails,
  SaveQuotationItemRequest,
} from '../../models/crm.model';
import { CrmApiService } from '../../services/crm-api.service';

interface EditableQuotationItem extends SaveQuotationItemRequest {
  skuCode: string;
  skuName: string;
}

@Component({
  selector: 'app-quotation-form-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './quotation-form-page.component.html',
  styleUrl: './quotation-form-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class QuotationFormPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(CrmApiService);
  private readonly productsApi = inject(ProductsApiService);
  private readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);

  protected readonly quotationID = Number(this.route.snapshot.paramMap.get('quotationID')) || null;
  protected readonly isEdit = computed(() => this.quotationID !== null);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly clients = signal<readonly ClientListItem[]>([]);
  protected readonly opportunities = signal<readonly OpportunityListItem[]>([]);
  protected readonly priceLists = signal<readonly PriceListListItem[]>([]);
  protected readonly skus = signal<readonly SKUListItem[]>([]);
  protected readonly items = signal<readonly EditableQuotationItem[]>([]);

  protected readonly form = this.fb.nonNullable.group({
    quotationNo: ['', Validators.required],
    opportunityID: this.fb.control<number | null>(null),
    clientID: [0, Validators.min(1)],
    campaignID: this.fb.control<number | null>(null),
    priceListID: this.fb.control<number | null>(null),
    validFrom: ['', Validators.required],
    validUntil: ['', Validators.required],
    terms: [''],
  });
  protected readonly itemForm = this.fb.nonNullable.group({
    skuID: [0, Validators.min(1)],
    quantity: [1, Validators.min(0.01)],
    unitPrice: [0, Validators.min(0)],
    discountPercent: [0, [Validators.min(0), Validators.max(100)]],
    taxAmount: [0, Validators.min(0)],
    note: [''],
  });

  constructor() {
    this.load();
  }

  protected addItem(): void {
    if (this.itemForm.invalid) {
      this.itemForm.markAllAsTouched();
      return;
    }
    const value = this.itemForm.getRawValue();
    const sku = this.skus().find((item) => item.skuID === value.skuID);
    if (!sku) return;
    const item: EditableQuotationItem = {
      ...value,
      note: value.note || null,
      skuCode: sku.skuCode,
      skuName: sku.skuName,
    };
    this.items.update((items) => [
      ...items.filter((current) => current.skuID !== item.skuID),
      item,
    ]);
    this.itemForm.reset({
      skuID: 0,
      quantity: 1,
      unitPrice: 0,
      discountPercent: 0,
      taxAmount: 0,
      note: '',
    });
  }

  protected useTradePrice(): void {
    const sku = this.skus().find((item) => item.skuID === this.itemForm.controls.skuID.value);
    if (sku) this.itemForm.controls.unitPrice.setValue(sku.standardTradePrice);
  }

  protected removeItem(skuID: number): void {
    this.items.update((items) => items.filter((item) => item.skuID !== skuID));
  }

  protected save(): void {
    if (this.form.invalid || this.items().length === 0) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const value = this.form.getRawValue();
    const request = {
      ...value,
      terms: value.terms || null,
      items: this.items().map(({ skuCode, skuName, ...item }) => item),
    };
    const operation: Observable<number | boolean> = this.quotationID
      ? this.api.updateQuotation(this.quotationID, request)
      : this.api.createQuotation(request);
    operation
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: (result) =>
          void this.router.navigate(['/crm/quotations', this.quotationID ?? Number(result)]),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected lineTotal(item: EditableQuotationItem): number {
    const gross = item.quantity * item.unitPrice;
    return gross - (gross * item.discountPercent) / 100 + item.taxAmount;
  }

  protected netTotal(): number {
    return this.items().reduce((total, item) => total + this.lineTotal(item), 0);
  }

  private load(): void {
    this.loading.set(true);
    const details$ = this.quotationID
      ? this.api.getQuotation(this.quotationID)
      : of<QuotationDetails | null>(null);
    forkJoin({
      details: details$,
      clients: this.api.getClients({
        pageNumber: 1,
        pageSize: 300,
        sortBy: 'ClientName',
        sortDirection: 'asc',
      }),
      opportunities: this.api.getOpportunities({
        pageNumber: 1,
        pageSize: 300,
        sortBy: 'OpportunityName',
        sortDirection: 'asc',
      }),
      priceLists: this.productsApi.getPriceLists({
        pageNumber: 1,
        pageSize: 300,
        sortBy: 'EffectiveFrom',
        sortDirection: 'desc',
      }),
      skus: this.productsApi.getSKUs({
        pageNumber: 1,
        pageSize: 500,
        sortBy: 'SKUName',
        sortDirection: 'asc',
      }),
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: ({ details, clients, opportunities, priceLists, skus }) => {
          this.clients.set(clients.items);
          this.opportunities.set(opportunities.items);
          this.priceLists.set(priceLists.items);
          this.skus.set(skus.items.filter((item) => item.isActive));
          if (!details) return;
          this.form.setValue({
            quotationNo: details.quotationNo,
            opportunityID: details.opportunityID,
            clientID: details.clientID,
            campaignID: details.campaignID,
            priceListID: details.priceListID,
            validFrom: details.validFrom,
            validUntil: details.validUntil,
            terms: details.terms ?? '',
          });
          this.items.set(
            details.items.map((item) => {
              const sku = skus.items.find((option) => option.skuID === item.skuID);
              return {
                skuID: item.skuID,
                skuCode: sku?.skuCode ?? `SKU-${item.skuID}`,
                skuName: sku?.skuName ?? `SKU ${item.skuID}`,
                quantity: item.quantity,
                unitPrice: item.unitPrice,
                discountPercent: item.discountPercent,
                taxAmount: item.taxAmount,
                note: item.note,
              };
            }),
          );
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}

