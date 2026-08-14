namespace MarketSphere.Application.Common.Interfaces;

public interface ICurrentUserService
{
    bool IsAuthenticated { get; }
    int? UserID { get; }
    int? EmployeeID { get; }
    string? Email { get; }
    IReadOnlyCollection<string> RoleCodes { get; }
    IReadOnlyCollection<string> PermissionCodes { get; }
}
