using MarketSphere.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace MarketSphere.Api.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddApiAuthorization(
        this IServiceCollection services)
    {
        services.AddAuthorization();

        services.AddSingleton<
            IAuthorizationHandler,
            PermissionAuthorizationHandler>();

        services.AddSingleton<
            IAuthorizationPolicyProvider,
            PermissionPolicyProvider>();

        return services;
    }
}

public sealed class PermissionPolicyProvider :
    DefaultAuthorizationPolicyProvider
{
    public PermissionPolicyProvider(
        IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?>
        GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(
                HasPermissionAttribute.PolicyPrefix,
                StringComparison.Ordinal))
        {
            var permissionCode = policyName[
                HasPermissionAttribute.PolicyPrefix.Length..];

            if (string.IsNullOrWhiteSpace(permissionCode))
                return null;

            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(
                    new PermissionRequirement(
                        permissionCode))
                .Build();
        }

        return await base.GetPolicyAsync(policyName);
    }
}
