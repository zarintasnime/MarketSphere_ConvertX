export interface ErrorPanelProps {
  title?: string;
  message: string;
  retryLabel?: string;
  traceIdentifier?: string | null;
  onRetry?: () => void;
}

export default function ErrorPanel({
  title = "Unable to load data",
  message,
  retryLabel = "Try again",
  traceIdentifier = null,
  onRetry,
}: ErrorPanelProps) {
  return (
    <section className="error-panel" role="alert" aria-live="assertive">
      <div className="error-panel__icon" aria-hidden="true">
        !
      </div>

      <div className="error-panel__content">
        <h3>{title}</h3>
        <p>{message}</p>

        {traceIdentifier ? <small>Trace ID: {traceIdentifier}</small> : null}

        {onRetry ? (
          <button
            type="button"
            className="msx-button msx-button--secondary"
            onClick={onRetry}
          >
            {retryLabel}
          </button>
        ) : null}
      </div>
    </section>
  );
}
