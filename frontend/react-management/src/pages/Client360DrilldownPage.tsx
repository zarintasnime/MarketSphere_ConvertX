import { useCallback, useMemo, useState } from "react";

import { getClient360 } from "../api/analyticsApi";
import DataTable, { type DataTableColumn } from "../components/DataTable";
import EmptyState from "../components/EmptyState";
import ErrorPanel from "../components/ErrorPanel";
import FilterBar from "../components/FilterBar";
import KpiCard from "../components/KpiCard";
import StatusBadge from "../components/StatusBadge";
import { useApi } from "../hooks/useApi";
import { useDateRange } from "../hooks/useDateRange";
import type {
  Client360,
  Client360Complaint,
  Client360Order,
  Client360Payment,
} from "../types/analytics.types";
import type { StatusTone } from "../types/common.types";
import { salesChannelLabels } from "../utils/chartMappers";
import {
  formatCurrency,
  formatDate,
  formatDateTime,
  formatEnumLabel,
  formatNumber,
  humanizeCode,
} from "../utils/formatters";

const clientTypeLabels: Readonly<Record<number, string>> = {
  1: "Outlet",
  2: "Dealer",
  3: "Distributor",
  4: "Modern Trade",
  5: "Business Partner",
};

const lifecycleLabels: Readonly<Record<number, string>> = {
  1: "Prospect",
  2: "Active",
  3: "Inactive",
  4: "Churned",
  5: "Reactivation in progress",
};

const riskLabels: Readonly<Record<number, string>> = {
  1: "Normal",
  2: "Watch",
  3: "High risk",
  4: "Blocked",
};

const orderStatusLabels: Readonly<Record<number, string>> = {
  1: "Draft",
  2: "Submitted",
  3: "Under review",
  4: "Approved",
  5: "Stock allocated",
  6: "Invoiced",
  7: "Ready for dispatch",
  8: "Partially delivered",
  9: "Delivered",
  10: "Returned",
  11: "Closed",
  12: "Rejected",
  13: "Cancelled",
};

const paymentMethodLabels: Readonly<Record<number, string>> = {
  1: "Cash",
  2: "Bank transfer",
  3: "Cheque",
  4: "Mobile financial service",
  5: "Other",
};

const paymentStatusLabels: Readonly<Record<number, string>> = {
  1: "Pending",
  2: "Confirmed",
  3: "Rejected",
  4: "Reversed",
};

const complaintPriorityLabels: Readonly<Record<number, string>> = {
  1: "Low",
  2: "Normal",
  3: "High",
  4: "Critical",
};

const complaintStatusLabels: Readonly<Record<number, string>> = {
  1: "Open",
  2: "Assigned",
  3: "In progress",
  4: "Waiting for customer",
  5: "Resolved",
  6: "Closed",
  7: "Rejected",
};

function orderTone(status: number): StatusTone {
  if ([9, 11].includes(status)) return "success";
  if ([12, 13].includes(status)) return "danger";
  if ([8, 10].includes(status)) return "warning";
  return "info";
}

function paymentTone(status: number): StatusTone {
  if (status === 2) return "success";
  if ([3, 4].includes(status)) return "danger";
  return "warning";
}

function complaintTone(status: number): StatusTone {
  if ([5, 6].includes(status)) return "success";
  if (status === 7) return "danger";
  return "warning";
}

