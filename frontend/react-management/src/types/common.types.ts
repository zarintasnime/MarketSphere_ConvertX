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

export interface PagedRequest {
  pageNumber: number;
  pageSize: number;
  search?: string;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface PagedResult<T> {
  items: readonly T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface Lookup {
  id: number;
  code: string;
  name: string;
}

export interface SelectOption {
  value: string;
  label: string;
  disabled?: boolean;
}

export type AsyncStatus = "idle" | "loading" | "success" | "error";

export type StatusTone = "neutral" | "info" | "success" | "warning" | "danger";

export interface DateRange {
  from: string;
  to: string;
}

export interface ApiState<T> {
  data: T | null;
  status: AsyncStatus;
  errorMessage: string;
}

export function createEmptyPagedResult<T>(pageSize = 20): PagedResult<T> {
  return {
    items: [],
    totalCount: 0,
    pageNumber: 1,
    pageSize,
    totalPages: 0,
  };
}

export function flattenValidationErrors(
  errors: ApiValidationErrors | null | undefined,
): string[] {
  return errors
    ? Object.values(errors)
        .flatMap((messages) => [...messages])
        .filter((message) => message.trim().length > 0)
    : [];
}
