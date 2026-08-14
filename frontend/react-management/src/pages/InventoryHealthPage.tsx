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

import { getInventoryHealth } from "../api/analyticsApi";
import ChartCard from "../components/ChartCard";
import DataTable, { type DataTableColumn } from "../components/DataTable";
import ErrorPanel from "../components/ErrorPanel";
import FilterBar from "../components/FilterBar";
import KpiCard from "../components/KpiCard";
import StatusBadge from "../components/StatusBadge";
import { useApi } from "../hooks/useApi";
import { useDateRange } from "../hooks/useDateRange";
import type {
  AnalyticsFilter,
  InventoryHealth,
  InventoryHealthItem,
} from "../types/analytics.types";
import type { StatusTone } from "../types/common.types";
import {
  formatDate,
  formatEnumLabel,
  formatNumber,
  formatQuantity,
} from "../utils/formatters";

const batchStatusLabels: Readonly<Record<number, string>> = {
  1: "Available",
  2: "Quarantine",
  3: "Expired",
  4: "Blocked",
  5: "Depleted",
};

function itemStatus(row: InventoryHealthItem): {
  label: string;
  tone: StatusTone;
} {
  if (row.batchStatus === 3) return { label: "Expired", tone: "danger" };
  if (row.batchStatus === 2) return { label: "Quarantine", tone: "warning" };
  if (row.batchStatus === 4) return { label: "Blocked", tone: "danger" };
  if (row.isLowStock) return { label: "Low stock", tone: "warning" };
  if (row.isNearExpiry) return { label: "Near expiry", tone: "warning" };
  return {
    label: formatEnumLabel(row.batchStatus, batchStatusLabels, "Batch"),
    tone: "success",
  };
}

export default function InventoryHealthPage() {
  const { range, apiRange, setRange, resetRange } = useDateRange();
  const [branchID, setBranchID] = useState("");
  const [regionID, setRegionID] = useState("");
  const state = useApi<InventoryHealth>();
  const { execute } = state;

  const createFilter = useCallback(
    (): AnalyticsFilter => ({
      ...apiRange,
      branchID: branchID ? Number(branchID) : null,
      regionID: regionID ? Number(regionID) : null,
    }),
    [apiRange, branchID, regionID],
  );

  const load = useCallback(
    () => execute(() => getInventoryHealth(createFilter())),
    [createFilter, execute],
  );

  useEffect(() => {
    void load();
  }, [load]);

  const chartData = useMemo(
    () =>
      state.data
        ? [
            { metric: "On hand", value: state.data.onHandQuantity },
            { metric: "Available", value: state.data.availableQuantity },
            { metric: "Reserved", value: state.data.reservedQuantity },
            { metric: "Quarantine", value: state.data.quarantineQuantity },
            { metric: "Damaged", value: state.data.damagedQuantity },
          ]
        : [],
    [state.data],
  );

  const columns = useMemo<readonly DataTableColumn<InventoryHealthItem>[]>(
    () => [
      {
        key: "warehouse",
        header: "Warehouse",
        render: (row) => <strong>{row.warehouseName}</strong>,
      },
      {
        key: "sku",
        header: "SKU",
        render: (row) => (
          <div className="table-primary-cell">
            <strong>{row.skuCode}</strong>
            <span>{row.skuName}</span>
          </div>
        ),
      },
      { key: "batch", header: "Batch", render: (row) => row.batchNo ?? "—" },
      {
        key: "expiry",
        header: "Expiry",
        render: (row) => formatDate(row.expiryDate),
      },
      {
        key: "onHand",
        header: "On hand",
        render: (row) => formatQuantity(row.onHandQuantity),
      },
      {
        key: "reserved",
        header: "Reserved",
        render: (row) => formatQuantity(row.reservedQuantity),
      },
      {
        key: "available",
        header: "Available",
        render: (row) => formatQuantity(row.availableQuantity),
      },
      {
        key: "status",
        header: "Health",
        render: (row) => {
          const status = itemStatus(row);
          return <StatusBadge label={status.label} tone={status.tone} />;
        },
      },
    ],
    [],
  );

  const reset = () => {
    setBranchID("");
    setRegionID("");
    resetRange();
  };

  return (
    <main className="management-page">
      <header className="page-heading">
        <div>
          <p className="page-heading__eyebrow">Inventory control</p>
          <h1>Inventory Health</h1>
          <p>
            Review availability, reservations, quarantine, damage, expiry, and
            low-stock exposure.
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

      {state.errorMessage ? (
        <ErrorPanel message={state.errorMessage} onRetry={() => void load()} />
      ) : null}

      {state.data ? (
        <>
          <section className="kpi-grid">
            <KpiCard
              label="Available quantity"
              value={formatQuantity(state.data.availableQuantity)}
              tone="success"
            />
            <KpiCard
              label="Reserved quantity"
              value={formatQuantity(state.data.reservedQuantity)}
              tone="info"
            />
            <KpiCard
              label="Near-expiry batches"
              value={formatNumber(state.data.nearExpiryBatchCount)}
              tone="warning"
            />
            <KpiCard
              label="Expired batches"
              value={formatNumber(state.data.expiredBatchCount)}
              tone="danger"
            />
            <KpiCard
              label="Low-stock SKUs"
              value={formatNumber(state.data.lowStockSkuCount)}
              hint={`Threshold: ${formatQuantity(state.data.lowStockThreshold)}`}
              tone="warning"
            />
          </section>

          <ChartCard
            title="Inventory quantity profile"
            subtitle="Current quantity allocation across stock conditions"
          >
            <div className="chart-height">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart
                  data={chartData}
                  margin={{ top: 12, right: 20, left: 8, bottom: 8 }}
                >
                  <CartesianGrid strokeDasharray="3 3" vertical={false} />
                  <XAxis dataKey="metric" tickLine={false} axisLine={false} />
                  <YAxis tickLine={false} axisLine={false} width={72} />
                  <Tooltip
                    formatter={(value) => formatQuantity(Number(value))}
                  />
                  <Bar
                    dataKey="value"
                    fill="var(--msx-color-brand-600)"
                    radius={[8, 8, 0, 0]}
                  />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </ChartCard>

          <section className="table-card">
            <DataTable
              rows={state.data.items}
              columns={columns}
              keyExtractor={(row) =>
                `${row.warehouseID}-${row.skuID}-${row.batchID ?? 0}`
              }
              loading={state.isLoading}
              emptyMessage="No inventory health records were returned."
            />
          </section>
        </>
      ) : null}
    </main>
  );
}

