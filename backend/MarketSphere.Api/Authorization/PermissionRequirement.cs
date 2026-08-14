using Microsoft.AspNetCore.Authorization;

namespace MarketSphere.Api.Authorization;

public sealed record PermissionRequirement(
    string PermissionCode)
    : IAuthorizationRequirement;