export default function Client360DrilldownPage() {
  const { range, apiRange, setRange, resetRange } = useDateRange();
  const [clientID, setClientID] = useState("");
  const state = useApi<Client360>();
  const { execute } = state;

  const load = useCallback(() => {
    const parsedClientID = Number(clientID);
    if (!Number.isInteger(parsedClientID) || parsedClientID <= 0) {
      return Promise.resolve(null);
    }
    return execute(() => getClient360(parsedClientID, apiRange));
  }, [apiRange, clientID, execute]);

  const orderColumns = useMemo<readonly DataTableColumn<Client360Order>[]>(
    () => [
      {
        key: "orderNo",
        header: "Order",
        render: (row) => <strong>{row.orderNo}</strong>,
      },
      {
        key: "date",
        header: "Order date",
        render: (row) => formatDate(row.orderDate),
      },
      {
        key: "channel",
        header: "Channel",
        render: (row) =>
          salesChannelLabels[row.channel] ?? `Channel ${row.channel}`,
      },
      {
        key: "value",
        header: "Net amount",
        render: (row) => formatCurrency(row.netAmount),
      },
      {
        key: "status",
        header: "Status",
        render: (row) => (
          <StatusBadge
            label={formatEnumLabel(row.status, orderStatusLabels, "Order")}
            tone={orderTone(row.status)}
          />
        ),
      },
    ],
    [],
  );

  const paymentColumns = useMemo<readonly DataTableColumn<Client360Payment>[]>(
    () => [
      {
        key: "paymentNo",
        header: "Payment",
        render: (row) => <strong>{row.paymentNo}</strong>,
      },
      {
        key: "date",
        header: "Payment date",
        render: (row) => formatDate(row.paymentDate),
      },
      {
        key: "method",
        header: "Method",
        render: (row) =>
          formatEnumLabel(row.paymentMethod, paymentMethodLabels, "Method"),
      },
      {
        key: "amount",
        header: "Amount",
        render: (row) => formatCurrency(row.amount),
      },
      {
        key: "status",
        header: "Status",
        render: (row) => (
          <StatusBadge
            label={formatEnumLabel(row.status, paymentStatusLabels, "Payment")}
            tone={paymentTone(row.status)}
          />
        ),
      },
    ],
    [],
  );

  const complaintColumns = useMemo<
    readonly DataTableColumn<Client360Complaint>[]
  >(
    () => [
      {
        key: "complaintNo",
        header: "Complaint",
        render: (row) => <strong>{row.complaintNo}</strong>,
      },
      {
        key: "openedAt",
        header: "Opened",
        render: (row) => formatDate(row.openedAt),
      },
      { key: "subject", header: "Subject", render: (row) => row.subject },
      {
        key: "priority",
        header: "Priority",
        render: (row) =>
          formatEnumLabel(row.priority, complaintPriorityLabels, "Priority"),
      },
      {
        key: "status",
        header: "Status",
        render: (row) => (
          <StatusBadge
            label={formatEnumLabel(
              row.status,
              complaintStatusLabels,
              "Complaint",
            )}
            tone={complaintTone(row.status)}
          />
        ),
      },
    ],
    [],
  );

  const reset = () => {
    setClientID("");
    state.reset();
    resetRange();
  };

  return (
    <main className="management-page">
      <header className="page-heading">
        <div>
          <p className="page-heading__eyebrow">Management drill-down</p>
          <h1>Client 360</h1>
          <p>
            Review commercial activity, financial exposure, complaints, and the
            client timeline.
          </p>
        </div>
      </header>

      <FilterBar
        range={range}
        onRangeChange={setRange}
        onApply={() => void load()}
        onReset={reset}
        busy={state.isLoading}
      >
        <label>
          <span>Client ID</span>
          <input
            type="number"
            min="1"
            value={clientID}
            onChange={(event) => setClientID(event.target.value)}
            placeholder="Required"
            required
          />
        </label>
      </FilterBar>

      {!clientID && !state.data ? (
        <EmptyState
          title="Enter a client ID"
          message="Select a date range, enter the client ID, and apply the filters."
          icon="search"
        />
      ) : null}

      {state.errorMessage ? (
        <ErrorPanel message={state.errorMessage} onRetry={() => void load()} />
      ) : null}

      {state.data ? (
        <>
          <section className="client-identity-card">
            <div>
              <p className="page-heading__eyebrow">
                {state.data.header.clientCode}
              </p>
              <h2>{state.data.header.clientName}</h2>
              <p>{state.data.header.address}</p>
              <div className="client-identity-card__badges">
                <StatusBadge
                  label={formatEnumLabel(
                    state.data.header.lifecycleStatus,
                    lifecycleLabels,
                    "Lifecycle",
                  )}
                  tone={
                    state.data.header.lifecycleStatus === 2
                      ? "success"
                      : "warning"
                  }
                />
                <StatusBadge
                  label={formatEnumLabel(
                    state.data.header.riskStatus,
                    riskLabels,
                    "Risk",
                  )}
                  tone={
                    state.data.header.riskStatus >= 3
                      ? "danger"
                      : state.data.header.riskStatus === 2
                        ? "warning"
                        : "success"
                  }
                />
                {state.data.header.isCreditBlocked ? (
                  <StatusBadge label="Credit blocked" tone="danger" />
                ) : null}
              </div>
            </div>
            <dl className="client-contact-grid">
              <div>
                <dt>Client type</dt>
                <dd>
                  {formatEnumLabel(
                    state.data.header.clientType,
                    clientTypeLabels,
                    "Client",
                  )}
                </dd>
              </div>
              <div>
                <dt>Channel</dt>
                <dd>
                  {salesChannelLabels[state.data.header.channel] ??
                    `Channel ${state.data.header.channel}`}
                </dd>
              </div>
              <div>
                <dt>Phone</dt>
                <dd>{state.data.header.phone ?? "—"}</dd>
              </div>
              <div>
                <dt>Email</dt>
                <dd>{state.data.header.email ?? "—"}</dd>
              </div>
            </dl>
          </section>

          <section className="kpi-grid">
            <KpiCard
              label="Orders"
              value={formatNumber(state.data.header.orderCount)}
            />
            <KpiCard
              label="Order value"
              value={formatCurrency(state.data.header.orderValue)}
            />
            <KpiCard
              label="Paid amount"
              value={formatCurrency(state.data.header.paidAmount)}
              tone="success"
            />
            <KpiCard
              label="Current due"
              value={formatCurrency(state.data.header.currentDue)}
              tone={state.data.header.currentDue > 0 ? "warning" : "success"}
            />
            <KpiCard
              label="Credit limit"
              value={formatCurrency(state.data.header.creditLimit)}
              tone="info"
            />
            <KpiCard
              label="Open complaints"
              value={formatNumber(state.data.header.openComplaintCount)}
              tone={
                state.data.header.openComplaintCount > 0 ? "danger" : "success"
              }
            />
          </section>

          <section className="analytics-section-grid">
            <article className="table-card">
              <header className="section-card-heading">
                <h2>Recent orders</h2>
              </header>
              <DataTable
                rows={state.data.recentOrders}
                columns={orderColumns}
                keyExtractor={(row) => row.orderID}
                emptyMessage="No recent orders were returned."
              />
            </article>
            <article className="table-card">
              <header className="section-card-heading">
                <h2>Recent payments</h2>
              </header>
              <DataTable
                rows={state.data.recentPayments}
                columns={paymentColumns}
                keyExtractor={(row) => row.paymentID}
                emptyMessage="No recent payments were returned."
              />
            </article>
          </section>

          <section className="table-card">
            <header className="section-card-heading">
              <h2>Recent complaints</h2>
            </header>
            <DataTable
              rows={state.data.recentComplaints}
              columns={complaintColumns}
              keyExtractor={(row) => row.complaintID}
              emptyMessage="No recent complaints were returned."
            />
          </section>

          <section className="timeline-card">
            <header className="section-card-heading">
              <h2>Client timeline</h2>
            </header>
            {state.data.timeline.length > 0 ? (
              <ol className="timeline-list">
                {state.data.timeline.map((item, index) => (
                  <li
                    key={`${item.occurredAt}-${item.type}-${item.referenceID ?? index}`}
                  >
                    <span
                      className="timeline-list__marker"
                      aria-hidden="true"
                    />
                    <div className="timeline-list__content">
                      <div>
                        <strong>{item.title}</strong>
                        <span>{humanizeCode(item.type)}</span>
                      </div>
                      <time>{formatDateTime(item.occurredAt)}</time>
                      <p>
                        {item.status
                          ? `Status: ${humanizeCode(item.status)}`
                          : "Status not provided"}
                        {item.amount !== null
                          ? ` · Amount: ${formatCurrency(item.amount)}`
                          : ""}
                      </p>
                    </div>
                  </li>
                ))}
              </ol>
            ) : (
              <EmptyState
                title="No timeline records"
                message="No timeline activity was returned for the selected period."
              />
            )}
          </section>
        </>
      ) : null}
    </main>
  );
}

