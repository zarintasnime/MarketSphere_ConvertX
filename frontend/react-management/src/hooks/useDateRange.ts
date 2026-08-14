import { useMemo, useState } from "react";

import type { DateRange } from "../types/common.types";

function toDateInputValue(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
}

function createDefaultRange(): DateRange {
  const today = new Date();
  const firstDay = new Date(today.getFullYear(), today.getMonth(), 1);

  return {
    from: toDateInputValue(firstDay),
    to: toDateInputValue(today),
  };
}

export function useDateRange(initialRange: DateRange = createDefaultRange()) {
  const [range, setRange] = useState<DateRange>(initialRange);

  const apiRange = useMemo(
    () => ({
      from: `${range.from}T00:00:00`,
      to: `${range.to}T23:59:59.999`,
    }),
    [range],
  );

  return {
    range,
    apiRange,
    setRange,
    resetRange: () => setRange(createDefaultRange()),
  };
}
