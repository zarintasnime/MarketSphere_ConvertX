export interface PagedResult<T> {
  items: readonly T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface PagedRequest {
  pageNumber: number;
  pageSize: number;
  search?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}

export function createEmptyPagedResult<T>(pageNumber = 1, pageSize = 10): PagedResult<T> {
  return {
    items: [],
    totalCount: 0,
    pageNumber,
    pageSize,
    totalPages: 0,
  };
}
