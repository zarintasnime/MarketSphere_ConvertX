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
  ACTIVITY_STATUS_OPTIONS,
  ACTIVITY_TYPE_OPTIONS,
  OPPORTUNITY_STAGE_OPTIONS,
  CrmActivity,
  CrmActivityStatus,
  CrmActivityType,
  OpportunityDetails,
  OpportunityStage,
  optionLabel,
} from '../../models/crm.model';
import { CrmApiService } from '../../services/crm-api.service';

@Component({
  selector: 'app-opportunity-details-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './opportunity-details-page.component.html',
  styleUrl: './opportunity-details-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OpportunityDetailsPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(CrmApiService);
  private readonly auth = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);

  protected readonly opportunityID =
    Number(this.route.snapshot.paramMap.get('opportunityID')) || null;
  protected readonly isNew = computed(() => this.opportunityID === null);
  protected readonly canManage = computed(() =>
    this.auth.hasPermission('crm.opportunities.manage'),
  );
  protected readonly canManageActivities = computed(() =>
    this.auth.hasPermission('crm.activities.manage'),
  );
  protected readonly details = signal<OpportunityDetails | null>(null);
  protected readonly activities = signal<readonly CrmActivity[]>([]);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly stageOptions = OPPORTUNITY_STAGE_OPTIONS;
  protected readonly activityTypeOptions = ACTIVITY_TYPE_OPTIONS;
  protected readonly activityStatusOptions = ACTIVITY_STATUS_OPTIONS;

  protected readonly form = this.fb.nonNullable.group({
    opportunityCode: ['', Validators.required],
    leadID: this.fb.control<number | null>(null),
    clientID: this.fb.control<number | null>(null),
    campaignID: this.fb.control<number | null>(null),
    ownerEmployeeID: [0, Validators.min(1)],
    opportunityName: ['', Validators.required],
    expectedValue: [0, Validators.min(0)],
    probabilityPercent: [0, [Validators.min(0), Validators.max(100)]],
    expectedCloseDate: [''],
    competitor: [''],
  });
  protected readonly stageForm = this.fb.nonNullable.group({
    stage: [OpportunityStage.Qualified],
    lostReason: [''],
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

  protected save(): void {
    if (!this.canManage() || this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const value = this.form.getRawValue();
    const request = {
      ...value,
      expectedCloseDate: value.expectedCloseDate || null,
      competitor: value.competitor || null,
    };
    const operation: Observable<number | boolean> = this.opportunityID
      ? this.api.updateOpportunity(this.opportunityID, request)
      : this.api.createOpportunity(request);
    operation.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: (result) => {
        const id = this.opportunityID ?? Number(result);
        if (this.isNew()) void this.router.navigate(['/crm/opportunities', id]);
        else this.load();
      },
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected changeStage(): void {
    if (!this.opportunityID) return;
    const value = this.stageForm.getRawValue();
    this.api
      .changeOpportunityStage(this.opportunityID, value.stage, value.lostReason || null)
      .subscribe({
        next: () => this.load(),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected addActivity(): void {
    if (!this.opportunityID || this.activityForm.invalid) return;
    const value = this.activityForm.getRawValue();
    this.api
      .createActivity({
        leadID: null,
        clientID: null,
        opportunityID: this.opportunityID,
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

  protected stageLabel(value: OpportunityStage): string {
    return optionLabel(OPPORTUNITY_STAGE_OPTIONS, value);
  }
  protected activityTypeLabel(value: number): string {
    return optionLabel(ACTIVITY_TYPE_OPTIONS, value);
  }

  private load(): void {
    if (!this.opportunityID) return;
    this.loading.set(true);
    forkJoin({
      details: this.api.getOpportunity(this.opportunityID),
      activities: this.api.getActivities({ opportunityID: this.opportunityID }),
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: ({ details, activities }) => {
          this.details.set(details);
          this.activities.set(activities);
          this.form.setValue({
            opportunityCode: details.opportunityCode,
            leadID: details.leadID,
            clientID: details.clientID,
            campaignID: details.campaignID,
            ownerEmployeeID: details.ownerEmployeeID,
            opportunityName: details.opportunityName,
            expectedValue: details.expectedValue,
            probabilityPercent: details.probabilityPercent,
            expectedCloseDate: details.expectedCloseDate ?? '',
            competitor: details.competitor ?? '',
          });
          this.stageForm.setValue({ stage: details.stage, lostReason: details.lostReason ?? '' });
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}

