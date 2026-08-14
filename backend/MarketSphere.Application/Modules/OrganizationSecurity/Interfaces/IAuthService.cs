using MarketSphere.Application.Modules.OrganizationSecurity.DTOs;

namespace MarketSphere.Application.Modules.OrganizationSecurity.Interfaces;

public interface IAuthService
{
    Task<AuthSessionDto> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default);

    Task<AuthSessionDto> RefreshSessionAsync(
        RefreshSessionRequestDto request,
        CancellationToken cancellationToken = default);

    Task ChangePasswordAsync(
        ChangePasswordRequestDto request,
        CancellationToken cancellationToken = default);

    Task ActivateAccountAsync(
        ActivateAccountRequestDto request,
        CancellationToken cancellationToken = default);

    Task ResetPasswordAsync(
        ResetPasswordRequestDto request,
        CancellationToken cancellationToken = default);

    Task RevokeSessionAsync(
        int userSessionID,
        CancellationToken cancellationToken = default);

    Task<AuthenticatedUserDto> GetCurrentUserAsync(
        CancellationToken cancellationToken = default);
}
