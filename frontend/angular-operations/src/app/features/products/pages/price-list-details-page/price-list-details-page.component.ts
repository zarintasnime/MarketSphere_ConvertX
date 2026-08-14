import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize, forkJoin, of, type Observable } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import {
  SALES_CHANNEL_OPTIONS,
  PriceListDetails,
  SKUListItem,
  SalesChannel,
  SavePriceListItemRequest,
} from '../../models/products.model';
import { ProductsApiService } from '../../services/products-api.service';

interface EditablePriceListItem extends SavePriceListItemRequest {
  skuCode: string;
  skuName: string;
}

@Component({
  selector: 'app-price-list-details-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './price-list-details-page.component.html',
  styleUrl: './price-list-details-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PriceListDetailsPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(ProductsApiService);
  private readonly auth = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);

  protected readonly priceListID = Number(this.route.snapshot.paramMap.get('priceListID')) || null;
  protected readonly isEdit = computed(() => this.priceListID !== null);
  protected readonly canManage = computed(() =>
    this.auth.hasPermission('pricing.price_lists.manage'),
  );
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly skuOptions = signal<readonly SKUListItem[]>([]);
  protected readonly items = signal<readonly EditablePriceListItem[]>([]);
  protected readonly channelOptions = SALES_CHANNEL_OPTIONS;

  protected readonly form = this.fb.nonNullable.group({
    priceListCode: ['', [Validators.required, Validators.maxLength(50)]],
    priceListName: ['', [Validators.required, Validators.maxLength(150)]],
    channel: [SalesChannel.GeneralTrade, Validators.required],
    clientSegmentID: this.fb.control<number | null>(null),
    effectiveFrom: ['', Validators.required],
    effectiveTo: [''],
    currencyCode: ['BDT', [Validators.required, Validators.maxLength(3)]],
  });

  protected readonly itemForm = this.fb.nonNullable.group({
    skuID: [0, Validators.min(1)],
    unitPrice: [0, Validators.min(0.01)],
    maximumDiscountPercent: [0, [Validators.min(0), Validators.max(100)]],
    minimumOrderQuantity: this.fb.control<number | null>(null),
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
    const sku = this.skuOptions().find((item) => item.skuID === value.skuID);
    if (!sku) return;

    const next: EditablePriceListItem = {
      skuID: value.skuID,
      skuCode: sku.skuCode,
      skuName: sku.skuName,
      unitPrice: value.unitPrice,
      maximumDiscountPercent: value.maximumDiscountPercent,
      minimumOrderQuantity: value.minimumOrderQuantity,
    };

    this.items.update((items) => [...items.filter((item) => item.skuID !== next.skuID), next]);
    this.itemForm.reset({
      skuID: 0,
      unitPrice: 0,
      maximumDiscountPercent: 0,
      minimumOrderQuantity: null,
    });
  }

  protected removeItem(skuID: number): void {
    this.items.update((items) => items.filter((item) => item.skuID !== skuID));
  }

  protected save(): void {
    if (!this.canManage() || this.form.invalid || this.items().length === 0) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.errorMessage.set('');
    const value = this.form.getRawValue();
    const request = {
      ...value,
      effectiveTo: value.effectiveTo || null,
      currencyCode: value.currencyCode.toUpperCase(),
      items: this.items().map(({ skuCode, skuName, ...item }) => item),
    };
    const operation: Observable<number | boolean> = this.priceListID
      ? this.api.updatePriceList(this.priceListID, request)
      : this.api.createPriceList(request);

    operation.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => void this.router.navigate(['/products/price-lists']),
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  private load(): void {
    this.loading.set(true);
    const details$ = this.priceListID
      ? this.api.getPriceList(this.priceListID)
      : of<PriceListDetails | null>(null);
    const skus$ = this.api.getSKUs({
      pageNumber: 1,
      pageSize: 500,
      sortBy: 'SKUName',
      sortDirection: 'asc',
    });
    forkJoin({ details: details$, skus: skus$ })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: ({ details, skus }) => {
          this.skuOptions.set(skus.items.filter((item) => item.isActive));
          if (!details) return;
          this.form.setValue({
            priceListCode: details.priceListCode,
            priceListName: details.priceListName,
            channel: details.channel,
            clientSegmentID: details.clientSegmentID,
            effectiveFrom: details.effectiveFrom,
            effectiveTo: details.effectiveTo ?? '',
            currencyCode: details.currencyCode,
          });
          this.items.set(
            details.items.map((item) => ({
              skuID: item.skuID,
              skuCode: item.skuCode,
              skuName: item.skuName,
              unitPrice: item.unitPrice,
              maximumDiscountPercent: item.maximumDiscountPercent,
              minimumOrderQuantity: item.minimumOrderQuantity,
            })),
          );
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}

