export type ApprovalType = 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 99;

export type ApprovalRequestStatus = 1 | 2 | 3 | 4 | 5;
export type ApprovalActionType = 1 | 2 | 3 | 4 | 5 | 6;

export interface ApprovalAction {
  approvalActionID: number;
  stepNo: number;
  actionByUserID: number;
  actionByName: string;
  action: ApprovalActionType;
  actionAt: string;
  note: string | null;
  delegatedFromUserID: number | null;
}

export interface ApprovalRequest {
  approvalRequestID: number;
  referenceType: string;
  referenceID: number;
  approvalType: ApprovalType;
  approvalPolicyID: number;
  requestedByUserID: number;
  requestedAt: string;
  currentStepNo: number;
  status: ApprovalRequestStatus;
  completedAt: string | null;
  actions: readonly ApprovalAction[];
}

export interface ApprovalQueueQuery {
  pageNumber: number;
  pageSize: number;
  search?: string;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface ApprovalActionRequest {
  action: ApprovalActionType;
  note: string | null;
  delegateToUserID: number | null;
}
