import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { finalize, forkJoin } from 'rxjs';

import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { StatusBadgeComponent } from '../../../../shared/components/status-badge.component';
import {
  CAMPAIGN_STATUS_OPTIONS,
  optionLabel,
  type CampaignDetails,
  type CampaignRoi,
} from '../../models/marketing.model';
import { MarketingApiService } from '../../services/marketing-api.service';

@Component({
  selector: 'app-campaign-details-page',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    LoadingPanelComponent,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './campaign-details-page.component.html',
  styleUrl: './campaign-details-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CampaignDetailsPageComponent {
  private readonly api = inject(MarketingApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);
  protected readonly campaignID = Number(this.route.snapshot.paramMap.get('campaignID'));
  protected readonly item = signal<CampaignDetails | null>(null);
  protected readonly roi = signal<CampaignRoi | null>(null);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly targetForm = this.fb.nonNullable.group({
    targetType: [0],
    regionID: [null as number | null],
    areaID: [null as number | null],
    clientSegmentID: [null as number | null],
    clientID: [null as number | null],
    productCategoryID: [null as number | null],
    skuID: [null as number | null],
    targetValue: [null as number | null],
  });
  protected readonly offerForm = this.fb.nonNullable.group({
    offerCode: ['', Validators.required],
    offerType: [0],
    ruleJson: ['{}'],
    discountValue: [null as number | null],
    freeSKUID: [null as number | null],
    priority: [1],
    usageLimit: [null as number | null],
    perClientLimit: [null as number | null],
    isStackable: [false],
    isActive: [true],
  });
  protected readonly expenseForm = this.fb.nonNullable.group({
    expenseDate: [new Date().toISOString().slice(0, 10)],
    expenseCategory: ['', Validators.required],
    amount: [0, [Validators.required, Validators.min(0.01)]],
    vendorName: [''],
    description: [''],
  });
  protected readonly attributionForm = this.fb.nonNullable.group({
    leadID: [null as number | null],
    opportunityID: [null as number | null],
    quotationID: [null as number | null],
    orderID: [null as number | null],
    attributionType: [0],
    weightPercent: [100, [Validators.min(0), Validators.max(100)]],
    attributedAmount: [null as number | null],
  });
  constructor() {
    this.load();
  }
  protected load(): void {
    this.loading.set(true);
    forkJoin({
      item: this.api.getCampaign(this.campaignID),
      roi: this.api.getCampaignRoi(this.campaignID),
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: ({ item, roi }) => {
          this.item.set(item);
          this.roi.set(roi);
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  protected addTarget(): void {
    this.run(this.api.addCampaignTarget(this.campaignID, this.targetForm.getRawValue()), () =>
      this.targetForm.reset({
        targetType: 0,
        regionID: null,
        areaID: null,
        clientSegmentID: null,
        clientID: null,
        productCategoryID: null,
        skuID: null,
        targetValue: null,
      }),
    );
  }
  protected addOffer(): void {
    if (this.offerForm.invalid) return;
    this.run(this.api.addCampaignOffer(this.campaignID, this.offerForm.getRawValue()), () =>
      this.offerForm.reset({
        offerCode: '',
        offerType: 0,
        ruleJson: '{}',
        discountValue: null,
        freeSKUID: null,
        priority: 1,
        usageLimit: null,
        perClientLimit: null,
        isStackable: false,
        isActive: true,
      }),
    );
  }
  protected addExpense(): void {
    if (this.expenseForm.invalid) return;
    const value = this.expenseForm.getRawValue();
    this.run(
      this.api.addCampaignExpense(this.campaignID, {
        ...value,
        vendorName: value.vendorName || null,
        description: value.description || null,
      }),
      () =>
        this.expenseForm.reset({
          expenseDate: new Date().toISOString().slice(0, 10),
          expenseCategory: '',
          amount: 0,
          vendorName: '',
          description: '',
        }),
    );
  }
  protected addAttribution(): void {
    this.run(
      this.api.addCampaignAttribution(this.campaignID, this.attributionForm.getRawValue()),
      () =>
        this.attributionForm.reset({
          leadID: null,
          opportunityID: null,
          quotationID: null,
          orderID: null,
          attributionType: 0,
          weightPercent: 100,
          attributedAmount: null,
        }),
    );
  }
  protected deleteTarget(id: number): void {
    this.run(this.api.deleteCampaignTarget(id));
  }
  protected deleteOffer(id: number): void {
    this.run(this.api.deleteCampaignOffer(id));
  }
  protected statusLabel(value: number): string {
    return optionLabel(CAMPAIGN_STATUS_OPTIONS, value);
  }
  private run(operation: any, after?: () => void): void {
    this.saving.set(true);
    operation.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        after?.();
        this.load();
      },
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }
}
