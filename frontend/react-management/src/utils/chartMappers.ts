import type {
  CampaignRoi,
  ChannelSales,
  DeliveryReturnPoint,
  EmployeeKpi,
  FunnelPoint,
  SellInSellOutPoint,
  SeriesPoint,
} from "../types/analytics.types";
import type {
  CampaignRoiChartPoint,
  ChannelSalesChartPoint,
  ChartPoint,
  DeliveryReturnChartPoint,
  EmployeeKpiChartPoint,
  FunnelChartPoint,
  SellInSellOutChartPoint,
} from "../types/dashboard.types";

export const salesChannelLabels: Readonly<Record<number, string>> = {
  1: "General Trade",
  2: "Modern Trade",
  3: "Business Partner",
  4: "Institutional",
  5: "Online",
};

function formatPeriod(value: string): string {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;

  return new Intl.DateTimeFormat("en-US", {
    month: "short",
    day: "2-digit",
  }).format(date);
}

export function mapSalesTrend(points: readonly SeriesPoint[]): ChartPoint[] {
  return points.map((point) => ({
    label: formatPeriod(point.period),
    value: point.value,
    series: point.series ?? "Sales",
  }));
}

export function mapFunnel(points: readonly FunnelPoint[]): FunnelChartPoint[] {
  return points.map((point, index) => {
    const previousCount =
      index === 0 ? null : (points[index - 1]?.count ?? null);
    const conversionRate =
      previousCount && previousCount > 0
        ? (point.count / previousCount) * 100
        : null;

    return {
      stage: point.stage,
      count: point.count,
      conversionRate,
    };
  });
}

export function mapCampaignRoi(
  rows: readonly CampaignRoi[],
): CampaignRoiChartPoint[] {
  return rows.map((row) => ({
    campaignID: row.campaignID,
    label: row.campaignCode,
    fullLabel: `${row.campaignCode} - ${row.campaignTitle}`,
    budget: row.budget,
    expense: row.expense,
    attributed: row.attributedValue,
    delivered: row.deliveredValue,
    roiPercent: row.roiPercent,
  }));
}

export function mapChannelSales(
  rows: readonly ChannelSales[],
): ChannelSalesChartPoint[] {
  return rows.map((row) => ({
    channel: salesChannelLabels[row.channel] ?? `Channel ${row.channel}`,
    orderCount: row.orderCount,
    gross: row.grossAmount,
    net: row.netAmount,
    delivered: row.deliveredValue,
  }));
}

export function mapSellInSellOut(
  rows: readonly SellInSellOutPoint[],
): SellInSellOutChartPoint[] {
  return rows.map((row) => ({
    period: formatPeriod(row.period),
    sellInQuantity: row.sellInQuantity,
    sellOutQuantity: row.sellOutQuantity,
    sellInValue: row.sellInValue,
    sellOutValue: row.sellOutValue,
  }));
}

export function mapDeliveryReturn(
  rows: readonly DeliveryReturnPoint[],
): DeliveryReturnChartPoint[] {
  return rows.map((row) => ({
    period: formatPeriod(row.period),
    planned: row.plannedCount,
    delivered: row.deliveredCount,
    partial: row.partialCount,
    failed: row.failedCount,
    rescheduled: row.rescheduledCount,
    returns: row.returnRequestCount,
    returnedQuantity: row.returnedQuantity,
  }));
}

export function mapEmployeeKpi(
  rows: readonly EmployeeKpi[],
): EmployeeKpiChartPoint[] {
  return [...rows]
    .sort((left, right) => right.achievementPercent - left.achievementPercent)
    .map((row) => ({
      employeeID: row.employeeID,
      employee: row.employeeName,
      target: row.targetValue,
      actual: row.actualValue,
      achievement: row.achievementPercent,
      reward: row.rewardAmount,
    }));
}
