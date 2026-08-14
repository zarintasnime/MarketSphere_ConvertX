import { useCallback, useEffect, useMemo } from "react";
import {
  Bar,
  BarChart,
  CartesianGrid,
  LabelList,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";

import { getLeadToOrderFunnel } from "../api/analyticsApi";
import ChartCard from "../components/ChartCard";
import DataTable, { type DataTableColumn } from "../components/DataTable";
import ErrorPanel from "../components/ErrorPanel";
import FilterBar from "../components/FilterBar";
import { useApi } from "../hooks/useApi";
import { useDateRange } from "../hooks/useDateRange";
import type { FunnelPoint } from "../types/analytics.types";
import type { FunnelChartPoint } from "../types/dashboard.types";
import { mapFunnel } from "../utils/chartMappers";
import { formatNumber, formatPercent } from "../utils/formatters";

export default function LeadToOrderFunnelPage() {
  const { range, apiRange, setRange, resetRange } = useDateRange();

  const { data, isLoading, errorMessage, execute } = useApi<
    readonly FunnelPoint[]
  >([]);

  const loadFunnel = useCallback(
    () => execute(() => getLeadToOrderFunnel(apiRange)),
    [apiRange, execute],
  );

  useEffect(() => {
    void loadFunnel();
  }, [loadFunnel]);

  const rows = useMemo(() => mapFunnel(data ?? []), [data]);

  const columns = useMemo<readonly DataTableColumn<FunnelChartPoint>[]>(
    () => [
      {
        key: "stage",
        header: "Stage",
        render: (row) => <strong>{row.stage}</strong>,
      },
      {
        key: "count",
        header: "Count",
        render: (row) => formatNumber(row.count),
      },
      {
        key: "conversion",
        header: "Conversion from previous",
        render: (row) => formatPercent(row.conversionRate),
      },
    ],
    [],
  );

  return (
    <main className="management-page">
      <header className="page-heading">
        <div>
          <p className="page-heading__eyebrow">Commercial pipeline</p>

          <h1>Lead-to-Order Funnel</h1>

          <p>
            Track progression from lead creation to confirmed customer orders.
          </p>
        </div>
      </header>

      <FilterBar
        range={range}
        onRangeChange={setRange}
        onApply={() => {
          void loadFunnel();
        }}
        onReset={resetRange}
        busy={isLoading}
      />

      {errorMessage ? (
        <ErrorPanel
          message={errorMessage}
          onRetry={() => {
            void loadFunnel();
          }}
        />
      ) : null}

      <section className="dashboard-grid dashboard-grid--equal">
        <ChartCard
          title="Funnel volume"
          subtitle="Record count by commercial stage"
        >
          <div className="chart-height chart-height--tall">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart
                data={rows}
                margin={{
                  top: 26,
                  right: 20,
                  left: 6,
                  bottom: 4,
                }}
              >
                <CartesianGrid strokeDasharray="3 3" vertical={false} />

                <XAxis dataKey="stage" tickLine={false} axisLine={false} />

                <YAxis
                  allowDecimals={false}
                  tickLine={false}
                  axisLine={false}
                />

                <Tooltip formatter={(value) => formatNumber(Number(value))} />

                <Bar
                  dataKey="count"
                  fill="var(--msx-color-brand-600)"
                  radius={[8, 8, 0, 0]}
                >
                  <LabelList dataKey="count" position="top" />
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          </div>
        </ChartCard>

        <ChartCard
          title="Stage conversion"
          subtitle="Operational reading of the funnel"
        >
          <DataTable
            rows={rows}
            columns={columns}
            keyExtractor={(row) => row.stage}
            loading={isLoading}
            emptyMessage="No funnel records were returned for the selected period."
          />
        </ChartCard>
      </section>
    </main>
  );
}
