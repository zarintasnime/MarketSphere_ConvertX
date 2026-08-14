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
import {
  StatusBadgeComponent,
  StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import {
  TASK_PRIORITY_OPTIONS,
  TASK_STATUS_OPTIONS,
  CrmTask,
  CrmTaskStatus,
  TaskPriority,
  optionLabel,
} from '../../models/crm.model';
import { CrmApiService } from '../../services/crm-api.service';

@Component({
  selector: 'app-tasks-page',
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
  templateUrl: './tasks-page.component.html',
  styleUrl: './tasks-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TasksPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(CrmApiService);
  private readonly auth = inject(AuthService);

  protected readonly result = signal(createEmptyPagedResult<CrmTask>(1, 10));
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly search = signal('');
  protected readonly assignedEmployeeID = signal<number | null>(null);
  protected readonly overdueOnly = signal(false);
  protected readonly editorOpen = signal(false);
  protected readonly editingID = signal<number | null>(null);
  protected readonly canManage = computed(() => this.auth.hasPermission('crm.tasks.manage'));
  protected readonly priorityOptions = TASK_PRIORITY_OPTIONS;
  protected readonly statusOptions = TASK_STATUS_OPTIONS;

  protected readonly form = this.fb.nonNullable.group({
    leadID: this.fb.control<number | null>(null),
    clientID: this.fb.control<number | null>(null),
    opportunityID: this.fb.control<number | null>(null),
    complaintID: this.fb.control<number | null>(null),
    reactivationCaseID: this.fb.control<number | null>(null),
    assignedEmployeeID: [0, Validators.min(1)],
    title: ['', Validators.required],
    description: [''],
    priority: [TaskPriority.Normal],
    dueAt: ['', Validators.required],
    reminderAt: [''],
    recurrenceRule: [''],
  });

  constructor() {
    this.load();
  }

  protected load(pageNumber = this.result().pageNumber): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.api
      .getTasks(
        {
          pageNumber,
          pageSize: this.result().pageSize,
          search: this.search(),
          sortBy: 'DueAt',
          sortDirection: 'asc',
        },
        this.assignedEmployeeID(),
        this.overdueOnly(),
      )
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

  protected edit(item: CrmTask): void {
    if (!this.canManage()) return;
    this.editingID.set(item.crmTaskID);
    this.editorOpen.set(true);
    this.form.setValue({
      leadID: item.leadID,
      clientID: item.clientID,
      opportunityID: item.opportunityID,
      complaintID: item.complaintID,
      reactivationCaseID: item.reactivationCaseID,
      assignedEmployeeID: item.assignedEmployeeID,
      title: item.title,
      description: item.description ?? '',
      priority: item.priority,
      dueAt: item.dueAt.slice(0, 16),
      reminderAt: item.reminderAt?.slice(0, 16) ?? '',
      recurrenceRule: item.recurrenceRule ?? '',
    });
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const taskID = this.editingID();
    const value = this.form.getRawValue();
    const request = {
      ...value,
      description: value.description || null,
      reminderAt: value.reminderAt || null,
      recurrenceRule: value.recurrenceRule || null,
    };
    const operation: Observable<number | boolean> = taskID ? this.api.updateTask(taskID, request) : this.api.createTask(request);
    operation.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.editorOpen.set(false);
        this.reset();
        this.load();
      },
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected changeStatus(item: CrmTask, status: CrmTaskStatus): void {
    this.api
      .changeTaskStatus(item.crmTaskID, status)
      .subscribe({
        next: () => this.load(),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected closeEditor(): void {
    this.editorOpen.set(false);
    this.reset();
  }
  protected priorityLabel(value: TaskPriority): string {
    return optionLabel(TASK_PRIORITY_OPTIONS, value);
  }
  protected statusLabel(value: CrmTaskStatus): string {
    return optionLabel(TASK_STATUS_OPTIONS, value);
  }
  protected statusTone(value: CrmTaskStatus): StatusBadgeTone {
    return value === CrmTaskStatus.Completed
      ? 'success'
      : value === CrmTaskStatus.InProgress
        ? 'info'
        : value === CrmTaskStatus.Cancelled
          ? 'neutral'
          : 'warning';
  }

  private reset(): void {
    this.editingID.set(null);
    this.form.reset({
      leadID: null,
      clientID: null,
      opportunityID: null,
      complaintID: null,
      reactivationCaseID: null,
      assignedEmployeeID: 0,
      title: '',
      description: '',
      priority: TaskPriority.Normal,
      dueAt: '',
      reminderAt: '',
      recurrenceRule: '',
    });
  }
}

