import { ChangeDetectionStrategy, Component, Input } from '@angular/core';

@Component({
  selector: 'app-loading-panel',
  standalone: true,
  templateUrl: './loading-panel.component.html',
  styleUrl: './loading-panel.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoadingPanelComponent {
  @Input() message = 'Loading...';
  @Input() compact = false;
  @Input() overlay = false;
}
