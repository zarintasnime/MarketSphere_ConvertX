import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import {
  FileTransferService,
  type FileAttachment,
} from '../../../../core/services/file-transfer.service';
import { getApiErrorMessage } from '../../../../core/http/error.interceptor';
import { LoadingPanelComponent } from '../../../../shared/components/loading-panel.component';
import { PageHeaderComponent } from '../../../../shared/components/page-header.component';
import { StatusBadgeComponent } from '../../../../shared/components/status-badge.component';
import type { VisitDetails } from '../../models/marketing.model';
import { MarketingApiService } from '../../services/marketing-api.service';
@Component({
  selector: 'app-visit-details-page',
  standalone: true,
  imports: [RouterLink, LoadingPanelComponent, PageHeaderComponent, StatusBadgeComponent],
  templateUrl: './visit-details-page.component.html',
  styleUrl: './visit-details-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class VisitDetailsPageComponent {
  private readonly api = inject(MarketingApiService);
  private readonly files = inject(FileTransferService);
  private readonly route = inject(ActivatedRoute);
  protected readonly visitID = Number(this.route.snapshot.paramMap.get('visitID'));
  protected readonly item = signal<VisitDetails | null>(null);
  protected readonly attachments = signal<readonly FileAttachment[]>([]);
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal('');
  constructor() {
    this.load();
  }
  protected load(): void {
    this.loading.set(true);
    this.api
      .getVisit(this.visitID)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (item) => {
          this.item.set(item);
          this.files
            .getAttachments('Visit', this.visitID)
            .subscribe({ next: (files) => this.attachments.set(files) });
        },
        error: (error: unknown) => this.errorMessage.set(getApiErrorMessage(error)),
      });
  }
  protected download(file: FileAttachment): void {
    this.files.download(file.fileAttachmentID, file.fileName).subscribe({ next: () => undefined });
  }
}
