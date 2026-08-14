using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Mapping;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Security;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.OrganizationSecurity.DTOs;
using MarketSphere.Application.Modules.OrganizationSecurity.Interfaces;
using MarketSphere.Domain.Constants;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using System.Security.Cryptography;

namespace MarketSphere.Application.Modules.OrganizationSecurity.Services;

public sealed class UserService : IUserService
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _clock;

    public UserService(
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

    public Task<PagedResult<UserListItemDto>> GetPagedAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        PaginationValidator.Validate(request);

        var query = _db.Users;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLowerInvariant();

            query = query.Where(
                x => x.FullName.ToLower().Contains(search) ||
                     x.Email.Contains(search));
        }

        var totalCount = query.Count();

        var users = query
            .OrderBy(x => x.FullName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToArray();

        var items = users
            .Select(user => new UserListItemDto(
                user.UserID,
                user.FullName,
                user.Email,
                user.Phone,
                user.Status,
                user.MustChangePassword,
                GetRoleCodes(user.UserID)))
            .ToArray();

        var result = PagedResult<UserListItemDto>.Create(
            items,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Task.FromResult(result);
    }

    public Task<UserDetailsDto> GetByIDAsync(
        int userID,
        CancellationToken cancellationToken = default)
    {
        var user =
            OrganizationSecurityServiceHelper.RequireUser(
                _db,
                userID);

        var roleIDs = _db.UserRoles
            .Where(x => x.UserID == userID)
            .Select(x => x.RoleID)
            .ToArray();

        var result = new UserDetailsDto(
            user.UserID,
            user.FullName,
            user.Email,
            user.Phone,
            user.Status,
            user.MustChangePassword,
            user.AccountActivatedAt,
            user.FailedLoginCount,
            user.LockoutEndAt,
            roleIDs,
            GetRoleCodes(userID));

        return Task.FromResult(result);
    }

    public async Task<int> CreateAsync(
        CreateUserRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidationHelper.RequireNotBlank(
            request.FullName,
            nameof(request.FullName),
            150);

        PasswordPolicy.Validate(request.TemporaryPassword);

        var email = request.Email.NormalizeEmail();

        if (_db.Users.Any(x => x.Email == email))
        {
            throw new ConflictException(
                BusinessRuleMessages.DuplicateEmail);
        }

        var roleIDs = request.RoleIDs
            .Distinct()
            .ToArray();

        if (roleIDs.Length > 0)
        {
            var validRoleCount = _db.Roles.Count(
                x => roleIDs.Contains(x.RoleID) &&
                     x.IsActive);

            if (validRoleCount != roleIDs.Length)
            {
                throw new NotFoundException(
                    "One or more selected roles were not found or are inactive.");
            }
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            Phone = request.Phone.NullIfWhiteSpace(),
            PasswordHash =
                _passwordHashService.HashPassword(
                    request.TemporaryPassword),
            Status = request.ActivateImmediately
                ? UserStatus.Active
                : UserStatus.Invited,
            MustChangePassword = true,
            AccountActivatedAt = request.ActivateImmediately
                ? _clock.UtcNow
                : null
        };

        await _db.AddAsync(user, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        var assignedByUserID =
            _currentUser.RequireUserID();

        foreach (var roleID in roleIDs)
        {
            var userRole = new UserRole
            {
                UserID = user.UserID,
                RoleID = roleID,
                AssignedAt = _clock.UtcNow,
                AssignedByUserID = assignedByUserID
            };

            await _db.AddAsync(
                userRole,
                cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);

        return user.UserID;
    }

    public async Task UpdateAsync(
        int userID,
        UpdateUserRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var user =
            OrganizationSecurityServiceHelper.RequireUser(
                _db,
                userID);

        var email = request.Email.NormalizeEmail();

        if (_db.Users.Any(
                x => x.Email == email &&
                     x.UserID != userID))
        {
            throw new ConflictException(
                BusinessRuleMessages.DuplicateEmail);
        }

        user.FullName = request.FullName.Trim();
        user.Email = email;
        user.Phone = request.Phone.NullIfWhiteSpace();

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeStatusAsync(
        int userID,
        ChangeUserStatusRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var user =
            OrganizationSecurityServiceHelper.RequireUser(
                _db,
                userID);

        var superAdminUserIDs = (
            from userRole in _db.UserRoles
            join role in _db.Roles
                on userRole.RoleID equals role.RoleID
            where role.RoleCode ==
                  RoleCodes.SuperAdmin
            select userRole.UserID)
            .ToHashSet();

        if (superAdminUserIDs.Contains(userID) &&
            request.Status == UserStatus.Disabled)
        {
            throw new BusinessRuleException(
                "A Super Admin account cannot be disabled.");
        }

        user.Status = request.Status;

        if (request.Status == UserStatus.Active)
        {
            user.FailedLoginCount = 0;
            user.LockoutEndAt = null;
            user.AccountActivatedAt ??= _clock.UtcNow;
        }
        else if (request.Status is
                 UserStatus.Disabled or
                 UserStatus.Locked)
        {
            var activeSessions =
                _db.UserSessions.Where(
                    x => x.UserID == userID &&
                         x.RevokedAt == null);

            foreach (var session in activeSessions)
                session.RevokedAt = _clock.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AssignRolesAsync(
        int userID,
        AssignUserRolesRequestDto request,
        CancellationToken cancellationToken = default)
    {
        OrganizationSecurityServiceHelper.RequireUser(
            _db,
            userID);

        var roleIDs = request.RoleIDs
            .Distinct()
            .ToArray();

        var validRoleCount = _db.Roles.Count(
            x => roleIDs.Contains(x.RoleID) &&
                 x.IsActive);

        if (validRoleCount != roleIDs.Length)
        {
            throw new NotFoundException(
                "One or more selected roles were not found or are inactive.");
        }

        var superAdminRoleID = _db.Roles
            .Where(
                x => x.RoleCode ==
                     RoleCodes.SuperAdmin)
            .Select(x => x.RoleID)
            .First();

        var currentlySuperAdmin = _db.UserRoles.Any(
            x => x.UserID == userID &&
                 x.RoleID == superAdminRoleID);

        if (currentlySuperAdmin &&
            !roleIDs.Contains(superAdminRoleID))
        {
            var totalSuperAdmins = _db.UserRoles.Count(
                x => x.RoleID == superAdminRoleID);

            if (totalSuperAdmins <= 1)
            {
                throw new BusinessRuleException(
                    "The final Super Admin role assignment cannot be removed.");
            }
        }

        var existing = _db.UserRoles
            .Where(x => x.UserID == userID)
            .ToArray();

        foreach (var item in existing)
            _db.Remove(item);

        var assignedByUserID =
            _currentUser.RequireUserID();

        foreach (var roleID in roleIDs)
        {
            await _db.AddAsync(
                new UserRole
                {
                    UserID = userID,
                    RoleID = roleID,
                    AssignedAt = _clock.UtcNow,
                    AssignedByUserID = assignedByUserID
                },
                cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<AccountTokenResultDto> CreateAccountTokenAsync(
        int userID,
        AccountTokenType tokenType,
        CancellationToken cancellationToken = default)
    {
        OrganizationSecurityServiceHelper.RequireUser(
            _db,
            userID);

        var rawToken = Base64UrlEncode(
            RandomNumberGenerator.GetBytes(48));

        var expiresAt = _clock.UtcNow.AddHours(
            tokenType == AccountTokenType.Activation
                ? 48
                : 2);

        var activeOldTokens =
            _db.UserAccountTokens.Where(
                x => x.UserID == userID &&
                     x.TokenType == tokenType &&
                     x.UsedAt == null &&
                     x.ExpiresAt > _clock.UtcNow);

        foreach (var oldToken in activeOldTokens)
            oldToken.UsedAt = _clock.UtcNow;

        var token = new UserAccountToken
        {
            UserID = userID,
            TokenType = tokenType,
            TokenHash =
                _jwtTokenService.HashToken(rawToken),
            ExpiresAt = expiresAt,
            CreatedByUserID =
                _currentUser.RequireUserID()
        };

        await _db.AddAsync(
            token,
            cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);

        return new AccountTokenResultDto(
            rawToken,
            expiresAt);
    }

    private IReadOnlyCollection<string> GetRoleCodes(
        int userID) =>
        (
            from userRole in _db.UserRoles
            join role in _db.Roles
                on userRole.RoleID equals role.RoleID
            where userRole.UserID == userID
            orderby role.RoleLevel
            select role.RoleCode
        ).ToArray();

    private static string Base64UrlEncode(
        byte[] value) =>
        Convert.ToBase64String(value)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
}
