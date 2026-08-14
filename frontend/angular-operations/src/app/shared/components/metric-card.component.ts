import { ChangeDetectionStrategy, Component, Input } from '@angular/core';

export type MetricCardTone = 'brand' | 'info' | 'success' | 'warning' | 'danger';
export type MetricTrendDirection = 'up' | 'down' | 'neutral';

@Component({
  selector: 'app-metric-card',
  standalone: true,
  templateUrl: './metric-card.component.html',
  styleUrl: './metric-card.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MetricCardComponent {
  @Input() label = '';
  @Input() value: string | number = '';
  @Input() hint = '';
  @Input() trendValue = '';
  @Input() trendDirection: MetricTrendDirection = 'neutral';
  @Input() tone: MetricCardTone = 'brand';
}
