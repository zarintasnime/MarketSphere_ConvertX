export interface OperationsDashboardSummary {
  totalUsers: number;
  activeUsers: number;
  totalRoles: number;
  activeRoles: number;
  totalEmployees: number;
  activeEmployees: number;
  totalBranches: number;
  activeBranches: number;
}

export interface DashboardQuickLink {
  label: string;
  description: string;
  route: string;
  permission?: string;
}
