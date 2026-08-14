import type { Key, ReactNode } from "react";

import EmptyState from "./EmptyState";
import LoadingPanel from "./LoadingPanel";

export interface DataTableColumn<T> {
  key: string;
  header: string;
  render: (row: T) => ReactNode;
  className?: string;
}

export interface DataTableProps<T> {
  rows: readonly T[];
  columns: readonly DataTableColumn<T>[];
  keyExtractor: (row: T) => Key;
  loading?: boolean;
  loadingMessage?: string;
  emptyTitle?: string;
  emptyMessage?: string;
  onRowClick?: (row: T) => void;
}

export default function DataTable<T>({
  rows,
  columns,
  keyExtractor,
  loading = false,
  loadingMessage = "Loading records...",
  emptyTitle = "No records found",
  emptyMessage = "Try changing the current filters.",
  onRowClick,
}: DataTableProps<T>) {
  if (loading) {
    return <LoadingPanel message={loadingMessage} />;
  }

  if (rows.length === 0) {
    return <EmptyState title={emptyTitle} message={emptyMessage} />;
  }

  return (
    <div className="data-table-wrap">
      <table className="data-table">
        <thead>
          <tr>
            {columns.map((column) => (
              <th key={column.key} className={column.className} scope="col">
                {column.header}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {rows.map((row) => (
            <tr
              key={keyExtractor(row)}
              className={onRowClick ? "data-table__clickable-row" : undefined}
              onClick={onRowClick ? () => onRowClick(row) : undefined}
            >
              {columns.map((column) => (
                <td key={column.key} className={column.className}>
                  {column.render(row)}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
