import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable, finalize } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import {
  SettingDataType,
  SettingScopeType,
  SystemSetting,
} from '../../models/administration.model';
import { AdministrationApiService } from '../../services/administration-api.service';

@Component({
  selector: 'app-settings-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './settings-page.component.html',
  styleUrl: './settings-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SettingsPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(AdministrationApiService);
  private readonly auth = inject(AuthService);

  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly successMessage = signal('');
  protected readonly settings = signal<readonly SystemSetting[]>([]);
  protected readonly selectedID = signal<number | null>(null);
  protected readonly canManage = computed(() =>
    this.auth.hasPermission('infrastructure.settings.manage'),
  );
  protected readonly dataTypes = [1, 2, 3, 4, 5, 6] as const;
  protected readonly scopeTypes = [1, 2, 3, 4] as const;

  protected readonly form = this.fb.nonNullable.group({
    settingKey: ['', [Validators.required, Validators.maxLength(150)]],
    settingValue: ['', Validators.required],
    dataType: [SettingDataType.String, Validators.required],
    scopeType: [SettingScopeType.Global, Validators.required],
    scopeID: this.fb.control<number | null>(null),
    description: [''],
    isEncrypted: [false],
  });

  constructor() {
    this.load();
  }

  protected select(setting: SystemSetting): void {
    this.selectedID.set(setting.systemSettingID);
    this.successMessage.set('');
    this.form.patchValue({
      settingKey: setting.settingKey,
      settingValue: setting.settingValue,
      dataType: setting.dataType,
      scopeType: setting.scopeType,
      scopeID: setting.scopeID,
      description: setting.description ?? '',
      isEncrypted: setting.isEncrypted,
    });
  }

  protected resetForm(): void {
    this.selectedID.set(null);
    this.form.reset({
      settingKey: '',
      settingValue: '',
      dataType: SettingDataType.String,
      scopeType: SettingScopeType.Global,
      scopeID: null,
      description: '',
      isEncrypted: false,
    });
  }

  protected save(): void {
    if (this.form.invalid || !this.canManage()) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    const request = { ...value, description: value.description || null };
    const settingID = this.selectedID();
    let request$: Observable<unknown> = settingID
      ? this.api.updateSetting(settingID, request)
      : this.api.createSetting(request);

    this.saving.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');
    request$.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.successMessage.set('System setting saved successfully.');
        this.resetForm();
        this.load(false);
      },
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected dataTypeName(value: SettingDataType): string {
    return SettingDataType[value];
  }
  protected scopeTypeName(value: SettingScopeType): string {
    return SettingScopeType[value];
  }

  protected load(showSpinner = true): void {
    if (showSpinner) this.loading.set(true);
    this.api
      .getSettings()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (items) => this.settings.set(items),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}
