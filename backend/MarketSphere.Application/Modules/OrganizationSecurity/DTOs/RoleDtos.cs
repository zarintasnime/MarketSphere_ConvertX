using System.ComponentModel.DataAnnotations;

namespace MarketSphere.Application.Modules.OrganizationSecurity.DTOs;

public sealed record PermissionDto(
    int PermissionID,
    string ModuleName,
    string ActionName,
    string PermissionCode,
    string? Description,
    bool IsAllowed);

public sealed record RoleListItemDto(
    int RoleID,
    string RoleCode,
    string RoleName,
    int RoleLevel,
    bool IsActive);

public sealed record RoleDetailsDto(
    int RoleID,
    string RoleCode,
    string RoleName,
    string? Description,
    int RoleLevel,
    bool IsActive,
    IReadOnlyCollection<PermissionDto> Permissions);

public sealed class CreateRoleRequestDto
{
    [Required, MaxLength(50)]
    public string RoleCode { get; init; } = string.Empty;

    [Required, MaxLength(100)]
    public string RoleName { get; init; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; init; }

    [Range(1, 1000)]
    public int RoleLevel { get; init; } = 100;
}

public sealed class UpdateRoleRequestDto
{
    [Required, MaxLength(100)]
    public string RoleName { get; init; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; init; }

    [Range(1, 1000)]
    public int RoleLevel { get; init; } = 100;

    public bool IsActive { get; init; } = true;
}

public sealed record UpdateRolePermissionsRequestDto(
    IReadOnlyCollection<int> AllowedPermissionIDs);
