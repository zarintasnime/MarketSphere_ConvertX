export type EmptyStateIcon = "inbox" | "search" | "warning" | "offline";

export interface EmptyStateProps {
  title?: string;
  message?: string;
  actionLabel?: string;
  icon?: EmptyStateIcon;
  compact?: boolean;
  onAction?: () => void;
}

const iconText: Readonly<Record<EmptyStateIcon, string>> = {
  inbox: "□",
  search: "⌕",
  warning: "!",
  offline: "↯",
};

export default function EmptyState({
  title = "No data found",
  message = "There is no information to display yet.",
  actionLabel,
  icon = "inbox",
  compact = false,
  onAction,
}: EmptyStateProps) {
  return (
    <section className={`empty-state${compact ? " empty-state--compact" : ""}`}>
      <div className="empty-state__icon" aria-hidden="true">
        {iconText[icon]}
      </div>

      <h3>{title}</h3>
      <p>{message}</p>

      {actionLabel && onAction ? (
        <button
          type="button"
          className="msx-button msx-button--primary"
          onClick={onAction}
        >
          {actionLabel}
        </button>
      ) : null}
    </section>
  );
}
