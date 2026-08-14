using MarketSphere.Application.Common.Models;
using System.Security.Claims;

namespace MarketSphere.Infrastructure.Identity;

public static class ClaimFactory
{
    public static IReadOnlyCollection<Claim> Create(
        CurrentUserInfo user)
    {
        var claims = new List<Claim>
        {
            new(
                ClaimTypes.NameIdentifier,
                user.UserID.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email)
        };

        claims.AddRange(
            user.RoleCodes.Select(
                role => new Claim(
                    ClaimTypes.Role,
                    role)));

        claims.AddRange(
            user.PermissionCodes.Select(
                permission => new Claim(
                    "permission",
                    permission)));

        return claims;
    }
}
