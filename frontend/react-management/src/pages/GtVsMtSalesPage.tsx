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

import { getChannelSales } from "../api/analyticsApi";
import ChartCard from "../components/ChartCard";
import DataTable, { type DataTableColumn } from "../components/DataTable";
import ErrorPanel from "../components/ErrorPanel";
import FilterBar from "../components/FilterBar";
import KpiCard from "../components/KpiCard";
import { useApi } from "../hooks/useApi";
import { useDateRange } from "../hooks/useDateRange";
import type { AnalyticsFilter, ChannelSales } from "../types/analytics.types";
import { mapChannelSales, salesChannelLabels } from "../utils/chartMappers";
import { formatCurrency, formatNumber } from "../utils/formatters";

export default function GtVsMtSalesPage() {
  const { range, apiRange, setRange, resetRange } = useDateRange();

  const [branchID, setBranchID] = useState("");
  const [regionID, setRegionID] = useState("");

  const { data, isLoading, errorMessage, execute } = useApi<
    readonly ChannelSales[]
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
    () => execute(() => getChannelSales(createFilter())),
    [createFilter, execute],
  );

  useEffect(() => {
    void load();
  }, [load]);

  const rows = useMemo<readonly ChannelSales[]>(() => data ?? [], [data]);

  const chartData = useMemo(() => mapChannelSales(rows), [rows]);

  const gt = useMemo(() => rows.find((row) => row.channel === 1), [rows]);

  const mt = useMemo(() => rows.find((row) => row.channel === 2), [rows]);

  const totalOrders = useMemo(
    () => rows.reduce((total, row) => total + row.orderCount, 0),
    [rows],
  );

  const totalDelivered = useMemo(
    () => rows.reduce((total, row) => total + row.deliveredValue, 0),
    [rows],
  );

  const columns = useMemo<readonly DataTableColumn<ChannelSales>[]>(
    () => [
      {
        key: "channel",
        header: "Channel",
        render: (row) => (
          <strong>
            {salesChannelLabels[row.channel] ?? `Channel ${row.channel}`}
          </strong>
        ),
      },
      {
        key: "orders",
        header: "Orders",
        render: (row) => formatNumber(row.orderCount),
      },
      {
        key: "gross",
        header: "Gross amount",
        render: (row) => formatCurrency(row.grossAmount),
      },
      {
        key: "net",
        header: "Net amount",
        render: (row) => formatCurrency(row.netAmount),
      },
      {
        key: "delivered",
        header: "Delivered value",
        render: (row) => formatCurrency(row.deliveredValue),
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
          <p className="page-heading__eyebrow">Channel performance</p>

          <h1>GT vs MT Sales</h1>

          <p>
            Compare General Trade and Modern Trade with the wider channel
            portfolio.
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
          label="GT net sales"
          value={formatCurrency(gt?.netAmount ?? 0)}
          tone="brand"
        />

        <KpiCard
          label="MT net sales"
          value={formatCurrency(mt?.netAmount ?? 0)}
          tone="info"
        />

        <KpiCard
          label="Total delivered value"
          value={formatCurrency(totalDelivered)}
          tone="success"
        />

        <KpiCard label="Total orders" value={formatNumber(totalOrders)} />
      </section>

      <section className="dashboard-grid dashboard-grid--equal">
        <ChartCard
          title="Channel sales comparison"
          subtitle="Gross, net, and delivered value"
        >
          <div className="chart-height chart-height--tall">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart
                data={chartData}
                margin={{
                  top: 12,
                  right: 20,
                  left: 8,
                  bottom: 12,
                }}
              >
                <CartesianGrid strokeDasharray="3 3" vertical={false} />

                <XAxis dataKey="channel" tickLine={false} axisLine={false} />

                <YAxis tickLine={false} axisLine={false} width={82} />

                <Tooltip formatter={(value) => formatCurrency(Number(value))} />

                <Bar
                  dataKey="gross"
                  name="Gross"
                  fill="var(--msx-color-neutral-400)"
                  radius={[6, 6, 0, 0]}
                />

                <Bar
                  dataKey="net"
                  name="Net"
                  fill="var(--msx-color-brand-600)"
                  radius={[6, 6, 0, 0]}
                />

                <Bar
                  dataKey="delivered"
                  name="Delivered"
                  fill="var(--msx-color-success-600)"
                  radius={[6, 6, 0, 0]}
                />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </ChartCard>

        <ChartCard
          title="Channel detail"
          subtitle="Order volume and commercial value"
        >
          <DataTable
            rows={rows}
            columns={columns}
            keyExtractor={(row) => row.channel}
            loading={isLoading}
            emptyMessage="No channel sales records were returned."
          />
        </ChartCard>
      </section>
    </main>
  );
}
