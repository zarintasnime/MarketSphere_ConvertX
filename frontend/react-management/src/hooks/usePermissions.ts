import { useMemo } from "react";

import { useAuth } from "../auth/useAuth";

export function usePermissions() {
  const { currentUser, hasPermission, hasAnyPermission, hasEveryPermission } =
    useAuth();

  return useMemo(
    () => ({
      permissions: currentUser?.permissionCodes ?? [],
      hasPermission,
      hasAnyPermission,
      hasEveryPermission,
    }),
    [currentUser, hasPermission, hasAnyPermission, hasEveryPermission],
  );
}
