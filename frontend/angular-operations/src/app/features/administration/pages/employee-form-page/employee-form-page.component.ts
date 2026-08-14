import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, finalize, forkJoin, of } from 'rxjs';

import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import type {
  Branch,
  Region,
  Area,
  Territory,
} from '../../../organization/models/organization.model';
import { OrganizationApiService } from '../../../organization/services/organization-api.service';
import {
  EmployeeDetails,
  EmployeeStatus,
  SaveEmployeeRequest,
} from '../../models/administration.model';
import { AdministrationApiService } from '../../services/administration-api.service';

@Component({
  selector: 'app-employee-form-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './employee-form-page.component.html',
  styleUrl: './employee-form-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EmployeeFormPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(AdministrationApiService);
  private readonly organizationApi = inject(OrganizationApiService);
  private readonly route = inject(ActivatedRoute);
  protected readonly router = inject(Router);

  protected readonly employeeID = Number(this.route.snapshot.paramMap.get('employeeID')) || null;
  protected readonly isEdit = computed(() => this.employeeID !== null);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly branches = signal<readonly Branch[]>([]);
  protected readonly regions = signal<readonly Region[]>([]);
  protected readonly areas = signal<readonly Area[]>([]);
  protected readonly territories = signal<readonly Territory[]>([]);
  protected readonly statuses = [
    EmployeeStatus.Active,
    EmployeeStatus.Inactive,
    EmployeeStatus.Suspended,
    EmployeeStatus.Terminated,
  ] as const;

  protected readonly form = this.fb.nonNullable.group({
    employeeCode: ['', [Validators.required, Validators.maxLength(50)]],
    userID: this.fb.control<number | null>(null),
    designationID: [0, [Validators.required, Validators.min(1)]],
    managerEmployeeID: this.fb.control<number | null>(null),
    branchID: [0, [Validators.required, Validators.min(1)]],
    regionID: this.fb.control<number | null>(null),
    areaID: this.fb.control<number | null>(null),
    territoryID: this.fb.control<number | null>(null),
    joiningDate: [new Date().toISOString().slice(0, 10), Validators.required],
    endDate: [''],
    status: [EmployeeStatus.Active, Validators.required],
    phone: [''],
    email: ['', Validators.email],
  });

  constructor() {
    this.load();
  }

  protected loadAreas(regionID: number | null): void {
    this.areas.set([]);
    this.territories.set([]);
    if (!regionID) return;
    this.organizationApi.getAreas(regionID).subscribe((areas) => this.areas.set(areas));
  }

  protected loadTerritories(areaID: number | null): void {
    this.territories.set([]);
    if (!areaID) return;
    this.organizationApi.getTerritories(areaID).subscribe((items) => this.territories.set(items));
  }

  protected statusName(status: EmployeeStatus): string {
    return EmployeeStatus[status];
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const raw = this.form.getRawValue();
    const request: SaveEmployeeRequest = {
      userID: raw.userID,
      designationID: raw.designationID,
      managerEmployeeID: raw.managerEmployeeID,
      branchID: raw.branchID,
      regionID: raw.regionID,
      areaID: raw.areaID,
      territoryID: raw.territoryID,
      joiningDate: raw.joiningDate,
      endDate: raw.endDate || null,
      status: raw.status,
      phone: raw.phone || null,
      email: raw.email || null,
    };
    let request$: Observable<unknown> = this.employeeID
      ? this.api.updateEmployee(this.employeeID, request)
      : this.api.createEmployee({ ...request, employeeCode: raw.employeeCode });

    this.saving.set(true);
    this.errorMessage.set('');
    request$
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => void this.router.navigate(['/administration/employees']),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  private load(): void {
    this.loading.set(true);
    const employee$: Observable<EmployeeDetails | null> = this.employeeID
      ? this.api.getEmployee(this.employeeID)
      : of(null);
    forkJoin({
      branches: this.organizationApi.getBranches(),
      regions: this.organizationApi.getRegions(),
      employee: employee$,
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: ({ branches, regions, employee }) => {
          this.branches.set(branches);
          this.regions.set(regions);
          if (employee) {
            this.form.patchValue({
              employeeCode: employee.employeeCode,
              userID: employee.userID,
              designationID: employee.designationID,
              managerEmployeeID: employee.managerEmployeeID,
              branchID: employee.branchID,
              regionID: employee.regionID,
              areaID: employee.areaID,
              territoryID: employee.territoryID,
              joiningDate: employee.joiningDate,
              endDate: employee.endDate ?? '',
              status: employee.status,
              phone: employee.phone ?? '',
              email: employee.email ?? '',
            });
            this.form.controls.employeeCode.disable();
            if (employee.regionID)
              this.organizationApi.getAreas(employee.regionID).subscribe((areas) => {
                this.areas.set(areas);
                if (employee.areaID)
                  this.organizationApi
                    .getTerritories(employee.areaID)
                    .subscribe((territories) => this.territories.set(territories));
              });
          }
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}
