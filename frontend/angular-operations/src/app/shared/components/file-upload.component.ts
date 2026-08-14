import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output,
  signal,
} from '@angular/core';

@Component({
  selector: 'app-file-upload',
  standalone: true,
  templateUrl: './file-upload.component.html',
  styleUrl: './file-upload.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FileUploadComponent {
  @Input() label = 'Select file';
  @Input() accept = '';
  @Input() multiple = false;
  @Input() disabled = false;

  @Output() readonly filesSelected = new EventEmitter<readonly File[]>();

  protected readonly isDragging = signal(false);
  protected readonly selectedNames = signal<readonly string[]>([]);

  protected handleSelection(event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = input.files ? Array.from(input.files) : [];
    this.emitFiles(files);
    input.value = '';
  }

  protected handleDragOver(event: DragEvent): void {
    if (this.disabled) {
      return;
    }

    event.preventDefault();
    this.isDragging.set(true);
  }

  protected handleDragLeave(event: DragEvent): void {
    event.preventDefault();
    this.isDragging.set(false);
  }

  protected handleDrop(event: DragEvent): void {
    if (this.disabled) {
      return;
    }

    event.preventDefault();
    this.isDragging.set(false);
    const files = event.dataTransfer?.files ? Array.from(event.dataTransfer.files) : [];
    this.emitFiles(this.multiple ? files : files.slice(0, 1));
  }

  private emitFiles(files: readonly File[]): void {
    const validFiles = files.filter((file) => file.size > 0);
    this.selectedNames.set(validFiles.map((file) => file.name));
    this.filesSelected.emit(validFiles);
  }
}
