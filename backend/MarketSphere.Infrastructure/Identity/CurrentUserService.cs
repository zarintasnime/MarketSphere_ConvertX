using MarketSphere.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MarketSphere.Infrastructure.Identity;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal =>
        _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        Principal?.Identity?.IsAuthenticated == true;

    public int? UserID => ReadIntegerClaim(
        ClaimTypes.NameIdentifier,
        "sub");

    public int? EmployeeID => ReadIntegerClaim(
        "employee_id");

    public string? Email =>
        Principal?.FindFirstValue(ClaimTypes.Email)
        ?? Principal?.FindFirstValue("email");

    public IReadOnlyCollection<string> RoleCodes
    {
        get
        {
            var principal = Principal;

            if (principal is null)
                return Array.Empty<string>();

            return principal
                .FindAll(ClaimTypes.Role)
                .Concat(principal.FindAll("role"))
                .Select(x => x.Value)
                .Distinct(StringComparer.Ordinal)
                .ToArray();
        }
    }

    public IReadOnlyCollection<string> PermissionCodes
    {
        get
        {
            var principal = Principal;

            if (principal is null)
                return Array.Empty<string>();

            return principal
                .FindAll("permission")
                .Select(x => x.Value)
                .Distinct(StringComparer.Ordinal)
                .ToArray();
        }
    }

    private int? ReadIntegerClaim(params string[] claimTypes)
    {
        var principal = Principal;

        foreach (var claimType in claimTypes)
        {
            var value = principal?.FindFirstValue(claimType);

            if (int.TryParse(value, out var parsed))
                return parsed;
        }

        return null;
    }
}
