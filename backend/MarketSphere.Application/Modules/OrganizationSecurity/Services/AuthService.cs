using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Security;
using MarketSphere.Application.Modules.OrganizationSecurity.DTOs;
using MarketSphere.Application.Modules.OrganizationSecurity.Interfaces;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;

namespace MarketSphere.Application.Modules.OrganizationSecurity.Services;

public sealed class AuthService : IAuthService
{
    private const int LockoutThreshold = 5;
    private const int LockoutMinutes = 15;

    private readonly IApplicationDbContext _db;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _clock;

    public AuthService(
        IApplicationDbContext db,
        IPasswordHashService passwordHashService,
        IJwtTokenService jwtTokenService,
        ICurrentUserService currentUser,
        IDateTimeProvider clock)
    {
        _db = db;
        _passwordHashService = passwordHashService;
        _jwtTokenService = jwtTokenService;
        _currentUser = currentUser;
        _clock = clock;
    }

    public async Task<AuthSessionDto> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = _db.Users.FirstOrDefault(x => x.Email == email)
            ?? throw new ForbiddenBusinessActionException(
                "Invalid email or password.");

        if (user.Status == UserStatus.Disabled)
        {
            throw new ForbiddenBusinessActionException(
                "This account is disabled.");
        }

        if (user.Status == UserStatus.Locked)
        {
            if (user.LockoutEndAt.HasValue &&
                user.LockoutEndAt.Value <= _clock.UtcNow)
            {
                user.Status = UserStatus.Active;
                user.FailedLoginCount = 0;
                user.LockoutEndAt = null;
            }
            else
            {
                throw new ForbiddenBusinessActionException(
                    "This account is temporarily locked.");
            }
        }

        if (!_passwordHashService.VerifyPassword(
                request.Password,
                user.PasswordHash))
        {
            user.FailedLoginCount++;

            if (user.FailedLoginCount >= LockoutThreshold)
            {
                user.Status = UserStatus.Locked;
                user.LockoutEndAt =
                    _clock.UtcNow.AddMinutes(LockoutMinutes);
            }

            await _db.SaveChangesAsync(cancellationToken);

            throw new ForbiddenBusinessActionException(
                "Invalid email or password.");
        }

        if (user.Status == UserStatus.Invited)
        {
            throw new ForbiddenBusinessActionException(
                "This account has not been activated.");
        }

        user.FailedLoginCount = 0;
        user.LockoutEndAt = null;

        var currentUserInfo =
            OrganizationSecurityServiceHelper.BuildCurrentUserInfo(
                _db,
                user);

        var token =
            _jwtTokenService.CreateToken(currentUserInfo);

        var session = new UserSession
        {
            UserID = user.UserID,
            DeviceIdentifier = request.DeviceIdentifier.Trim(),
            DeviceName = string.IsNullOrWhiteSpace(request.DeviceName)
                ? null
                : request.DeviceName.Trim(),
            RefreshTokenHash =
                _jwtTokenService.HashToken(token.RefreshToken),
            IssuedAt = _clock.UtcNow,
            ExpiresAt = token.RefreshTokenExpiresAt,
            LastSeenAt = _clock.UtcNow
        };

        await _db.AddAsync(session, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return ToSessionDto(user, token);
    }

    public async Task<AuthSessionDto> RefreshSessionAsync(
        RefreshSessionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var tokenHash =
            _jwtTokenService.HashToken(request.RefreshToken);

        var session = _db.UserSessions.FirstOrDefault(
            x => x.RefreshTokenHash == tokenHash)
            ?? throw new ForbiddenBusinessActionException(
                "The refresh token is invalid.");

        if (session.RevokedAt.HasValue ||
            session.ExpiresAt <= _clock.UtcNow)
        {
            throw new ForbiddenBusinessActionException(
                "The refresh token is expired or revoked.");
        }

        if (!string.Equals(
                session.DeviceIdentifier,
                request.DeviceIdentifier.Trim(),
                StringComparison.Ordinal))
        {
            throw new ForbiddenBusinessActionException(
                "The refresh token does not belong to this device.");
        }

        var user =
            OrganizationSecurityServiceHelper.RequireUser(
                _db,
                session.UserID);

        if (user.Status != UserStatus.Active)
        {
            throw new ForbiddenBusinessActionException(
                "The account is not active.");
        }

        var currentUserInfo =
            OrganizationSecurityServiceHelper.BuildCurrentUserInfo(
                _db,
                user);

        var token =
            _jwtTokenService.CreateToken(currentUserInfo);

        session.RefreshTokenHash =
            _jwtTokenService.HashToken(token.RefreshToken);

        session.ExpiresAt = token.RefreshTokenExpiresAt;
        session.LastSeenAt = _clock.UtcNow;

        if (!string.IsNullOrWhiteSpace(request.DeviceName))
            session.DeviceName = request.DeviceName.Trim();

        await _db.SaveChangesAsync(cancellationToken);

        return ToSessionDto(user, token);
    }

