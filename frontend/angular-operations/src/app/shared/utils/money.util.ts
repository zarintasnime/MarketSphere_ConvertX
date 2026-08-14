export function formatMoney(
  value: number | null | undefined,
  currency = 'USD',
  locale = 'en-US',
): string {
  const safeValue = Number.isFinite(value) ? Number(value) : 0;

  return new Intl.NumberFormat(locale, {
    style: 'currency',
    currency,
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(safeValue);
}

export function roundMoney(value: number, decimalPlaces = 2): number {
  const factor = 10 ** decimalPlaces;
  return Math.round((value + Number.EPSILON) * factor) / factor;
}
