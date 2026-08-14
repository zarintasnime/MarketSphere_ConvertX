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

import { getEmployeeKpi } from "../api/analyticsApi";
import ChartCard from "../components/ChartCard";
import DataTable, { type DataTableColumn } from "../components/DataTable";
import ErrorPanel from "../components/ErrorPanel";
import FilterBar from "../components/FilterBar";
import KpiCard from "../components/KpiCard";
import StatusBadge from "../components/StatusBadge";
import { useApi } from "../hooks/useApi";
import { useDateRange } from "../hooks/useDateRange";
import type { AnalyticsFilter, EmployeeKpi } from "../types/analytics.types";
import type { StatusTone } from "../types/common.types";
import { mapEmployeeKpi } from "../utils/chartMappers";
import {
  formatCurrency,
  formatNumber,
  formatPercent,
} from "../utils/formatters";

function achievementTone(value: number): StatusTone {
  if (value >= 100) return "success";
  if (value >= 80) return "info";
  if (value >= 60) return "warning";
  return "danger";
}

export default function EmployeeKpiPage() {
  const { range, apiRange, setRange, resetRange } = useDateRange();
  const [employeeID, setEmployeeID] = useState("");
  const state = useApi<readonly EmployeeKpi[]>([]);
  const { execute } = state;

  const createFilter = useCallback(
    (): AnalyticsFilter => ({
      ...apiRange,
      employeeID: employeeID ? Number(employeeID) : null,
    }),
    [apiRange, employeeID],
  );

  const load = useCallback(
    () => execute(() => getEmployeeKpi(createFilter())),
    [createFilter, execute],
  );

  useEffect(() => {
    void load();
  }, [load]);

  const rows = useMemo(
    () =>
      [...(state.data ?? [])].sort(
        (left, right) => right.achievementPercent - left.achievementPercent,
      ),
    [state.data],
  );
  const chartData = useMemo(() => mapEmployeeKpi(rows).slice(0, 12), [rows]);
  const totalTarget = rows.reduce((total, row) => total + row.targetValue, 0);
  const totalActual = rows.reduce((total, row) => total + row.actualValue, 0);
  const totalReward = rows.reduce((total, row) => total + row.rewardAmount, 0);
  const averageAchievement =
    rows.length > 0
      ? rows.reduce((total, row) => total + row.achievementPercent, 0) /
        rows.length
      : 0;
  const topPerformer = rows[0] ?? null;

  const columns = useMemo<readonly DataTableColumn<EmployeeKpi>[]>(
    () => [
      {
        key: "employee",
        header: "Employee",
        render: (row) => (
          <div className="table-primary-cell">
            <strong>{row.employeeName}</strong>
            <span>{row.employeeCode}</span>
          </div>
        ),
      },
      {
        key: "target",
        header: "Target",
        render: (row) => formatCurrency(row.targetValue),
      },
      {
        key: "actual",
        header: "Actual",
        render: (row) => formatCurrency(row.actualValue),
      },
      {
        key: "achievement",
        header: "Achievement",
        render: (row) => (
          <StatusBadge
            label={formatPercent(row.achievementPercent)}
            tone={achievementTone(row.achievementPercent)}
          />
        ),
      },
      {
        key: "reward",
        header: "Reward",
        render: (row) => formatCurrency(row.rewardAmount),
      },
    ],
    [],
  );

  const reset = () => {
    setEmployeeID("");
    resetRange();
  };

  return (
    <main className="management-page">
      <header className="page-heading">
        <div>
          <p className="page-heading__eyebrow">People performance</p>
          <h1>Employee KPI</h1>
          <p>
            Review target achievement and reward outcomes for the selected
            period.
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
          <span>Employee ID</span>
          <input
            type="number"
            min="1"
            value={employeeID}
            onChange={(event) => setEmployeeID(event.target.value)}
            placeholder="All employees"
          />
        </label>
      </FilterBar>

      {state.errorMessage ? (
        <ErrorPanel message={state.errorMessage} onRetry={() => void load()} />
      ) : null}

      <section className="kpi-grid">
        <KpiCard label="Employees" value={formatNumber(rows.length)} />
        <KpiCard label="Total target" value={formatCurrency(totalTarget)} />
        <KpiCard
          label="Total actual"
          value={formatCurrency(totalActual)}
          tone="success"
        />
        <KpiCard
          label="Average achievement"
          value={formatPercent(averageAchievement)}
          tone={achievementTone(averageAchievement)}
        />
        <KpiCard
          label="Total reward"
          value={formatCurrency(totalReward)}
          tone="info"
        />
      </section>

      {topPerformer ? (
        <section className="analytics-highlight">
          <div>
            <span>Top performer</span>
            <strong>{topPerformer.employeeName}</strong>
            <small>{topPerformer.employeeCode}</small>
          </div>
          <div>
            <span>Achievement</span>
            <strong>{formatPercent(topPerformer.achievementPercent)}</strong>
          </div>
          <div>
            <span>Reward</span>
            <strong>{formatCurrency(topPerformer.rewardAmount)}</strong>
          </div>
        </section>
      ) : null}

      <ChartCard
        title="Top employee performance"
        subtitle="Target and actual value for the highest achievement rates"
      >
        <div className="chart-height chart-height--tall">
          <ResponsiveContainer width="100%" height="100%">
            <BarChart
              data={chartData}
              margin={{ top: 12, right: 20, left: 8, bottom: 40 }}
            >
              <CartesianGrid strokeDasharray="3 3" vertical={false} />
              <XAxis
                dataKey="employee"
                tickLine={false}
                axisLine={false}
                angle={-25}
                textAnchor="end"
                height={70}
              />
              <YAxis tickLine={false} axisLine={false} width={82} />
              <Tooltip formatter={(value) => formatCurrency(Number(value))} />
              <Bar
                dataKey="target"
                name="Target"
                fill="var(--msx-color-neutral-400)"
                radius={[6, 6, 0, 0]}
              />
              <Bar
                dataKey="actual"
                name="Actual"
                fill="var(--msx-color-brand-600)"
                radius={[6, 6, 0, 0]}
              />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </ChartCard>

      <section className="table-card">
        <DataTable
          rows={rows}
          columns={columns}
          keyExtractor={(row) => row.employeeID}
          loading={state.isLoading}
          emptyMessage="No employee KPI analytics were returned."
        />
      </section>
    </main>
  );
}

