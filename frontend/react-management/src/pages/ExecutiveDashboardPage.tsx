import { useCallback, useEffect, useMemo } from "react";
import {
  CartesianGrid,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";

import { getExecutiveDashboard } from "../api/analyticsApi";
import ChartCard from "../components/ChartCard";
import ErrorPanel from "../components/ErrorPanel";
import FilterBar from "../components/FilterBar";
import KpiCard from "../components/KpiCard";
import LoadingPanel from "../components/LoadingPanel";
import { useApi } from "../hooks/useApi";
import { useDateRange } from "../hooks/useDateRange";
import type { ExecutiveDashboard } from "../types/analytics.types";
import { mapFunnel, mapSalesTrend } from "../utils/chartMappers";
import { formatCurrency, formatNumber } from "../utils/formatters";

function formatKpiValue(value: number, unit: string | null): string {
  if (unit?.toUpperCase() === "BDT") {
    return formatCurrency(value, "BDT");
  }

  return formatNumber(value, value % 1 === 0 ? 0 : 2);
}

export default function ExecutiveDashboardPage() {
  const { range, apiRange, setRange, resetRange } = useDateRange();

  const { data, isLoading, errorMessage, execute } =
    useApi<ExecutiveDashboard>();

  const loadDashboard = useCallback(
    () => execute(() => getExecutiveDashboard(apiRange)),
    [apiRange, execute],
  );

  useEffect(() => {
    void loadDashboard();
  }, [loadDashboard]);

  const salesTrend = useMemo(
    () => mapSalesTrend(data?.salesTrend ?? []),
    [data?.salesTrend],
  );

  const funnel = useMemo(
    () => mapFunnel(data?.leadToOrderFunnel ?? []),
    [data?.leadToOrderFunnel],
  );

  return (
    <main className="management-page">
      <header className="page-heading">
        <div>
          <p className="page-heading__eyebrow">Executive overview</p>

          <h1>Executive Dashboard</h1>

          <p>
            Review commercial performance, operational pressure, and management
            actions.
          </p>
        </div>
      </header>

      <FilterBar
        range={range}
        onRangeChange={setRange}
        onApply={() => {
          void loadDashboard();
        }}
        onReset={resetRange}
        busy={isLoading}
      />

      {errorMessage ? (
        <ErrorPanel
          message={errorMessage}
          onRetry={() => {
            void loadDashboard();
          }}
        />
      ) : null}

      {isLoading && !data ? (
        <LoadingPanel message="Loading executive dashboard..." />
      ) : null}

      {data ? (
        <>
          <section className="kpi-grid">
            {data.kpis.map((kpi) => (
              <KpiCard
                key={kpi.code}
                label={kpi.label}
                value={formatKpiValue(kpi.value, kpi.unit)}
                trendValue={
                  kpi.changePercent === null
                    ? undefined
                    : `${Math.abs(kpi.changePercent).toFixed(1)}%`
                }
                trendDirection={
                  kpi.changePercent === null || kpi.changePercent === 0
                    ? "neutral"
                    : kpi.changePercent > 0
                      ? "up"
                      : "down"
                }
              />
            ))}
          </section>

          <section className="attention-grid">
            <article>
              <span>Pending approvals</span>

              <strong>{formatNumber(data.pendingApprovals)}</strong>
            </article>

            <article>
              <span>Overdue CRM tasks</span>

              <strong>{formatNumber(data.overdueTasks)}</strong>
            </article>

            <article>
              <span>Near-expiry batches</span>

              <strong>{formatNumber(data.nearExpiryBatches)}</strong>
            </article>
          </section>

          <section className="dashboard-grid">
            <ChartCard
              title="Sales trend"
              subtitle="Order net value for the selected period"
              className="dashboard-grid__wide"
            >
              <div className="chart-height">
                <ResponsiveContainer width="100%" height="100%">
                  <LineChart
                    data={salesTrend}
                    margin={{
                      top: 12,
                      right: 20,
                      left: 6,
                      bottom: 4,
                    }}
                  >
                    <CartesianGrid strokeDasharray="3 3" vertical={false} />

                    <XAxis dataKey="label" tickLine={false} axisLine={false} />

                    <YAxis tickLine={false} axisLine={false} width={72} />

                    <Tooltip
                      formatter={(value) =>
                        formatCurrency(Number(value), "BDT")
                      }
                    />

                    <Line
                      type="monotone"
                      dataKey="value"
                      stroke="var(--msx-color-brand-600)"
                      strokeWidth={3}
                      dot={false}
                      activeDot={{ r: 5 }}
                    />
                  </LineChart>
                </ResponsiveContainer>
              </div>
            </ChartCard>

            <ChartCard
              title="Lead-to-order funnel"
              subtitle="Stage count and step conversion"
            >
              <div className="funnel-summary">
                {funnel.map((point) => (
                  <article key={point.stage}>
                    <div>
                      <span>{point.stage}</span>

                      <strong>{formatNumber(point.count)}</strong>
                    </div>

                    <small>
                      {point.conversionRate === null
                        ? "Entry stage"
                        : `${point.conversionRate.toFixed(
                            1,
                          )}% from previous stage`}
                    </small>
                  </article>
                ))}
              </div>
            </ChartCard>
          </section>
        </>
      ) : null}
    </main>
  );
}
