namespace MarketSphere.Application.Common.Security;

public static class PermissionEvaluator
{
    public static bool HasPermission(
        IEnumerable<string> assignedPermissions,
        string requiredPermission) =>
        assignedPermissions.Contains(
            requiredPermission,
            StringComparer.OrdinalIgnoreCase);
}
