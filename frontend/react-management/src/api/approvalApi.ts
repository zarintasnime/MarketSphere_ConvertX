import httpClient from "./httpClient";
import type { ApiResponse, PagedResult } from "../types/common.types";
import type {
  ApprovalActionRequest,
  ApprovalQueueQuery,
  ApprovalRequest,
} from "../types/approval.types";

function requireData<T>(response: ApiResponse<T>): T {
  if (!response.succeeded || response.data === null) {
    throw new Error(
      response.message || "The approval response did not contain data.",
    );
  }

  return response.data;
}

export async function getApprovalQueue(
  query: ApprovalQueueQuery,
): Promise<PagedResult<ApprovalRequest>> {
  const response = await httpClient.get<
    ApiResponse<PagedResult<ApprovalRequest>>
  >("/approvals", { params: query });

  return requireData(response.data);
}

export async function getApprovalRequest(
  approvalRequestID: number,
): Promise<ApprovalRequest> {
  const response = await httpClient.get<ApiResponse<ApprovalRequest>>(
    `/approvals/${approvalRequestID}`,
  );

  return requireData(response.data);
}

export async function recordApprovalAction(
  approvalRequestID: number,
  request: ApprovalActionRequest,
): Promise<boolean> {
  const response = await httpClient.post<ApiResponse<boolean>>(
    `/approvals/${approvalRequestID}/actions`,
    request,
  );

  return requireData(response.data);
}