    public async Task ChangePasswordAsync(
        ChangePasswordRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var userID = _currentUser.RequireUserID();

        var user =
            OrganizationSecurityServiceHelper.RequireUser(
                _db,
                userID);

        if (!_passwordHashService.VerifyPassword(
                request.CurrentPassword,
                user.PasswordHash))
        {
            throw new ForbiddenBusinessActionException(
                "The current password is incorrect.");
        }

        PasswordPolicy.Validate(request.NewPassword);

        user.PasswordHash =
            _passwordHashService.HashPassword(request.NewPassword);

        user.MustChangePassword = false;
        user.AccountActivatedAt ??= _clock.UtcNow;
        user.Status = UserStatus.Active;

        RevokeAllSessions(user.UserID);

        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task ActivateAccountAsync(
        ActivateAccountRequestDto request,
        CancellationToken cancellationToken = default) =>
        UseAccountTokenAsync(
            request.Token,
            request.NewPassword,
            AccountTokenType.Activation,
            cancellationToken);

    public Task ResetPasswordAsync(
        ResetPasswordRequestDto request,
        CancellationToken cancellationToken = default) =>
        UseAccountTokenAsync(
            request.Token,
            request.NewPassword,
            AccountTokenType.PasswordReset,
            cancellationToken);

    public async Task RevokeSessionAsync(
        int userSessionID,
        CancellationToken cancellationToken = default)
    {
        var userID = _currentUser.RequireUserID();

        var session = _db.UserSessions.FirstOrDefault(
            x => x.UserSessionID == userSessionID &&
                 x.UserID == userID)
            ?? throw new NotFoundException(
                "Session was not found.");

        session.RevokedAt ??= _clock.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<AuthenticatedUserDto> GetCurrentUserAsync(
        CancellationToken cancellationToken = default)
    {
        var userID = _currentUser.RequireUserID();

        var user =
            OrganizationSecurityServiceHelper.RequireUser(
                _db,
                userID);

        var dto =
            OrganizationSecurityServiceHelper.ToAuthenticatedUserDto(
                _db,
                user);

        return Task.FromResult(dto);
    }

    private async Task UseAccountTokenAsync(
        string rawToken,
        string newPassword,
        AccountTokenType tokenType,
        CancellationToken cancellationToken)
    {
        PasswordPolicy.Validate(newPassword);

        var tokenHash =
            _jwtTokenService.HashToken(rawToken);

        var accountToken = _db.UserAccountTokens.FirstOrDefault(
            x => x.TokenHash == tokenHash &&
                 x.TokenType == tokenType)
            ?? throw new ForbiddenBusinessActionException(
                "The account token is invalid.");

        if (accountToken.UsedAt.HasValue)
        {
            throw new ForbiddenBusinessActionException(
                "The account token has already been used.");
        }

        if (accountToken.ExpiresAt <= _clock.UtcNow)
        {
            throw new ForbiddenBusinessActionException(
                "The account token has expired.");
        }

        var user =
            OrganizationSecurityServiceHelper.RequireUser(
                _db,
                accountToken.UserID);

        user.PasswordHash =
            _passwordHashService.HashPassword(newPassword);

        user.MustChangePassword = false;
        user.Status = UserStatus.Active;
        user.AccountActivatedAt ??= _clock.UtcNow;
        user.FailedLoginCount = 0;
        user.LockoutEndAt = null;

        accountToken.UsedAt = _clock.UtcNow;

        RevokeAllSessions(user.UserID);

        await _db.SaveChangesAsync(cancellationToken);
    }

    private void RevokeAllSessions(int userID)
    {
        var activeSessions =
            _db.UserSessions.Where(
                x => x.UserID == userID &&
                     x.RevokedAt == null);

        foreach (var session in activeSessions)
            session.RevokedAt = _clock.UtcNow;
    }

    private AuthSessionDto ToSessionDto(
        User user,
        TokenResult token) =>
        new(
            OrganizationSecurityServiceHelper.ToAuthenticatedUserDto(
                _db,
                user),
            token.AccessToken,
            token.AccessTokenExpiresAt,
            token.RefreshToken,
            token.RefreshTokenExpiresAt);
}
