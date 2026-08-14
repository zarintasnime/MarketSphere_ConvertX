import { useCallback, useEffect, useMemo, useState } from "react";
import {
  CartesianGrid,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";

import { getSellInSellOut } from "../api/analyticsApi";
import ChartCard from "../components/ChartCard";
import DataTable, { type DataTableColumn } from "../components/DataTable";
import ErrorPanel from "../components/ErrorPanel";
import FilterBar from "../components/FilterBar";
import KpiCard from "../components/KpiCard";
import { useApi } from "../hooks/useApi";
import { useDateRange } from "../hooks/useDateRange";
import type {
  AnalyticsFilter,
  SellInSellOutPoint,
} from "../types/analytics.types";
import { mapSellInSellOut } from "../utils/chartMappers";
import {
  formatCurrency,
  formatDate,
  formatQuantity,
} from "../utils/formatters";

export default function SellInSellOutPage() {
  const { range, apiRange, setRange, resetRange } = useDateRange();

  const [regionID, setRegionID] = useState("");

  const { data, isLoading, errorMessage, execute } = useApi<
    readonly SellInSellOutPoint[]
  >([]);

  const createFilter = useCallback(
    (): AnalyticsFilter => ({
      ...apiRange,
      regionID: regionID ? Number(regionID) : null,
    }),
    [apiRange, regionID],
  );

  const load = useCallback(
    () => execute(() => getSellInSellOut(createFilter())),
    [createFilter, execute],
  );

  useEffect(() => {
    void load();
  }, [load]);

  const rows = useMemo<readonly SellInSellOutPoint[]>(() => data ?? [], [data]);

  const chartData = useMemo(() => mapSellInSellOut(rows), [rows]);

  const totals = useMemo(
    () =>
      rows.reduce(
        (result, row) => ({
          sellInQuantity: result.sellInQuantity + row.sellInQuantity,

          sellOutQuantity: result.sellOutQuantity + row.sellOutQuantity,

          sellInValue: result.sellInValue + row.sellInValue,

          sellOutValue: result.sellOutValue + row.sellOutValue,
        }),
        {
          sellInQuantity: 0,
          sellOutQuantity: 0,
          sellInValue: 0,
          sellOutValue: 0,
        },
      ),
    [rows],
  );

  const columns = useMemo<readonly DataTableColumn<SellInSellOutPoint>[]>(
    () => [
      {
        key: "period",
        header: "Period",
        render: (row) => formatDate(row.period),
      },
      {
        key: "sellInQty",
        header: "Sell-in quantity",
        render: (row) => formatQuantity(row.sellInQuantity),
      },
      {
        key: "sellOutQty",
        header: "Sell-out quantity",
        render: (row) => formatQuantity(row.sellOutQuantity),
      },
      {
        key: "sellInValue",
        header: "Sell-in value",
        render: (row) => formatCurrency(row.sellInValue),
      },
      {
        key: "sellOutValue",
        header: "Sell-out value",
        render: (row) => formatCurrency(row.sellOutValue),
      },
    ],
    [],
  );

  const reset = useCallback(() => {
    setRegionID("");
    resetRange();
  }, [resetRange]);

  const valueGap = totals.sellInValue - totals.sellOutValue;

  return (
    <main className="management-page">
      <header className="page-heading">
        <div>
          <p className="page-heading__eyebrow">Distribution visibility</p>

          <h1>Sell-In vs Sell-Out</h1>

          <p>
            Compare delivered movement into partners with verified downstream
            sell-out.
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
          label="Sell-in quantity"
          value={formatQuantity(totals.sellInQuantity)}
        />

        <KpiCard
          label="Sell-out quantity"
          value={formatQuantity(totals.sellOutQuantity)}
          tone="info"
        />

        <KpiCard
          label="Sell-in value"
          value={formatCurrency(totals.sellInValue)}
        />

        <KpiCard
          label="Sell-out value"
          value={formatCurrency(totals.sellOutValue)}
          tone="success"
        />

        <KpiCard
          label="Value gap"
          value={formatCurrency(valueGap)}
          tone={valueGap > 0 ? "warning" : "success"}
        />
      </section>

      <ChartCard
        title="Sell-in and sell-out value trend"
        subtitle="Commercial value by reporting period"
      >
        <div className="chart-height chart-height--tall">
          <ResponsiveContainer width="100%" height="100%">
            <LineChart
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

              <YAxis tickLine={false} axisLine={false} width={82} />

              <Tooltip formatter={(value) => formatCurrency(Number(value))} />

              <Line
                type="monotone"
                dataKey="sellInValue"
                name="Sell-in value"
                stroke="var(--msx-color-brand-600)"
                strokeWidth={3}
                dot={false}
              />

              <Line
                type="monotone"
                dataKey="sellOutValue"
                name="Sell-out value"
                stroke="var(--msx-color-success-600)"
                strokeWidth={3}
                dot={false}
              />
            </LineChart>
          </ResponsiveContainer>
        </div>

        <div className="chart-legend">
          <span>
            <i className="legend-swatch legend-swatch--brand" />
            Sell-in value
          </span>

          <span>
            <i className="legend-swatch legend-swatch--success" />
            Sell-out value
          </span>
        </div>
      </ChartCard>

      <section className="table-card">
        <DataTable
          rows={rows}
          columns={columns}
          keyExtractor={(row) => row.period}
          loading={isLoading}
          emptyMessage="No sell-in or sell-out analytics were returned."
        />
      </section>
    </main>
  );
}
