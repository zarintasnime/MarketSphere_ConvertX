import { ChangeDetectionStrategy, Component, Input } from '@angular/core';

export type StatusBadgeTone = 'neutral' | 'info' | 'success' | 'warning' | 'danger';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  templateUrl: './status-badge.component.html',
  styleUrl: './status-badge.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StatusBadgeComponent {
  @Input() label = '';
  @Input() tone: StatusBadgeTone = 'neutral';
  @Input() showDot = true;
}
