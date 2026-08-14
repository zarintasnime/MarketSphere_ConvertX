import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable, finalize, forkJoin } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { StatusBadgeComponent } from '../../../../shared/components/status-badge.component';
import {
  Area,
  GeographyScopeType,
  Region,
  RouteItem,
  Territory,
  VisitFrequency,
} from '../../models/organization.model';
import { OrganizationApiService } from '../../services/organization-api.service';

@Component({
  selector: 'app-routes-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent, StatusBadgeComponent],
  templateUrl: './routes-page.component.html',
  styleUrl: './routes-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RoutesPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(OrganizationApiService);
  private readonly auth = inject(AuthService);

  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly successMessage = signal('');
  protected readonly regions = signal<readonly Region[]>([]);
  protected readonly areas = signal<readonly Area[]>([]);
  protected readonly territories = signal<readonly Territory[]>([]);
  protected readonly routes = signal<readonly RouteItem[]>([]);
  protected readonly selectedRouteID = signal<number | null>(null);
  protected readonly canManageRoutes = computed(() => this.auth.hasPermission('routes.manage'));
  protected readonly canManageAssignments = computed(() =>
    this.auth.hasPermission('assignments.manage'),
  );
  protected readonly frequencies = [1, 2, 3, 4, 5] as const;
  protected readonly days = [0, 1, 2, 3, 4, 5, 6] as const;
  protected readonly scopeTypes = [1, 2, 3] as const;

  protected readonly routeForm = this.fb.nonNullable.group({
    territoryID: [0, [Validators.required, Validators.min(1)]],
    routeCode: ['', Validators.required],
    routeName: ['', Validators.required],
    dayOfWeek: this.fb.control<number | null>(null),
    visitFrequency: [VisitFrequency.Weekly, Validators.required],
    isActive: [true],
  });

  protected readonly employeeRouteForm = this.fb.nonNullable.group({
    employeeID: [0, [Validators.required, Validators.min(1)]],
    routeID: [0, [Validators.required, Validators.min(1)]],
    effectiveFrom: [new Date().toISOString().slice(0, 10), Validators.required],
    effectiveTo: [''],
    dayOfWeek: this.fb.control<number | null>(null),
    isPrimary: [false],
  });

  protected readonly territoryAssignmentForm = this.fb.nonNullable.group({
    employeeID: [0, [Validators.required, Validators.min(1)]],
    scopeType: [GeographyScopeType.Territory, Validators.required],
    regionID: this.fb.control<number | null>(null),
    areaID: this.fb.control<number | null>(null),
    territoryID: this.fb.control<number | null>(null),
    effectiveFrom: [new Date().toISOString().slice(0, 10), Validators.required],
    effectiveTo: [''],
    isPrimary: [false],
  });

  constructor() {
    this.loadInitial();
  }

  protected loadAreas(regionID: number | null): void {
    this.areas.set([]);
    this.territories.set([]);
    if (!regionID) return;
    this.api
      .getAreas(regionID)
      .subscribe({
        next: (areas) => this.areas.set(areas),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected loadTerritories(areaID: number | null): void {
    this.territories.set([]);
    if (!areaID) return;
    this.api
      .getTerritories(areaID)
      .subscribe({
        next: (territories) => this.territories.set(territories),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected filterRoutes(territoryID: number | null): void {
    this.api
      .getRoutes(territoryID)
      .subscribe({
        next: (routes) => this.routes.set(routes),
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  protected selectRoute(route: RouteItem): void {
    this.selectedRouteID.set(route.routeID);
    this.routeForm.patchValue({
      territoryID: route.territoryID,
      routeCode: route.routeCode,
      routeName: route.routeName,
      dayOfWeek: route.dayOfWeek,
      visitFrequency: route.visitFrequency,
      isActive: route.isActive,
    });
    this.routeForm.controls.territoryID.disable();
    this.routeForm.controls.routeCode.disable();
    this.employeeRouteForm.controls.routeID.setValue(route.routeID);
  }

  protected resetRoute(): void {
    this.selectedRouteID.set(null);
    this.routeForm.controls.territoryID.enable();
    this.routeForm.controls.routeCode.enable();
    this.routeForm.reset({
      territoryID: this.territories()[0]?.territoryID ?? 0,
      routeCode: '',
      routeName: '',
      dayOfWeek: null,
      visitFrequency: VisitFrequency.Weekly,
      isActive: true,
    });
  }

  protected saveRoute(): void {
    if (this.routeForm.invalid || !this.canManageRoutes()) {
      this.routeForm.markAllAsTouched();
      return;
    }
    const raw = this.routeForm.getRawValue();
    const id = this.selectedRouteID();
    const request$: Observable<unknown> = id
      ? this.api.updateRoute(id, {
          routeName: raw.routeName,
          dayOfWeek: raw.dayOfWeek,
          visitFrequency: raw.visitFrequency,
          isActive: raw.isActive,
        })
      : this.api.createRoute({
          territoryID: raw.territoryID,
          routeCode: raw.routeCode,
          routeName: raw.routeName,
          dayOfWeek: raw.dayOfWeek,
          visitFrequency: raw.visitFrequency,
        });
    this.runSave(request$, 'Route saved successfully.', () => {
      this.resetRoute();
      this.filterRoutes(null);
    });
  }

  protected assignEmployeeRoute(): void {
    if (this.employeeRouteForm.invalid || !this.canManageAssignments()) {
      this.employeeRouteForm.markAllAsTouched();
      return;
    }
    const raw = this.employeeRouteForm.getRawValue();
    this.runSave(
      this.api.assignEmployeeRoute({
        employeeID: raw.employeeID,
        routeID: raw.routeID,
        effectiveFrom: raw.effectiveFrom,
        effectiveTo: raw.effectiveTo || null,
        dayOfWeek: raw.dayOfWeek,
        isPrimary: raw.isPrimary,
      }),
      'Employee route assignment created successfully.',
    );
  }

  protected assignEmployeeTerritory(): void {
    if (this.territoryAssignmentForm.invalid || !this.canManageAssignments()) {
      this.territoryAssignmentForm.markAllAsTouched();
      return;
    }
    const raw = this.territoryAssignmentForm.getRawValue();
    this.runSave(
      this.api.assignEmployeeTerritory({
        employeeID: raw.employeeID,
        scopeType: raw.scopeType,
        regionID: raw.scopeType === GeographyScopeType.Region ? raw.regionID : null,
        areaID: raw.scopeType === GeographyScopeType.Area ? raw.areaID : null,
        territoryID: raw.scopeType === GeographyScopeType.Territory ? raw.territoryID : null,
        effectiveFrom: raw.effectiveFrom,
        effectiveTo: raw.effectiveTo || null,
        isPrimary: raw.isPrimary,
      }),
      'Employee geography assignment created successfully.',
    );
  }

  protected frequencyName(value: VisitFrequency): string {
    return VisitFrequency[value];
  }
  protected scopeName(value: GeographyScopeType): string {
    return GeographyScopeType[value];
  }
  protected dayName(value: number | null): string {
    return value === null
      ? 'Any day'
      : (['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'][value] ??
          'Unknown');
  }

  private loadInitial(): void {
    this.loading.set(true);
    forkJoin({ regions: this.api.getRegions(), routes: this.api.getRoutes() })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: ({ regions, routes }) => {
          this.regions.set(regions);
          this.routes.set(routes);
          const regionID = regions[0]?.regionID;
          if (regionID)
            this.api.getAreas(regionID).subscribe((areas) => {
              this.areas.set(areas);
              const areaID = areas[0]?.areaID;
              if (areaID)
                this.api.getTerritories(areaID).subscribe((territories) => {
                  this.territories.set(territories);
                  this.routeForm.controls.territoryID.setValue(territories[0]?.territoryID ?? 0);
                });
            });
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  private runSave(request$: Observable<unknown>, message: string, afterSave?: () => void): void {
    this.saving.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');
    request$.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.successMessage.set(message);
        afterSave?.();
      },
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }
}
