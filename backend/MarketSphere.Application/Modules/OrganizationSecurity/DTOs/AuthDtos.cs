using System.ComponentModel.DataAnnotations;

namespace MarketSphere.Application.Modules.OrganizationSecurity.DTOs;

public sealed class LoginRequestDto
{
    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; init; } = string.Empty;

    [Required, MaxLength(200)]
    public string Password { get; init; } = string.Empty;

    [Required, MaxLength(200)]
    public string DeviceIdentifier { get; init; } = string.Empty;

    [MaxLength(200)]
    public string? DeviceName { get; init; }
}

public sealed class RefreshSessionRequestDto
{
    [Required]
    public string RefreshToken { get; init; } = string.Empty;

    [Required, MaxLength(200)]
    public string DeviceIdentifier { get; init; } = string.Empty;

    [MaxLength(200)]
    public string? DeviceName { get; init; }
}

public sealed class ChangePasswordRequestDto
{
    [Required]
    public string CurrentPassword { get; init; } = string.Empty;

    [Required]
    public string NewPassword { get; init; } = string.Empty;
}

public sealed class ActivateAccountRequestDto
{
    [Required]
    public string Token { get; init; } = string.Empty;

    [Required]
    public string NewPassword { get; init; } = string.Empty;
}

public sealed class ResetPasswordRequestDto
{
    [Required]
    public string Token { get; init; } = string.Empty;

    [Required]
    public string NewPassword { get; init; } = string.Empty;
}

public sealed record AuthenticatedUserDto(
    int UserID,
    int? EmployeeID,
    string FullName,
    string Email,
    bool MustChangePassword,
    IReadOnlyCollection<string> RoleCodes,
    IReadOnlyCollection<string> PermissionCodes);

public sealed record AuthSessionDto(
    AuthenticatedUserDto User,
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);
