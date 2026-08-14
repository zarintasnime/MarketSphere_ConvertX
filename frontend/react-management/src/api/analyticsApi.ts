import httpClient from "./httpClient";
import type { ApiResponse } from "../types/common.types";
import type {
  AnalyticsFilter,
  CampaignRoi,
  ChannelSales,
  Client360,
  DeliveryReturnPoint,
  EmployeeKpi,
  ExecutiveDashboard,
  FunnelPoint,
  InventoryHealth,
  SellInSellOutPoint,
  SeriesPoint,
} from "../types/analytics.types";

function requireData<T>(response: ApiResponse<T>): T {
  if (!response.succeeded || response.data === null) {
    throw new Error(
      response.message || "The analytics response did not contain data.",
    );
  }

  return response.data;
}

function toQueryParams(
  filter: AnalyticsFilter,
): Record<string, string | number> {
  const params: Record<string, string | number> = {
    from: filter.from,
    to: filter.to,
  };

  if (filter.branchID !== null && filter.branchID !== undefined) {
    params.branchID = filter.branchID;
  }
  if (filter.regionID !== null && filter.regionID !== undefined) {
    params.regionID = filter.regionID;
  }
  if (filter.employeeID !== null && filter.employeeID !== undefined) {
    params.employeeID = filter.employeeID;
  }
  if (filter.campaignID !== null && filter.campaignID !== undefined) {
    params.campaignID = filter.campaignID;
  }

  return params;
}

export async function getExecutiveDashboard(
  filter: AnalyticsFilter,
): Promise<ExecutiveDashboard> {
  const response = await httpClient.get<ApiResponse<ExecutiveDashboard>>(
    "/analytics/executive-dashboard",
    { params: toQueryParams(filter) },
  );

  return requireData(response.data);
}

export async function getLeadToOrderFunnel(
  filter: AnalyticsFilter,
): Promise<readonly FunnelPoint[]> {
  const response = await httpClient.get<ApiResponse<readonly FunnelPoint[]>>(
    "/analytics/lead-to-order-funnel",
    { params: toQueryParams(filter) },
  );

  return requireData(response.data);
}

export async function getSalesTrend(
  filter: AnalyticsFilter,
): Promise<readonly SeriesPoint[]> {
  const response = await httpClient.get<ApiResponse<readonly SeriesPoint[]>>(
    "/analytics/sales-trend",
    { params: toQueryParams(filter) },
  );

  return requireData(response.data);
}

export async function getCampaignRoi(
  filter: AnalyticsFilter,
): Promise<readonly CampaignRoi[]> {
  const response = await httpClient.get<ApiResponse<readonly CampaignRoi[]>>(
    "/analytics/campaign-roi",
    { params: toQueryParams(filter) },
  );

  return requireData(response.data);
}

export async function getChannelSales(
  filter: AnalyticsFilter,
): Promise<readonly ChannelSales[]> {
  const response = await httpClient.get<ApiResponse<readonly ChannelSales[]>>(
    "/analytics/channel-sales",
    { params: toQueryParams(filter) },
  );

  return requireData(response.data);
}

export async function getSellInSellOut(
  filter: AnalyticsFilter,
): Promise<readonly SellInSellOutPoint[]> {
  const response = await httpClient.get<
    ApiResponse<readonly SellInSellOutPoint[]>
  >("/analytics/sell-in-sell-out", { params: toQueryParams(filter) });

  return requireData(response.data);
}

export async function getInventoryHealth(
  filter: AnalyticsFilter,
): Promise<InventoryHealth> {
  const response = await httpClient.get<ApiResponse<InventoryHealth>>(
    "/analytics/inventory-health",
    { params: toQueryParams(filter) },
  );

  return requireData(response.data);
}

export async function getDeliveryReturn(
  filter: AnalyticsFilter,
): Promise<readonly DeliveryReturnPoint[]> {
  const response = await httpClient.get<
    ApiResponse<readonly DeliveryReturnPoint[]>
  >("/analytics/delivery-return", { params: toQueryParams(filter) });

  return requireData(response.data);
}

export async function getEmployeeKpi(
  filter: AnalyticsFilter,
): Promise<readonly EmployeeKpi[]> {
  const response = await httpClient.get<ApiResponse<readonly EmployeeKpi[]>>(
    "/analytics/employee-kpi",
    { params: toQueryParams(filter) },
  );

  return requireData(response.data);
}

export async function getClient360(
  clientID: number,
  filter: AnalyticsFilter,
): Promise<Client360> {
  if (!Number.isInteger(clientID) || clientID <= 0) {
    throw new Error("A valid client ID is required.");
  }

  const response = await httpClient.get<ApiResponse<Client360>>(
    `/analytics/client-360/${clientID}`,
    { params: toQueryParams(filter) },
  );

  return requireData(response.data);
}
