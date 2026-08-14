import {
  type SyntheticEvent,
  useCallback,
  useEffect,
  useMemo,
  useState,
} from "react";

import {
  getApprovalQueue,
  getApprovalRequest,
  recordApprovalAction,
} from "../api/approvalApi";
import { getApiErrorMessage } from "../api/httpClient";
import ApprovalActionDialog from "../components/ApprovalActionDialog";
import DataTable, { type DataTableColumn } from "../components/DataTable";
import ErrorPanel from "../components/ErrorPanel";
import StatusBadge from "../components/StatusBadge";
import { useApi } from "../hooks/useApi";
import { usePermissions } from "../hooks/usePermissions";
import {
  createEmptyPagedResult,
  type PagedResult,
  type StatusTone,
} from "../types/common.types";
import type {
  ApprovalActionRequest,
  ApprovalActionType,
  ApprovalRequest,
  ApprovalRequestStatus,
} from "../types/approval.types";
import { formatDateTime, humanizeCode } from "../utils/formatters";

const PAGE_SIZE = 20;

const statusLabels: Readonly<Record<ApprovalRequestStatus, string>> = {
  1: "Pending",
  2: "In progress",
  3: "Approved",
  4: "Rejected",
  5: "Cancelled",
};

const statusTones: Readonly<Record<ApprovalRequestStatus, StatusTone>> = {
  1: "warning",
  2: "info",
  3: "success",
  4: "danger",
  5: "neutral",
};

const actionLabels: Readonly<Record<ApprovalActionType, string>> = {
  1: "Submitted",
  2: "Approved",
  3: "Rejected",
  4: "Cancelled",
  5: "Delegated",
  6: "Commented",
};

