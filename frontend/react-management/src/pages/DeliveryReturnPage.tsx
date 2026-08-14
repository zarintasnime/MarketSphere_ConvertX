import { useCallback, useEffect, useMemo, useState } from "react";
import {
  Bar,
  BarChart,
  CartesianGrid,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";

import { getDeliveryReturn } from "../api/analyticsApi";
import ChartCard from "../components/ChartCard";
import DataTable, { type DataTableColumn } from "../components/DataTable";
import ErrorPanel from "../components/ErrorPanel";
import FilterBar from "../components/FilterBar";
import KpiCard from "../components/KpiCard";
import { useApi } from "../hooks/useApi";
import { useDateRange } from "../hooks/useDateRange";
import type {
  AnalyticsFilter,
  DeliveryReturnPoint,
} from "../types/analytics.types";
import { mapDeliveryReturn } from "../utils/chartMappers";
import {
  formatDate,
  formatNumber,
  formatPercent,
  formatQuantity,
} from "../utils/formatters";

export default function DeliveryReturnPage() {
  const { range, apiRange, setRange, resetRange } = useDateRange();

  const [branchID, setBranchID] = useState("");
  const [regionID, setRegionID] = useState("");

  const { data, isLoading, errorMessage, execute } = useApi<
    readonly DeliveryReturnPoint[]
  >([]);

  const createFilter = useCallback(
    (): AnalyticsFilter => ({
      ...apiRange,
      branchID: branchID ? Number(branchID) : null,
      regionID: regionID ? Number(regionID) : null,
    }),
    [apiRange, branchID, regionID],
  );

  const load = useCallback(
    () => execute(() => getDeliveryReturn(createFilter())),
    [createFilter, execute],
  );

  useEffect(() => {
    void load();
  }, [load]);

  const rows = useMemo<readonly DeliveryReturnPoint[]>(
    () => data ?? [],
    [data],
  );

  const chartData = useMemo(() => mapDeliveryReturn(rows), [rows]);

  const totals = useMemo(
    () =>
      rows.reduce(
        (result, row) => ({
          planned: result.planned + row.plannedCount,

          delivered: result.delivered + row.deliveredCount,

          partial: result.partial + row.partialCount,

          failed: result.failed + row.failedCount,

          rescheduled: result.rescheduled + row.rescheduledCount,

          returns: result.returns + row.returnRequestCount,

          returnedQuantity: result.returnedQuantity + row.returnedQuantity,
        }),
        {
          planned: 0,
          delivered: 0,
          partial: 0,
          failed: 0,
          rescheduled: 0,
          returns: 0,
          returnedQuantity: 0,
        },
      ),
    [rows],
  );

  const deliveryRate =
    totals.planned > 0 ? (totals.delivered / totals.planned) * 100 : 0;

  const columns = useMemo<readonly DataTableColumn<DeliveryReturnPoint>[]>(
    () => [
      {
        key: "period",
        header: "Period",
        render: (row) => formatDate(row.period),
      },
      {
        key: "planned",
        header: "Planned",
        render: (row) => formatNumber(row.plannedCount),
      },
      {
        key: "delivered",
        header: "Delivered",
        render: (row) => formatNumber(row.deliveredCount),
      },
      {
        key: "partial",
        header: "Partial",
        render: (row) => formatNumber(row.partialCount),
      },
      {
        key: "failed",
        header: "Failed",
        render: (row) => formatNumber(row.failedCount),
      },
      {
        key: "rescheduled",
        header: "Rescheduled",
        render: (row) => formatNumber(row.rescheduledCount),
      },
      {
        key: "returns",
        header: "Return requests",
        render: (row) => formatNumber(row.returnRequestCount),
      },
      {
        key: "returnedQty",
        header: "Returned quantity",
        render: (row) => formatQuantity(row.returnedQuantity),
      },
    ],
    [],
  );

  const reset = useCallback(() => {
    setBranchID("");
    setRegionID("");
    resetRange();
  }, [resetRange]);

  return (
    <main className="management-page">
      <header className="page-heading">
        <div>
          <p className="page-heading__eyebrow">Fulfilment quality</p>

          <h1>Delivery and Return Analytics</h1>

          <p>
            Track delivery outcomes, rescheduling pressure, and customer return
            volume.
          </p>
        </div>
      </header>

      <FilterBar
        range={range}
        onRangeChange={setRange}
        onApply={() => {
          void load();
        }}
        onReset={reset}
        busy={isLoading}
      >
        <label>
          <span>Branch ID</span>

          <input
            type="number"
            min="1"
            value={branchID}
            onChange={(event) => setBranchID(event.target.value)}
            placeholder="All branches"
          />
        </label>

        <label>
          <span>Region ID</span>

          <input
            type="number"
            min="1"
            value={regionID}
            onChange={(event) => setRegionID(event.target.value)}
            placeholder="All regions"
          />
        </label>
      </FilterBar>

      {errorMessage ? (
        <ErrorPanel
          message={errorMessage}
          onRetry={() => {
            void load();
          }}
        />
      ) : null}

      <section className="kpi-grid">
        <KpiCard
          label="Planned deliveries"
          value={formatNumber(totals.planned)}
        />

        <KpiCard
          label="Delivered"
          value={formatNumber(totals.delivered)}
          tone="success"
        />

        <KpiCard
          label="Delivery rate"
          value={formatPercent(deliveryRate)}
          tone="info"
        />

        <KpiCard
          label="Failed"
          value={formatNumber(totals.failed)}
          tone="danger"
        />

        <KpiCard
          label="Return requests"
          value={formatNumber(totals.returns)}
          tone="warning"
        />
      </section>

      <ChartCard
        title="Delivery outcome trend"
        subtitle="Planned, delivered, partial, failed, and return counts"
      >
        <div className="chart-height chart-height--tall">
          <ResponsiveContainer width="100%" height="100%">
            <BarChart
              data={chartData}
              margin={{
                top: 12,
                right: 20,
                left: 8,
                bottom: 8,
              }}
            >
              <CartesianGrid strokeDasharray="3 3" vertical={false} />

              <XAxis dataKey="period" tickLine={false} axisLine={false} />

              <YAxis allowDecimals={false} tickLine={false} axisLine={false} />

              <Tooltip formatter={(value) => formatNumber(Number(value))} />

              <Bar
                dataKey="planned"
                name="Planned"
                fill="var(--msx-color-neutral-400)"
                radius={[5, 5, 0, 0]}
              />

              <Bar
                dataKey="delivered"
                name="Delivered"
                fill="var(--msx-color-success-600)"
                radius={[5, 5, 0, 0]}
              />

              <Bar
                dataKey="partial"
                name="Partial"
                fill="var(--msx-color-info-600)"
                radius={[5, 5, 0, 0]}
              />

              <Bar
                dataKey="failed"
                name="Failed"
                fill="var(--msx-color-danger-600)"
                radius={[5, 5, 0, 0]}
              />

              <Bar
                dataKey="returns"
                name="Returns"
                fill="var(--msx-color-warning-600)"
                radius={[5, 5, 0, 0]}
              />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </ChartCard>

      <section className="table-card">
        <DataTable
          rows={rows}
          columns={columns}
          keyExtractor={(row) => row.period}
          loading={isLoading}
          emptyMessage="No delivery or return analytics were returned."
        />
      </section>
    </main>
  );
}
