import type { StatusTone } from "./common.types";

export type TrendDirection = "up" | "down" | "neutral";

export interface KpiMetric {
  id: string;
  label: string;
  value: string | number;
  hint?: string;
  trendValue?: string;
  trendDirection?: TrendDirection;
  tone?: StatusTone | "brand";
}

export interface ChartPoint {
  label: string;
  value: number;
  series?: string;
}

export interface FunnelChartPoint {
  stage: string;
  count: number;
  conversionRate: number | null;
}

export interface CampaignRoiChartPoint {
  campaignID: number;
  label: string;
  fullLabel: string;
  budget: number;
  expense: number;
  attributed: number;
  delivered: number;
  roiPercent: number;
}

export interface ChannelSalesChartPoint {
  channel: string;
  orderCount: number;
  gross: number;
  net: number;
  delivered: number;
}

export interface SellInSellOutChartPoint {
  period: string;
  sellInQuantity: number;
  sellOutQuantity: number;
  sellInValue: number;
  sellOutValue: number;
}

export interface DeliveryReturnChartPoint {
  period: string;
  planned: number;
  delivered: number;
  partial: number;
  failed: number;
  rescheduled: number;
  returns: number;
  returnedQuantity: number;
}

export interface EmployeeKpiChartPoint {
  employeeID: number;
  employee: string;
  target: number;
  actual: number;
  achievement: number;
  reward: number;
}