export default function ApprovalQueuePage() {
  const { hasPermission } = usePermissions();

  const canAct = hasPermission("infrastructure.approvals.act");

  const queue = useApi<PagedResult<ApprovalRequest>>(
    createEmptyPagedResult(PAGE_SIZE),
  );

  const details = useApi<ApprovalRequest>();

  const {
    data: queueData,
    isLoading: queueLoading,
    errorMessage: queueErrorMessage,
    execute: executeQueue,
  } = queue;

  const {
    data: activeRequest,
    execute: executeDetails,
    setData: setDetailsData,
  } = details;

  const [pageNumber, setPageNumber] = useState(1);
  const [search, setSearch] = useState("");
  const [appliedSearch, setAppliedSearch] = useState("");

  const [dialogAction, setDialogAction] = useState<Extract<
    ApprovalActionType,
    2 | 3 | 6
  > | null>(null);

  const [actionBusy, setActionBusy] = useState(false);
  const [actionError, setActionError] = useState("");

  const loadQueue = useCallback(
    () =>
      executeQueue(() =>
        getApprovalQueue({
          pageNumber,
          pageSize: PAGE_SIZE,
          search: appliedSearch || undefined,
          sortBy: "requestedAt",
          sortDescending: true,
        }),
      ),
    [appliedSearch, pageNumber, executeQueue],
  );

  useEffect(() => {
    void loadQueue();
  }, [loadQueue]);

  const openDetails = useCallback(
    async (request: ApprovalRequest): Promise<void> => {
      await executeDetails(() => getApprovalRequest(request.approvalRequestID));
    },
    [executeDetails],
  );

  const submitSearch = (event: SyntheticEvent<HTMLFormElement>): void => {
    event.preventDefault();

    setPageNumber(1);
    setAppliedSearch(search.trim());
  };

  const submitAction = async (
    payload: ApprovalActionRequest,
  ): Promise<void> => {
    if (!activeRequest) {
      return;
    }

    const currentRequest = activeRequest;

    setActionBusy(true);
    setActionError("");

    try {
      await recordApprovalAction(currentRequest.approvalRequestID, payload);

      setDialogAction(null);

      await Promise.all([loadQueue(), openDetails(currentRequest)]);
    } catch (error: unknown) {
      setActionError(getApiErrorMessage(error));
    } finally {
      setActionBusy(false);
    }
  };

  const columns = useMemo<readonly DataTableColumn<ApprovalRequest>[]>(
    () => [
      {
        key: "request",
        header: "Request",
        render: (row) => (
          <div className="table-primary">
            <strong>#{row.approvalRequestID}</strong>

            <span>{humanizeCode(row.referenceType)}</span>
          </div>
        ),
      },
      {
        key: "reference",
        header: "Reference",
        render: (row) => `#${row.referenceID}`,
      },
      {
        key: "step",
        header: "Current step",
        render: (row) => row.currentStepNo,
      },
      {
        key: "requested",
        header: "Requested",
        render: (row) => formatDateTime(row.requestedAt),
      },
      {
        key: "status",
        header: "Status",
        render: (row) => (
          <StatusBadge
            label={statusLabels[row.status]}
            tone={statusTones[row.status]}
          />
        ),
      },
      {
        key: "open",
        header: "",
        render: () => <span className="table-link">View details</span>,
      },
    ],
    [],
  );

  const result =
    queueData ?? createEmptyPagedResult<ApprovalRequest>(PAGE_SIZE);

  const canReceiveAction =
    activeRequest?.status === 1 || activeRequest?.status === 2;

  return (
    <main className="management-page">
      <header className="page-heading page-heading--actions">
        <div>
          <p className="page-heading__eyebrow">Management actions</p>

          <h1>Approval Queue</h1>

          <p>
            Review active requests, inspect action history, and record
            management decisions.
          </p>
        </div>

        <form className="search-form" onSubmit={submitSearch}>
          <input
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Search reference type"
            aria-label="Search approval queue"
          />

          <button type="submit" className="msx-button msx-button--primary">
            Search
          </button>
        </form>
      </header>

      {queueErrorMessage ? (
        <ErrorPanel
          message={queueErrorMessage}
          onRetry={() => {
            void loadQueue();
          }}
        />
      ) : null}

      <section className="table-card">
        <DataTable
          rows={result.items}
          columns={columns}
          keyExtractor={(row) => row.approvalRequestID}
          loading={queueLoading}
          onRowClick={(row) => {
            void openDetails(row);
          }}
          emptyMessage="No approval requests match the current search."
        />

        <footer className="pagination-bar">
          <span>
            Page {result.pageNumber || 1} of {Math.max(result.totalPages, 1)} ·{" "}
            {result.totalCount} records
          </span>

          <div>
            <button
              type="button"
              className="msx-button msx-button--ghost"
              disabled={pageNumber <= 1 || queueLoading}
              onClick={() => setPageNumber((value) => Math.max(1, value - 1))}
            >
              Previous
            </button>

            <button
              type="button"
              className="msx-button msx-button--ghost"
              disabled={pageNumber >= result.totalPages || queueLoading}
              onClick={() => setPageNumber((value) => value + 1)}
            >
              Next
            </button>
          </div>
        </footer>
      </section>

      {activeRequest ? (
        <aside className="details-panel" aria-label="Approval request details">
          <header className="details-panel__header">
            <div>
              <p>
                {humanizeCode(activeRequest.referenceType)} #
                {activeRequest.referenceID}
              </p>

              <h2>Approval request #{activeRequest.approvalRequestID}</h2>
            </div>

            <button
              type="button"
              className="icon-button"
              aria-label="Close details"
              onClick={() => setDetailsData(null)}
            >
              ×
            </button>
          </header>

          <dl className="details-grid">
            <div>
              <dt>Status</dt>

              <dd>
                <StatusBadge
                  label={statusLabels[activeRequest.status]}
                  tone={statusTones[activeRequest.status]}
                />
              </dd>
            </div>

            <div>
              <dt>Current step</dt>

              <dd>{activeRequest.currentStepNo}</dd>
            </div>

            <div>
              <dt>Requested by user</dt>

              <dd>#{activeRequest.requestedByUserID}</dd>
            </div>

            <div>
              <dt>Requested at</dt>

              <dd>{formatDateTime(activeRequest.requestedAt)}</dd>
            </div>
          </dl>

          <section className="timeline">
            <h3>Action history</h3>

            {activeRequest.actions.length === 0 ? (
              <p>No actions have been recorded.</p>
            ) : (
              activeRequest.actions.map((action) => (
                <article key={action.approvalActionID}>
                  <span className="timeline__dot" aria-hidden="true" />

                  <div>
                    <strong>{actionLabels[action.action]}</strong>

                    <span>
                      {action.actionByName} · {formatDateTime(action.actionAt)}
                    </span>

                    {action.note ? <p>{action.note}</p> : null}
                  </div>
                </article>
              ))
            )}
          </section>

          {canAct && canReceiveAction ? (
            <div className="details-panel__actions">
              <button
                type="button"
                className="msx-button msx-button--secondary"
                onClick={() => {
                  setActionError("");
                  setDialogAction(6);
                }}
              >
                Comment
              </button>

              <button
                type="button"
                className="msx-button msx-button--danger"
                onClick={() => {
                  setActionError("");
                  setDialogAction(3);
                }}
              >
                Reject
              </button>

              <button
                type="button"
                className="msx-button msx-button--primary"
                onClick={() => {
                  setActionError("");
                  setDialogAction(2);
                }}
              >
                Approve
              </button>
            </div>
          ) : null}
        </aside>
      ) : null}

      <ApprovalActionDialog
        request={activeRequest}
        action={dialogAction}
        busy={actionBusy}
        errorMessage={actionError}
        onClose={() => {
          setActionError("");
          setDialogAction(null);
        }}
        onSubmit={submitAction}
      />
    </main>
  );
}
