using Microsoft.AspNetCore.Authorization;

namespace MarketSphere.Api.Authorization;

[AttributeUsage(
    AttributeTargets.Class |
    AttributeTargets.Method,
    AllowMultiple = true,
    Inherited = true)]
public sealed class HasPermissionAttribute :
    AuthorizeAttribute
{
    public const string PolicyPrefix =
        "Permission:";

    public HasPermissionAttribute(
        string permissionCode)
    {
        Policy =
            $"{PolicyPrefix}{permissionCode}";
    }
}
