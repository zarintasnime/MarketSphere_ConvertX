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

import { getCampaignRoi } from "../api/analyticsApi";
import ChartCard from "../components/ChartCard";
import DataTable, { type DataTableColumn } from "../components/DataTable";
import ErrorPanel from "../components/ErrorPanel";
import FilterBar from "../components/FilterBar";
import KpiCard from "../components/KpiCard";
import { useApi } from "../hooks/useApi";
import { useDateRange } from "../hooks/useDateRange";
import type { AnalyticsFilter, CampaignRoi } from "../types/analytics.types";
import { mapCampaignRoi } from "../utils/chartMappers";
import { formatCurrency, formatPercent } from "../utils/formatters";

export default function CampaignRoiPage() {
  const { range, apiRange, setRange, resetRange } = useDateRange();

  const [campaignID, setCampaignID] = useState("");

  const { data, isLoading, errorMessage, execute } = useApi<
    readonly CampaignRoi[]
  >([]);

  const createFilter = useCallback(
    (): AnalyticsFilter => ({
      ...apiRange,
      campaignID: campaignID ? Number(campaignID) : null,
    }),
    [apiRange, campaignID],
  );

  const load = useCallback(
    () => execute(() => getCampaignRoi(createFilter())),
    [createFilter, execute],
  );

  useEffect(() => {
    void load();
  }, [load]);

  const rows = useMemo<readonly CampaignRoi[]>(() => data ?? [], [data]);

  const chartData = useMemo(() => mapCampaignRoi(rows), [rows]);

  const summary = useMemo(() => {
    const budget = rows.reduce((total, row) => total + row.budget, 0);

    const expense = rows.reduce((total, row) => total + row.expense, 0);

    const attributed = rows.reduce(
      (total, row) => total + row.attributedValue,
      0,
    );

    const delivered = rows.reduce(
      (total, row) => total + row.deliveredValue,
      0,
    );

    const roiPercent =
      expense > 0 ? ((delivered - expense) / expense) * 100 : 0;

    return {
      budget,
      expense,
      attributed,
      delivered,
      roiPercent,
    };
  }, [rows]);

  const columns = useMemo<readonly DataTableColumn<CampaignRoi>[]>(
    () => [
      {
        key: "campaign",
        header: "Campaign",
        render: (row) => (
          <div className="table-primary-cell">
            <strong>{row.campaignCode}</strong>

            <span>{row.campaignTitle}</span>
          </div>
        ),
      },
      {
        key: "budget",
        header: "Budget",
        render: (row) => formatCurrency(row.budget),
      },
      {
        key: "expense",
        header: "Expense",
        render: (row) => formatCurrency(row.expense),
      },
      {
        key: "attributed",
        header: "Attributed value",
        render: (row) => formatCurrency(row.attributedValue),
      },
      {
        key: "delivered",
        header: "Delivered value",
        render: (row) => formatCurrency(row.deliveredValue),
      },
      {
        key: "roi",
        header: "ROI",
        render: (row) => formatPercent(row.roiPercent),
      },
    ],
    [],
  );

  const reset = useCallback(() => {
    setCampaignID("");
    resetRange();
  }, [resetRange]);

  return (
    <main className="management-page">
      <header className="page-heading">
        <div>
          <p className="page-heading__eyebrow">Marketing performance</p>

          <h1>Campaign ROI</h1>

          <p>
            Compare campaign investment, attribution, delivered value, and
            return.
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
          <span>Campaign ID</span>

          <input
            type="number"
            min="1"
            value={campaignID}
            onChange={(event) => setCampaignID(event.target.value)}
            placeholder="All campaigns"
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
        <KpiCard label="Budget" value={formatCurrency(summary.budget)} />

        <KpiCard
          label="Expense"
          value={formatCurrency(summary.expense)}
          tone="warning"
        />

        <KpiCard
          label="Attributed value"
          value={formatCurrency(summary.attributed)}
          tone="info"
        />

        <KpiCard
          label="Delivered value"
          value={formatCurrency(summary.delivered)}
          tone="success"
        />

        <KpiCard
          label="Portfolio ROI"
          value={formatPercent(summary.roiPercent)}
          tone={summary.roiPercent >= 0 ? "success" : "danger"}
        />
      </section>

      <ChartCard
        title="Campaign value comparison"
        subtitle="Budget, expense, attribution, and delivered value"
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

              <XAxis dataKey="label" tickLine={false} axisLine={false} />

              <YAxis tickLine={false} axisLine={false} width={82} />

              <Tooltip formatter={(value) => formatCurrency(Number(value))} />

              <Bar
                dataKey="budget"
                name="Budget"
                fill="var(--msx-color-neutral-400)"
                radius={[6, 6, 0, 0]}
              />

              <Bar
                dataKey="expense"
                name="Expense"
                fill="var(--msx-color-warning-600)"
                radius={[6, 6, 0, 0]}
              />

              <Bar
                dataKey="attributed"
                name="Attributed"
                fill="var(--msx-color-info-600)"
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

        <div className="chart-legend" aria-label="Campaign chart legend">
          <span>
            <i className="legend-swatch legend-swatch--neutral" />
            Budget
          </span>

          <span>
            <i className="legend-swatch legend-swatch--warning" />
            Expense
          </span>

          <span>
            <i className="legend-swatch legend-swatch--info" />
            Attributed
          </span>

          <span>
            <i className="legend-swatch legend-swatch--success" />
            Delivered
          </span>
        </div>
      </ChartCard>

      <section className="table-card">
        <DataTable
          rows={rows}
          columns={columns}
          keyExtractor={(row) => row.campaignID}
          loading={isLoading}
          loadingMessage="Loading campaign analytics..."
          emptyMessage="No campaign analytics were returned for the selected period."
        />
      </section>
    </main>
  );
}
