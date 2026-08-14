import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'enumLabel',
  standalone: true,
})
export class EnumLabelPipe implements PipeTransform {
  transform(
    value: string | number | null | undefined,
    labels?: Readonly<Record<string, string>>,
  ): string {
    if (value === null || value === undefined || value === '') {
      return '—';
    }

    const key = String(value);
    const mappedLabel = labels?.[key];

    if (mappedLabel) {
      return mappedLabel;
    }

    return key
      .replace(/[_-]+/g, ' ')
      .replace(/([a-z0-9])([A-Z])/g, '$1 $2')
      .trim()
      .replace(/\b\w/g, (character) => character.toUpperCase());
  }
}
