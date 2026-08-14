export interface LoadingPanelProps {
  message?: string;
  compact?: boolean;
  overlay?: boolean;
}

export default function LoadingPanel({
  message = "Loading...",
  compact = false,
  overlay = false,
}: LoadingPanelProps) {
  const className = [
    "loading-panel",
    compact ? "loading-panel--compact" : "",
    overlay ? "loading-panel--overlay" : "",
  ]
    .filter(Boolean)
    .join(" ");

  return (
    <div
      className={className}
      role="status"
      aria-live="polite"
      aria-busy="true"
    >
      <span className="loading-panel__spinner" aria-hidden="true" />
      <span className="loading-panel__message">{message}</span>
    </div>
  );
}
