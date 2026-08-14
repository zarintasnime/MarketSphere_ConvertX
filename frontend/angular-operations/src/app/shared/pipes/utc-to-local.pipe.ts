import { formatDate } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'utcToLocal',
  standalone: true,
})
export class UtcToLocalPipe implements PipeTransform {
  transform(
    value: string | Date | null | undefined,
    format = 'MMM d, y, h:mm a',
    locale = 'en-US',
  ): string {
    if (!value) {
      return '—';
    }

    const date = value instanceof Date ? value : new Date(value);

    if (Number.isNaN(date.getTime())) {
      return '—';
    }

    return formatDate(date, format, locale);
  }
}
