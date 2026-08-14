import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-page-header',
  standalone: true,
  templateUrl: './page-header.component.html',
  styleUrl: './page-header.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PageHeaderComponent {
  @Input() eyebrow = '';
  @Input() title = '';
  @Input() subtitle = '';
  @Input() primaryActionLabel = '';
  @Input() secondaryActionLabel = '';
  @Input() primaryActionDisabled = false;
  @Input() secondaryActionDisabled = false;

  @Output() readonly primaryAction = new EventEmitter<void>();
  @Output() readonly secondaryAction = new EventEmitter<void>();

  protected get hasActions(): boolean {
    return Boolean(this.primaryActionLabel || this.secondaryActionLabel);
  }
}
