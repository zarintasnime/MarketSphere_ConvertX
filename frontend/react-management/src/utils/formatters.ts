export function formatNumber(value: number, maximumFractionDigits = 0): string {
  return new Intl.NumberFormat("en-US", {
    maximumFractionDigits,
  }).format(value);
}

export function formatCompactNumber(
  value: number,
  maximumFractionDigits = 1,
): string {
  return new Intl.NumberFormat("en-US", {
    notation: "compact",
    maximumFractionDigits,
  }).format(value);
}

export function formatCurrency(
  value: number,
  currency = "BDT",
  maximumFractionDigits = 0,
): string {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency,
    maximumFractionDigits,
  }).format(value);
}

export function formatQuantity(
  value: number,
  maximumFractionDigits = 2,
): string {
  return formatNumber(value, maximumFractionDigits);
}

export function formatDate(value: string | Date | null | undefined): string {
  if (!value) return "—";

  const date = value instanceof Date ? value : new Date(value);
  if (Number.isNaN(date.getTime())) return "—";

  return new Intl.DateTimeFormat("en-US", {
    year: "numeric",
    month: "short",
    day: "2-digit",
  }).format(date);
}

export function formatDateTime(
  value: string | Date | null | undefined,
): string {
  if (!value) return "—";

  const date = value instanceof Date ? value : new Date(value);
  if (Number.isNaN(date.getTime())) return "—";

  return new Intl.DateTimeFormat("en-US", {
    year: "numeric",
    month: "short",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  }).format(date);
}

export function formatPercent(value: number | null | undefined): string {
  if (value === null || value === undefined || Number.isNaN(value)) return "—";
  return `${value.toFixed(1)}%`;
}

export function formatRatio(numerator: number, denominator: number): string {
  if (denominator <= 0) return "—";
  return formatPercent((numerator / denominator) * 100);
}

export function formatEnumLabel(
  value: number | null | undefined,
  labels: Readonly<Record<number, string>>,
  fallback = "Unknown",
): string {
  if (value === null || value === undefined) return "—";
  return labels[value] ?? `${fallback} (${value})`;
}

export function humanizeCode(value: string): string {
  return value
    .replace(/[._-]+/g, " ")
    .replace(/([a-z])([A-Z])/g, "$1 $2")
    .replace(/\b\w/g, (character) => character.toUpperCase());
}
