using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Domain.Constants;
using MarketSphere.Domain.Exceptions;

namespace MarketSphere.Application.Common.Security;

public static class CurrentUserExtensions
{
    private static readonly HashSet<string> FieldRoleCodes = new(
        new[]
        {
            RoleCodes.SalesOfficer,
            RoleCodes.ModernTradeExecutive,
            RoleCodes.BusinessPromoter,
            RoleCodes.Merchandiser
        },
        StringComparer.OrdinalIgnoreCase);

    public static int RequireUserID(this ICurrentUserService currentUser)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserID is null)
        {
            throw new ForbiddenBusinessActionException(
                "An authenticated user is required.");
        }

        return currentUser.UserID.Value;
    }

    public static int RequireEmployeeID(this ICurrentUserService currentUser)
    {
        if (!currentUser.IsAuthenticated || currentUser.EmployeeID is null)
        {
            throw new ForbiddenBusinessActionException(
                "The authenticated user is not linked to an active employee.");
        }

        return currentUser.EmployeeID.Value;
    }

    public static bool IsFieldUser(this ICurrentUserService currentUser) =>
        currentUser.RoleCodes.Any(FieldRoleCodes.Contains);

    public static int ResolveFieldEmployeeID(
        this ICurrentUserService currentUser,
        int submittedEmployeeID)
    {
        if (!currentUser.IsFieldUser())
            return submittedEmployeeID;

        var currentEmployeeID = currentUser.RequireEmployeeID();

        if (submittedEmployeeID > 0 &&
            submittedEmployeeID != currentEmployeeID)
        {
            throw new ForbiddenBusinessActionException(
                "A field user cannot submit a record for another employee.");
        }

        return currentEmployeeID;
    }

    public static int? ResolveOptionalFieldEmployeeID(
        this ICurrentUserService currentUser,
        int? submittedEmployeeID)
    {
        if (!currentUser.IsFieldUser())
            return submittedEmployeeID;

        var currentEmployeeID = currentUser.RequireEmployeeID();

        if (submittedEmployeeID.HasValue &&
            submittedEmployeeID.Value != currentEmployeeID)
        {
            throw new ForbiddenBusinessActionException(
                "A field user cannot submit a record for another employee.");
        }

        return currentEmployeeID;
    }

    public static void EnsureFieldRecordOwnership(
        this ICurrentUserService currentUser,
        int recordEmployeeID)
    {
        if (!currentUser.IsFieldUser())
            return;

        if (currentUser.RequireEmployeeID() != recordEmployeeID)
        {
            throw new ForbiddenBusinessActionException(
                "A field user cannot access another employee's field record.");
        }
    }

    public static void EnsureOptionalFieldRecordOwnership(
        this ICurrentUserService currentUser,
        int? recordEmployeeID)
    {
        if (!currentUser.IsFieldUser())
            return;

        if (!recordEmployeeID.HasValue ||
            currentUser.RequireEmployeeID() != recordEmployeeID.Value)
        {
            throw new ForbiddenBusinessActionException(
                "A field user cannot access another employee's field record.");
        }
    }
}
