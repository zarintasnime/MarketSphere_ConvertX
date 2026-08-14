import type { FormEvent } from "react";

import type { DateRange } from "../types/common.types";

export interface FilterBarProps {
  range: DateRange;
  onRangeChange: (range: DateRange) => void;
  onApply: () => void;
  onReset?: () => void;
  busy?: boolean;
  children?: React.ReactNode;
}

export default function FilterBar({
  range,
  onRangeChange,
  onApply,
  onReset,
  busy = false,
  children,
}: FilterBarProps) {
  const submit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    onApply();
  };

  return (
    <form className="filter-bar" onSubmit={submit}>
      <label>
        <span>From</span>
        <input
          type="date"
          value={range.from}
          max={range.to}
          onChange={(event) =>
            onRangeChange({ ...range, from: event.target.value })
          }
          required
        />
      </label>

      <label>
        <span>To</span>
        <input
          type="date"
          value={range.to}
          min={range.from}
          onChange={(event) =>
            onRangeChange({ ...range, to: event.target.value })
          }
          required
        />
      </label>

      {children}

      <div className="filter-bar__actions">
        {onReset ? (
          <button
            type="button"
            className="msx-button msx-button--ghost"
            onClick={onReset}
            disabled={busy}
          >
            Reset
          </button>
        ) : null}
        <button
          type="submit"
          className="msx-button msx-button--primary"
          disabled={busy}
        >
          {busy ? "Applying..." : "Apply filters"}
        </button>
      </div>
    </form>
  );
}
