export type ApiValidationErrors = Readonly<Record<string, readonly string[]>>;

export interface ApiResponse<T> {
  succeeded: boolean;
  message: string;
  data: T | null;
  errors: ApiValidationErrors | null;
}

export interface ApiProblemDetails {
  title?: string;
  detail?: string;
  status?: number;
  errorCode?: string;
  traceIdentifier?: string;
  errors?: ApiValidationErrors | null;
}

export function flattenApiErrors(errors: ApiValidationErrors | null | undefined): string[] {
  if (!errors) {
    return [];
  }

  return Object.values(errors)
    .flatMap((messages) => [...messages])
    .filter((message) => message.trim().length > 0);
}

export function firstApiError(
  errors: ApiValidationErrors | null | undefined,
  fallback = '',
): string {
  return flattenApiErrors(errors)[0] ?? fallback;
}

export function normalizeValidationKey(key: string): string {
  return key
    .replace(/^\$\./, '')
    .replace(/\[(\d+)\]/g, '.$1')
    .split('.')
    .filter(Boolean)
    .map((segment, index) =>
      index === 0 ? segment.charAt(0).toLowerCase() + segment.slice(1) : segment,
    )
    .join('.');
}

export function requireApiData<T>(response: ApiResponse<T>): T {
  if (!response.succeeded || response.data === null) {
    const validationMessage = firstApiError(response.errors);
    throw new Error(
      validationMessage || response.message || 'The API response did not contain data.',
    );
  }

  return response.data;
}
