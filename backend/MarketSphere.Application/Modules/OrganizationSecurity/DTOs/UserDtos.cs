using MarketSphere.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MarketSphere.Application.Modules.OrganizationSecurity.DTOs;

public sealed class CreateUserRequestDto
{
    [Required, MaxLength(150)]
    public string FullName { get; init; } = string.Empty;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; init; } = string.Empty;

    [MaxLength(30)]
    public string? Phone { get; init; }

    [Required]
    public string TemporaryPassword { get; init; } = string.Empty;

    public bool ActivateImmediately { get; init; }

    public IReadOnlyCollection<int> RoleIDs { get; init; } =
        Array.Empty<int>();
}

public sealed class UpdateUserRequestDto
{
    [Required, MaxLength(150)]
    public string FullName { get; init; } = string.Empty;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; init; } = string.Empty;

    [MaxLength(30)]
    public string? Phone { get; init; }
}

public sealed record UserListItemDto(
    int UserID,
    string FullName,
    string Email,
    string? Phone,
    UserStatus Status,
    bool MustChangePassword,
    IReadOnlyCollection<string> RoleCodes);

public sealed record UserDetailsDto(
    int UserID,
    string FullName,
    string Email,
    string? Phone,
    UserStatus Status,
    bool MustChangePassword,
    DateTime? AccountActivatedAt,
    int FailedLoginCount,
    DateTime? LockoutEndAt,
    IReadOnlyCollection<int> RoleIDs,
    IReadOnlyCollection<string> RoleCodes);

public sealed record ChangeUserStatusRequestDto(UserStatus Status);

public sealed record AssignUserRolesRequestDto(
    IReadOnlyCollection<int> RoleIDs);

public sealed record AccountTokenResultDto(
    string Token,
    DateTime ExpiresAt);
