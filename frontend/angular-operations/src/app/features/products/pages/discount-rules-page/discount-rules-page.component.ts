import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, type Observable } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { createEmptyPagedResult } from '../../../../core/models/paged-result.model';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { PaginationComponent } from '../../../../shared/components/pagination.component';
import { StatusBadgeComponent } from '../../../../shared/components/status-badge.component';
import {
  SALES_CHANNEL_OPTIONS,
  PriceResolution,
  SalesChannel,
  StandardDiscountRule,
  optionLabel,
} from '../../models/products.model';
import { ProductsApiService } from '../../services/products-api.service';

@Component({
  selector: 'app-discount-rules-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LoadingPanelComponent,
    PageHeaderComponent,
    PaginationComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './discount-rules-page.component.html',
  styleUrl: './discount-rules-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DiscountRulesPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(ProductsApiService);
  private readonly auth = inject(AuthService);

  protected readonly result = signal(createEmptyPagedResult<StandardDiscountRule>(1, 10));
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly search = signal('');
  protected readonly editingID = signal<number | null>(null);
  protected readonly editorOpen = signal(false);
  protected readonly resolvedPrice = signal<PriceResolution | null>(null);
  protected readonly canManage = computed(() =>
    this.auth.hasPermission('pricing.discount_rules.manage'),
  );
  protected readonly channelOptions = SALES_CHANNEL_OPTIONS;

  protected readonly form = this.fb.nonNullable.group({
    ruleName: ['', [Validators.required, Validators.maxLength(150)]],
    channel: [SalesChannel.GeneralTrade, Validators.required],
    clientSegmentID: this.fb.control<number | null>(null),
    skuID: this.fb.control<number | null>(null),
    productCategoryID: this.fb.control<number | null>(null),
    minQuantity: this.fb.control<number | null>(null),
    maxDiscountPercent: [0, [Validators.min(0), Validators.max(100)]],
    requiresApproval: [false],
    effectiveFrom: ['', Validators.required],
    effectiveTo: [''],
    isActive: [true],
  });

  protected readonly resolverForm = this.fb.nonNullable.group({
    skuID: [0, Validators.min(1)],
    channel: [SalesChannel.GeneralTrade, Validators.required],
    clientSegmentID: this.fb.control<number | null>(null),
    quantity: [1, Validators.min(0.01)],
    priceDate: [new Date().toISOString().slice(0, 10), Validators.required],
  });

  constructor() {
    this.load();
  }

  protected load(pageNumber = this.result().pageNumber): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.api
      .getDiscountRules({
        pageNumber,
        pageSize: this.result().pageSize,
        search: this.search(),
        sortBy: 'EffectiveFrom',
        sortDirection: 'desc',
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected openCreate(): void {
    this.reset();
    this.editorOpen.set(true);
  }

  protected edit(item: StandardDiscountRule): void {
    if (!this.canManage()) return;
    this.editingID.set(item.standardDiscountRuleID);
    this.editorOpen.set(true);
    this.form.setValue({
      ruleName: item.ruleName,
      channel: item.channel,
      clientSegmentID: item.clientSegmentID,
      skuID: item.skuID,
      productCategoryID: item.productCategoryID,
      minQuantity: item.minQuantity,
      maxDiscountPercent: item.maxDiscountPercent,
      requiresApproval: item.requiresApproval,
      effectiveFrom: item.effectiveFrom,
      effectiveTo: item.effectiveTo ?? '',
      isActive: item.isActive,
    });
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const ruleID = this.editingID();
    const value = this.form.getRawValue();
    const request = { ...value, effectiveTo: value.effectiveTo || null };
    const operation: Observable<number | boolean> = ruleID
      ? this.api.updateDiscountRule(ruleID, request)
      : this.api.createDiscountRule(request);
    operation.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.editorOpen.set(false);
        this.reset();
        this.load();
      },
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected toggleStatus(item: StandardDiscountRule): void {
    this.api.setDiscountRuleStatus(item.standardDiscountRuleID, !item.isActive).subscribe({
      next: () => this.load(),
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected resolvePrice(): void {
    if (this.resolverForm.invalid) {
      this.resolverForm.markAllAsTouched();
      return;
    }
    this.resolvedPrice.set(null);
    this.api.resolvePrice(this.resolverForm.getRawValue()).subscribe({
      next: (result) => this.resolvedPrice.set(result),
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected closeEditor(): void {
    this.editorOpen.set(false);
    this.reset();
  }
  protected channelLabel(value: SalesChannel): string {
    return optionLabel(this.channelOptions, value);
  }

  private reset(): void {
    this.editingID.set(null);
    this.form.reset({
      ruleName: '',
      channel: SalesChannel.GeneralTrade,
      clientSegmentID: null,
      skuID: null,
      productCategoryID: null,
      minQuantity: null,
      maxDiscountPercent: 0,
      requiresApproval: false,
      effectiveFrom: '',
      effectiveTo: '',
      isActive: true,
    });
  }
}

