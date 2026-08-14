import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';

export interface DataTableColumn {
  key: string;
  label: string;
  width?: string;
}

@Component({
  selector: 'app-data-table',
  standalone: true,
  templateUrl: './data-table.component.html',
  styleUrl: './data-table.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DataTableComponent {
  @Input() columns: readonly DataTableColumn[] = [];
  @Input() rows: readonly Record<string, unknown>[] = [];
  @Input() rowKey = 'id';
  @Input() emptyMessage = 'No records were found.';
  @Input() selectable = false;
  @Input() selectedKey: string | number | null = null;

  @Output() readonly rowSelected = new EventEmitter<Record<string, unknown>>();

  protected selectRow(row: Record<string, unknown>): void {
    if (this.selectable) {
      this.rowSelected.emit(row);
    }
  }

  protected isSelected(row: Record<string, unknown>): boolean {
    return this.selectedKey !== null && row[this.rowKey] === this.selectedKey;
  }

  protected displayValue(value: unknown): string {
    if (value === null || value === undefined || value === '') {
      return '—';
    }

    if (Array.isArray(value)) {
      return value.join(', ');
    }

    return String(value);
  }
}
