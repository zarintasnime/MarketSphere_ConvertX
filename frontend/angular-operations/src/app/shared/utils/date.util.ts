export interface DateRange {
  from: string;
  to: string;
}

export function toDateInputValue(value: string | Date | null | undefined): string {
  if (!value) {
    return '';
  }

  const date = value instanceof Date ? value : new Date(value);

  if (Number.isNaN(date.getTime())) {
    return '';
  }

  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');

  return `${year}-${month}-${day}`;
}

export function toUtcIso(value: string | Date | null | undefined): string | null {
  if (!value) {
    return null;
  }

  const date = value instanceof Date ? value : new Date(value);
  return Number.isNaN(date.getTime()) ? null : date.toISOString();
}

export function isValidDateRange(range: DateRange): boolean {
  if (!range.from || !range.to) {
    return false;
  }

  const from = new Date(range.from);
  const to = new Date(range.to);

  return (
    !Number.isNaN(from.getTime()) && !Number.isNaN(to.getTime()) && from.getTime() <= to.getTime()
  );
}

export function formatShortDate(value: string | Date | null | undefined, locale = 'en-US'): string {
  if (!value) {
    return '—';
  }

  const date = value instanceof Date ? value : new Date(value);

  if (Number.isNaN(date.getTime())) {
    return '—';
  }

  return new Intl.DateTimeFormat(locale, {
    year: 'numeric',
    month: 'short',
    day: '2-digit',
  }).format(date);
}
