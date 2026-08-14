import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';

export type EmptyStateIcon = 'inbox' | 'search' | 'warning' | 'offline';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  templateUrl: './empty-state.component.html',
  styleUrl: './empty-state.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EmptyStateComponent {
  @Input() title = 'No data found';
  @Input() message = 'There is no information to display yet.';
  @Input() actionLabel = '';
  @Input() icon: EmptyStateIcon = 'inbox';
  @Input() compact = false;

  @Output() readonly actionRequested = new EventEmitter<void>();

  protected requestAction(): void {
    this.actionRequested.emit();
  }
}
