namespace MarketSphere.Application.Common.Models;

public sealed record CurrentUserInfo(
    int UserID,
    int? EmployeeID,
    string FullName,
    string Email,
    IReadOnlyCollection<string> RoleCodes,
    IReadOnlyCollection<string> PermissionCodes);
