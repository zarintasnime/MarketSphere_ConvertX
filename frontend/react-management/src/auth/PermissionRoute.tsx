import { Navigate, Outlet, useLocation } from "react-router-dom";

import { useAuth } from "./useAuth";

export interface PermissionRouteProps {
  requiredPermissions: readonly string[];
  match?: "all" | "any";
}

export default function PermissionRoute({
  requiredPermissions,
  match = "all",
}: PermissionRouteProps) {
  const location = useLocation();
  const { hasAnyPermission, hasEveryPermission } = useAuth();

  const isAllowed =
    requiredPermissions.length === 0 ||
    (match === "any"
      ? hasAnyPermission(requiredPermissions)
      : hasEveryPermission(requiredPermissions));

  return isAllowed ? (
    <Outlet />
  ) : (
    <Navigate
      to="/access-denied"
      replace
      state={{ attemptedPath: location.pathname }}
    />
  );
}
