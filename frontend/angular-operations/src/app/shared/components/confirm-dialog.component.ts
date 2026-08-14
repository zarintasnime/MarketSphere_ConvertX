import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  Output,
  ViewChild,
} from '@angular/core';

export type ConfirmDialogTone = 'primary' | 'danger';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  templateUrl: './confirm-dialog.component.html',
  styleUrl: './confirm-dialog.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ConfirmDialogComponent {
  @Input() open = false;
  @Input() title = 'Confirm action';
  @Input() message = 'Are you sure you want to continue?';
  @Input() confirmText = 'Confirm';
  @Input() cancelText = 'Cancel';
  @Input() tone: ConfirmDialogTone = 'primary';
  @Input() busy = false;

  @Output() readonly confirmed = new EventEmitter<void>();
  @Output() readonly cancelled = new EventEmitter<void>();

  @ViewChild('confirmButton')
  private confirmButton?: ElementRef<HTMLButtonElement>;

  @HostListener('document:keydown.escape')
  protected handleEscape(): void {
    this.cancel();
  }

  protected confirm(): void {
    if (!this.busy) {
      this.confirmed.emit();
    }
  }

  protected cancel(): void {
    if (this.open && !this.busy) {
      this.cancelled.emit();
    }
  }

  protected handleBackdrop(event: MouseEvent): void {
    if (event.target === event.currentTarget) {
      this.cancel();
    }
  }
}
