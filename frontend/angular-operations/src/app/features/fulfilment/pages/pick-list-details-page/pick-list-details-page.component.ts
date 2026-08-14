import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';

import type { PagedRequest } from '../../../../core/models/paged-result.model';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import {
  StatusBadgeComponent,
  type StatusBadgeTone,
} from '../../../../shared/components/status-badge.component';
import type { EmployeeListItem } from '../../../administration/models/administration.model';
import { AdministrationApiService } from '../../../administration/services/administration-api.service';
import {
  PickListStatus,
  type PickListDetails,
  type PickListItem,
} from '../../models/fulfilment.model';
import { FulfilmentApiService } from '../../services/fulfilment-api.service';

@Component({
  selector: 'app-pick-list-details-page',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    LoadingPanelComponent,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './pick-list-details-page.component.html',
  styleUrl: './pick-list-details-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PickListDetailsPageComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(FulfilmentApiService);
  private readonly adminApi = inject(AdministrationApiService);

  protected readonly item = signal<PickListDetails | null>(null);

  protected readonly employees = signal<readonly EmployeeListItem[]>([]);

  protected readonly loading = signal(true);
  protected readonly error = signal('');
  protected readonly success = signal('');

  protected releasedByEmployeeID = 0;
  protected verifiedByEmployeeID = 0;
  protected verifyNote = '';

  protected readonly pickValues = new Map<
    number,
    {
      pickedQuantity: number;
      shortQuantity: number;
      pickedByEmployeeID: number;
      verificationNote: string;
    }
  >();

  private readonly id = Number(this.route.snapshot.paramMap.get('id'));

  constructor() {
    this.refresh();

    const request: PagedRequest = {
      pageNumber: 1,
      pageSize: 500,
    };

    this.adminApi.getEmployees(request).subscribe({
      next: (result) => {
        this.employees.set(result.items);
      },
      error: () => {
        this.error.set('Unable to load employees.');
      },
    });
  }

  protected pickValue(line: PickListItem) {
    if (!this.pickValues.has(line.pickListItemID)) {
      this.pickValues.set(line.pickListItemID, {
        pickedQuantity: line.pickedQuantity || line.requestedQuantity,

        shortQuantity: line.shortQuantity || 0,

        pickedByEmployeeID: line.pickedByEmployeeID || 0,

        verificationNote: line.verificationNote || '',
      });
    }

    return this.pickValues.get(line.pickListItemID)!;
  }

  protected canRecord(status: PickListStatus): boolean {
    return [2, 3, 4].includes(Number(status));
  }

  protected canVerify(status: PickListStatus): boolean {
    return [4, 5].includes(Number(status));
  }

  protected isRecorded(status: PickListStatus): boolean {
    return [5, 6].includes(Number(status));
  }

  protected release(): void {
    this.error.set('');
    this.success.set('');

    if (!this.releasedByEmployeeID) {
      this.error.set('Select the releasing employee.');

      return;
    }

    this.api.releasePickList(this.id, this.releasedByEmployeeID).subscribe({
      next: () => {
        this.error.set('');
        this.success.set('Pick list released.');

        this.pickValues.clear();
        this.refresh();
      },

      error: () => {
        this.success.set('');
        this.error.set('Unable to release the pick list.');
      },
    });
  }

  protected record(line: PickListItem): void {
    this.error.set('');
    this.success.set('');

    const pickList = this.item();

    if (!pickList || !this.canRecord(pickList.status)) {
      this.error.set('This pick list is no longer open for recording.');

      return;
    }

    const value = this.pickValue(line);

    if (!value.pickedByEmployeeID) {
      this.error.set('Select the picking employee.');

      return;
    }

    const pickedQuantity = Number(value.pickedQuantity);

    const shortQuantity = Number(value.shortQuantity);

    if (!Number.isFinite(pickedQuantity) || pickedQuantity < 0) {
      this.error.set('Picked quantity must be zero or greater.');

      return;
    }

    if (!Number.isFinite(shortQuantity) || shortQuantity < 0) {
      this.error.set('Short quantity must be zero or greater.');

      return;
    }

    if (pickedQuantity + shortQuantity > line.requestedQuantity) {
      this.error.set('Picked and short quantities cannot exceed the requested quantity.');

      return;
    }

    this.api
      .recordPick(this.id, {
        pickListItemID: line.pickListItemID,

        pickedQuantity,

        shortQuantity,

        pickedByEmployeeID: Number(value.pickedByEmployeeID),

        verificationNote: value.verificationNote.trim() || null,
      })
      .subscribe({
        next: () => {
          this.error.set('');

          this.success.set(`Pick recorded for ${line.skuCode}.`);

          this.pickValues.delete(line.pickListItemID);

          this.refresh();
        },

        error: () => {
          this.success.set('');

          this.error.set('Unable to record picked quantity.');
        },
      });
  }

  protected verify(): void {
    this.error.set('');
    this.success.set('');

    const pickList = this.item();

    if (!pickList || !this.canVerify(pickList.status)) {
      this.error.set('The pick list is not ready for verification.');

      return;
    }

    if (!this.verifiedByEmployeeID) {
      this.error.set('Select the verifying employee.');

      return;
    }

    this.api
      .verifyPickList(this.id, this.verifiedByEmployeeID, this.verifyNote.trim() || null)
      .subscribe({
        next: () => {
          this.error.set('');

          this.success.set('Pick list verified.');

          this.pickValues.clear();
          this.refresh();
        },

        error: () => {
          this.success.set('');

          this.error.set('Unable to verify the pick list.');
        },
      });
  }

  private refresh(): void {
    this.api.getPickList(this.id).subscribe({
      next: (result) => {
        this.item.set(result);
        this.loading.set(false);
      },

      error: () => {
        this.error.set('Unable to load pick list details.');

        this.loading.set(false);
      },
    });
  }

  protected label(value: PickListStatus): string {
    return PickListStatus[value] ?? 'Unknown';
  }

  protected tone(value: PickListStatus): StatusBadgeTone {
    if (Number(value) === 6) {
      return 'success';
    }

    if (Number(value) === 7) {
      return 'danger';
    }

    if ([2, 3, 4, 5].includes(Number(value))) {
      return 'warning';
    }

    return 'neutral';
  }
}
