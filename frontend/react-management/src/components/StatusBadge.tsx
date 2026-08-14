import type { StatusTone } from "../types/common.types";

export interface StatusBadgeProps {
  label: string;
  tone?: StatusTone;
  showDot?: boolean;
}

export default function StatusBadge({
  label,
  tone = "neutral",
  showDot = true,
}: StatusBadgeProps) {
  return (
    <span className={`status-badge status-badge--${tone}`}>
      {showDot ? (
        <span className="status-badge__dot" aria-hidden="true" />
      ) : null}
      <span>{label}</span>
    </span>
  );
}
