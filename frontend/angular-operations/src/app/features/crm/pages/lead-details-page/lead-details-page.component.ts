import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin, finalize } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import {
  ACTIVITY_STATUS_OPTIONS,
  ACTIVITY_TYPE_OPTIONS,
  CLIENT_TYPE_OPTIONS,
  LEAD_SOURCE_OPTIONS,
  LEAD_STATUS_OPTIONS,
  LEAD_TEMPERATURE_OPTIONS,
  SALES_CHANNEL_OPTIONS,
  CrmActivity,
  CrmActivityStatus,
  CrmActivityType,
  DuplicateCandidate,
  DuplicateResolutionType,
  DuplicateReview,
  LeadDetails,
  LeadScoreResult,
  LeadStatus,
  SalesChannel,
  ClientType,
  optionLabel,
} from '../../models/crm.model';
import { CrmApiService } from '../../services/crm-api.service';

@Component({
  selector: 'app-lead-details-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './lead-details-page.component.html',
  styleUrl: './lead-details-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LeadDetailsPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(CrmApiService);
  private readonly auth = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);

  protected readonly leadID = Number(this.route.snapshot.paramMap.get('leadID'));
  protected readonly lead = signal<LeadDetails | null>(null);
  protected readonly score = signal<LeadScoreResult | null>(null);
  protected readonly duplicates = signal<readonly DuplicateCandidate[]>([]);
  protected readonly reviews = signal<readonly DuplicateReview[]>([]);
  protected readonly activities = signal<readonly CrmActivity[]>([]);
  protected readonly loading = signal(false);
  protected readonly busy = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly canManage = computed(() => this.auth.hasPermission('crm.leads.manage'));
  protected readonly canManageDuplicates = computed(() =>
    this.auth.hasPermission('crm.duplicate_reviews.manage'),
  );
  protected readonly canManageActivities = computed(() =>
    this.auth.hasPermission('crm.activities.manage'),
  );
  protected readonly statusOptions = LEAD_STATUS_OPTIONS;
  protected readonly activityTypeOptions = ACTIVITY_TYPE_OPTIONS;
  protected readonly activityStatusOptions = ACTIVITY_STATUS_OPTIONS;
  protected readonly clientTypeOptions = CLIENT_TYPE_OPTIONS;
  protected readonly channelOptions = SALES_CHANNEL_OPTIONS;

  protected readonly statusForm = this.fb.nonNullable.group({
    status: [LeadStatus.Contacted],
    lostReason: [''],
  });
  protected readonly conversionForm = this.fb.nonNullable.group({
    clientCode: ['', Validators.required],
    clientType: [ClientType.Outlet],
    channel: [SalesChannel.GeneralTrade],
    address: ['', Validators.required],
  });
  protected readonly activityForm = this.fb.nonNullable.group({
    activityType: [CrmActivityType.Call],
    subject: ['', Validators.required],
    details: [''],
    activityAt: [new Date().toISOString().slice(0, 16)],
    activityStatus: [CrmActivityStatus.Completed],
    outcome: [''],
    nextActionAt: [''],
    performedByEmployeeID: this.fb.control<number | null>(null),
  });

  constructor() {
    this.load();
  }

  protected changeStatus(): void {
    const value = this.statusForm.getRawValue();
    this.api.changeLeadStatus(this.leadID, value.status, value.lostReason || null).subscribe({
      next: () => this.load(),
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected recalculateScore(): void {
    this.busy.set(true);
    this.api
      .recalculateLeadScore(this.leadID)
      .pipe(finalize(() => this.busy.set(false)))
      .subscribe({
        next: (result) => {
          this.score.set(result);
          this.loadLeadOnly();
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected convertToClient(): void {
    if (this.conversionForm.invalid) return;
    this.api.convertLeadToClient(this.leadID, this.conversionForm.getRawValue()).subscribe({
      next: (clientID) => void this.router.navigate(['/crm/clients', clientID]),
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected resolveReview(review: DuplicateReview, resolutionType: DuplicateResolutionType): void {
    this.api.resolveDuplicateReview(review.duplicateReviewCaseID, resolutionType, null).subscribe({
      next: () => this.load(),
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected addActivity(): void {
    if (this.activityForm.invalid) return;
    const value = this.activityForm.getRawValue();
    this.api
      .createActivity({
        leadID: this.leadID,
        clientID: null,
        opportunityID: null,
        activityType: value.activityType,
        subject: value.subject,
        details: value.details || null,
        activityAt: value.activityAt,
        scheduledStartAt: null,
        scheduledEndAt: null,
        locationOrMeetingLink: null,
        agenda: null,
        activityStatus: value.activityStatus,
        outcome: value.outcome || null,
        nextActionAt: value.nextActionAt || null,
        performedByEmployeeID: value.performedByEmployeeID,
        participants: [],
      })
      .subscribe({
        next: () => {
          this.activityForm.reset({
            activityType: CrmActivityType.Call,
            subject: '',
            details: '',
            activityAt: new Date().toISOString().slice(0, 16),
            activityStatus: CrmActivityStatus.Completed,
            outcome: '',
            nextActionAt: '',
            performedByEmployeeID: null,
          });
          this.load();
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected statusLabel(value: LeadStatus): string {
    return optionLabel(LEAD_STATUS_OPTIONS, value);
  }
  protected sourceLabel(value: number): string {
    return optionLabel(LEAD_SOURCE_OPTIONS, value);
  }
  protected temperatureLabel(value: number): string {
    return optionLabel(LEAD_TEMPERATURE_OPTIONS, value);
  }
  protected activityTypeLabel(value: number): string {
    return optionLabel(ACTIVITY_TYPE_OPTIONS, value);
  }

  private load(): void {
    this.loading.set(true);
    forkJoin({
      lead: this.api.getLead(this.leadID),
      duplicates: this.api.findLeadDuplicates(this.leadID),
      reviews: this.api.getDuplicateReviews(),
      activities: this.api.getActivities({ leadID: this.leadID }),
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: ({ lead, duplicates, reviews, activities }) => {
          this.lead.set(lead);
          this.duplicates.set(duplicates);
          this.reviews.set(reviews.filter((item) => item.sourceEntityID === this.leadID));
          this.activities.set(activities);
          this.statusForm.setValue({ status: lead.status, lostReason: lead.lostReason ?? '' });
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  private loadLeadOnly(): void {
    this.api.getLead(this.leadID).subscribe((lead) => this.lead.set(lead));
  }
}
