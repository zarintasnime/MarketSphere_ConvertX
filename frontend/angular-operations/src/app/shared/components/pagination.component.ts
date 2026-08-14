import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  standalone: true,
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PaginationComponent {
  @Input() pageNumber = 1;
  @Input() pageSize = 10;
  @Input() totalCount = 0;
  @Input() totalPages = 0;
  @Input() disabled = false;

  @Output() readonly pageChange = new EventEmitter<number>();

  protected get firstItemNumber(): number {
    return this.totalCount === 0 ? 0 : (this.pageNumber - 1) * this.pageSize + 1;
  }

  protected get lastItemNumber(): number {
    return Math.min(this.pageNumber * this.pageSize, this.totalCount);
  }

  protected get visiblePages(): readonly (number | null)[] {
    if (this.totalPages <= 7) {
      return Array.from({ length: this.totalPages }, (_, index) => index + 1);
    }

    const pages = new Set<number>([
      1,
      this.totalPages,
      this.pageNumber - 1,
      this.pageNumber,
      this.pageNumber + 1,
    ]);

    const sortedPages = [...pages]
      .filter((page) => page >= 1 && page <= this.totalPages)
      .sort((left, right) => left - right);

    const result: (number | null)[] = [];

    sortedPages.forEach((page, index) => {
      const previousPage = sortedPages[index - 1];

      if (index > 0 && previousPage !== undefined && page - previousPage > 1) {
        result.push(null);
      }

      result.push(page);
    });

    return result;
  }

  protected goToPage(page: number): void {
    if (this.disabled || page < 1 || page > this.totalPages || page === this.pageNumber) {
      return;
    }

    this.pageChange.emit(page);
  }
}
