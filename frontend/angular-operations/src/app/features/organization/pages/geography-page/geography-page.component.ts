import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable, finalize, forkJoin } from 'rxjs';

import { AuthService } from '../../../../core/auth/auth.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import type { Area, Company, Region, Territory } from '../../models/organization.model';
import { OrganizationApiService } from '../../services/organization-api.service';

@Component({
  selector: 'app-geography-page',
  standalone: true,
  imports: [ReactiveFormsModule, LoadingPanelComponent, PageHeaderComponent],
  templateUrl: './geography-page.component.html',
  styleUrl: './geography-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GeographyPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(OrganizationApiService);
  private readonly auth = inject(AuthService);

  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly companies = signal<readonly Company[]>([]);
  protected readonly regions = signal<readonly Region[]>([]);
  protected readonly areas = signal<readonly Area[]>([]);
  protected readonly territories = signal<readonly Territory[]>([]);
  protected readonly selectedRegionID = signal<number | null>(null);
  protected readonly selectedAreaID = signal<number | null>(null);
  protected readonly selectedTerritoryID = signal<number | null>(null);
  protected readonly canManage = computed(() => this.auth.hasPermission('geography.manage'));

  protected readonly regionForm = this.fb.nonNullable.group({
    companyID: [0, [Validators.required, Validators.min(1)]],
    regionCode: ['', Validators.required],
    regionName: ['', Validators.required],
    isActive: [true],
  });
  protected readonly areaForm = this.fb.nonNullable.group({
    regionID: [0, [Validators.required, Validators.min(1)]],
    areaCode: ['', Validators.required],
    areaName: ['', Validators.required],
    isActive: [true],
  });
  protected readonly territoryForm = this.fb.nonNullable.group({
    areaID: [0, [Validators.required, Validators.min(1)]],
    territoryCode: ['', Validators.required],
    territoryName: ['', Validators.required],
    isActive: [true],
  });

  constructor() {
    this.loadInitial();
  }

  protected selectRegion(region: Region): void {
    this.selectedRegionID.set(region.regionID);
    this.regionForm.patchValue({
      companyID: region.companyID,
      regionCode: region.regionCode,
      regionName: region.regionName,
      isActive: region.isActive,
    });
    this.regionForm.controls.companyID.disable();
    this.regionForm.controls.regionCode.disable();
    this.areaForm.controls.regionID.setValue(region.regionID);
    this.loadAreas(region.regionID);
  }

  protected selectArea(area: Area): void {
    this.selectedAreaID.set(area.areaID);
    this.areaForm.patchValue({
      regionID: area.regionID,
      areaCode: area.areaCode,
      areaName: area.areaName,
      isActive: area.isActive,
    });
    this.areaForm.controls.regionID.disable();
    this.areaForm.controls.areaCode.disable();
    this.territoryForm.controls.areaID.setValue(area.areaID);
    this.loadTerritories(area.areaID);
  }

  protected selectTerritory(territory: Territory): void {
    this.selectedTerritoryID.set(territory.territoryID);
    this.territoryForm.patchValue({
      areaID: territory.areaID,
      territoryCode: territory.territoryCode,
      territoryName: territory.territoryName,
      isActive: territory.isActive,
    });
    this.territoryForm.controls.areaID.disable();
    this.territoryForm.controls.territoryCode.disable();
  }

  protected resetRegion(): void {
    this.selectedRegionID.set(null);
    this.regionForm.controls.companyID.enable();
    this.regionForm.controls.regionCode.enable();
    this.regionForm.reset({
      companyID: this.companies()[0]?.companyID ?? 0,
      regionCode: '',
      regionName: '',
      isActive: true,
    });
  }
  protected resetArea(): void {
    this.selectedAreaID.set(null);
    this.areaForm.controls.regionID.enable();
    this.areaForm.controls.areaCode.enable();
    this.areaForm.reset({
      regionID: this.selectedRegionID() ?? this.regions()[0]?.regionID ?? 0,
      areaCode: '',
      areaName: '',
      isActive: true,
    });
  }
  protected resetTerritory(): void {
    this.selectedTerritoryID.set(null);
    this.territoryForm.controls.areaID.enable();
    this.territoryForm.controls.territoryCode.enable();
    this.territoryForm.reset({
      areaID: this.selectedAreaID() ?? this.areas()[0]?.areaID ?? 0,
      territoryCode: '',
      territoryName: '',
      isActive: true,
    });
  }

  protected saveRegion(): void {
    if (this.regionForm.invalid || !this.canManage()) {
      this.regionForm.markAllAsTouched();
      return;
    }
    const raw = this.regionForm.getRawValue();
    const id = this.selectedRegionID();
    const request$: Observable<unknown> = id
      ? this.api.updateRegion(id, { regionName: raw.regionName, isActive: raw.isActive })
      : this.api.createRegion({
          companyID: raw.companyID,
          regionCode: raw.regionCode,
          regionName: raw.regionName,
        });
    this.runSave(request$, () => {
      this.resetRegion();
      this.loadRegions();
    });
  }

  protected saveArea(): void {
    if (this.areaForm.invalid || !this.canManage()) {
      this.areaForm.markAllAsTouched();
      return;
    }
    const raw = this.areaForm.getRawValue();
    const id = this.selectedAreaID();
    const request$: Observable<unknown> = id
      ? this.api.updateArea(id, { areaName: raw.areaName, isActive: raw.isActive })
      : this.api.createArea({
          regionID: raw.regionID,
          areaCode: raw.areaCode,
          areaName: raw.areaName,
        });
    this.runSave(request$, () => {
      const regionID = raw.regionID;
      this.resetArea();
      this.loadAreas(regionID);
    });
  }

  protected saveTerritory(): void {
    if (this.territoryForm.invalid || !this.canManage()) {
      this.territoryForm.markAllAsTouched();
      return;
    }
    const raw = this.territoryForm.getRawValue();
    const id = this.selectedTerritoryID();
    const request$: Observable<unknown> = id
      ? this.api.updateTerritory(id, { territoryName: raw.territoryName, isActive: raw.isActive })
      : this.api.createTerritory({
          areaID: raw.areaID,
          territoryCode: raw.territoryCode,
          territoryName: raw.territoryName,
        });
    this.runSave(request$, () => {
      const areaID = raw.areaID;
      this.resetTerritory();
      this.loadTerritories(areaID);
    });
  }

  protected loadAreas(regionID: number): void {
    this.areas.set([]);
    this.territories.set([]);
    this.selectedAreaID.set(null);
    this.selectedTerritoryID.set(null);
    this.api.getAreas(regionID).subscribe({
      next: (areas) => {
        this.areas.set(areas);
        this.areaForm.controls.regionID.setValue(regionID);
        this.territoryForm.controls.areaID.setValue(areas[0]?.areaID ?? 0);
      },
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  protected loadTerritories(areaID: number): void {
    this.territories.set([]);
    this.selectedTerritoryID.set(null);
    this.api.getTerritories(areaID).subscribe({
      next: (territories) => {
        this.territories.set(territories);
        this.territoryForm.controls.areaID.setValue(areaID);
      },
      error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
    });
  }

  private loadInitial(): void {
    this.loading.set(true);
    forkJoin({ companies: this.api.getCompanies(), regions: this.api.getRegions() })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: ({ companies, regions }) => {
          this.companies.set(companies);
          this.regions.set(regions);
          this.regionForm.controls.companyID.setValue(companies[0]?.companyID ?? 0);
          const regionID = regions[0]?.regionID;
          if (regionID) {
            this.areaForm.controls.regionID.setValue(regionID);
            this.loadAreas(regionID);
          }
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }

  private loadRegions(): void {
    this.api.getRegions().subscribe((regions) => this.regions.set(regions));
  }
  private runSave(request$: Observable<unknown>, onSuccess: () => void): void {
    this.saving.set(true);
    this.errorMessage.set('');
    request$
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: onSuccess,
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
}
