import { type FormEvent, useEffect, useRef, useState } from "react";

import type {
  ApprovalActionRequest,
  ApprovalActionType,
  ApprovalRequest,
} from "../types/approval.types";

export interface ApprovalActionDialogProps {
  request: ApprovalRequest | null;
  action: Extract<ApprovalActionType, 2 | 3 | 6> | null;
  busy?: boolean;
  errorMessage?: string;
  onClose: () => void;
  onSubmit: (payload: ApprovalActionRequest) => Promise<void> | void;
}

const actionLabels: Readonly<Record<2 | 3 | 6, string>> = {
  2: "Approve request",
  3: "Reject request",
  6: "Add comment",
};

export default function ApprovalActionDialog({
  request,
  action,
  busy = false,
  errorMessage = "",
  onClose,
  onSubmit,
}: ApprovalActionDialogProps) {
  const [note, setNote] = useState("");
  const noteRef = useRef<HTMLTextAreaElement | null>(null);

  useEffect(() => {
    if (!request || !action) {
      return;
    }

    const timeoutID = window.setTimeout(() => {
      setNote("");
      noteRef.current?.focus();
    }, 0);

    return () => {
      window.clearTimeout(timeoutID);
    };
  }, [request, action]);

  useEffect(() => {
    if (!request || !action) {
      return;
    }

    const onKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Escape" && !busy) {
        onClose();
      }
    };

    document.addEventListener("keydown", onKeyDown);

    return () => {
      document.removeEventListener("keydown", onKeyDown);
    };
  }, [request, action, busy, onClose]);

  if (!request || !action) {
    return null;
  }

  const submit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (busy) {
      return;
    }

    await onSubmit({
      action,
      note: note.trim() || null,
      delegateToUserID: null,
    });
  };

  return (
    <div
      className="dialog-backdrop"
      role="presentation"
      onMouseDown={() => {
        if (!busy) onClose();
      }}
    >
      <section
        className="dialog-card"
        role="dialog"
        aria-modal="true"
        aria-labelledby="approval-dialog-title"
        aria-describedby={errorMessage ? "approval-dialog-error" : undefined}
        onMouseDown={(event) => event.stopPropagation()}
      >
        <header className="dialog-card__header">
          <div>
            <p className="dialog-card__eyebrow">
              {request.referenceType} #{request.referenceID}
            </p>
            <h2 id="approval-dialog-title">{actionLabels[action]}</h2>
          </div>
          <button
            type="button"
            className="icon-button"
            aria-label="Close dialog"
            onClick={onClose}
            disabled={busy}
          >
            ×
          </button>
        </header>

        <form onSubmit={submit}>
          <label className="form-field">
            <span>{action === 3 ? "Reason" : "Note"}</span>
            <textarea
              ref={noteRef}
              rows={5}
              value={note}
              onChange={(event) => setNote(event.target.value)}
              required={action === 3}
              maxLength={1000}
              disabled={busy}
              placeholder={
                action === 3
                  ? "Explain why this request is being rejected."
                  : "Add an optional management note."
              }
            />
          </label>

          {errorMessage ? (
            <p id="approval-dialog-error" className="form-error" role="alert">
              {errorMessage}
            </p>
          ) : null}

          <div className="dialog-card__actions">
            <button
              type="button"
              className="msx-button msx-button--ghost"
              onClick={onClose}
              disabled={busy}
            >
              Cancel
            </button>
            <button
              type="submit"
              className={`msx-button ${
                action === 3 ? "msx-button--danger" : "msx-button--primary"
              }`}
              disabled={busy}
            >
              {busy ? "Saving..." : actionLabels[action]}
            </button>
          </div>
        </form>
      </section>
    </div>
  );
}
