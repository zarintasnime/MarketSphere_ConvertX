using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Mapping;
using MarketSphere.Application.Modules.OrganizationSecurity.DTOs;
using MarketSphere.Application.Modules.OrganizationSecurity.Interfaces;
using MarketSphere.Domain.Constants;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Exceptions;

namespace MarketSphere.Application.Modules.OrganizationSecurity.Services;

public sealed class RoleService : IRoleService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _clock;

    public RoleService(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IDateTimeProvider clock)
    {
        _db = db;
        _currentUser = currentUser;
        _clock = clock;
    }

    public Task<IReadOnlyCollection<RoleListItemDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<RoleListItemDto> result =
            _db.Roles
                .OrderBy(x => x.RoleLevel)
                .ThenBy(x => x.RoleName)
                .Select(x => new RoleListItemDto(
                    x.RoleID,
                    x.RoleCode,
                    x.RoleName,
                    x.RoleLevel,
                    x.IsActive))
                .ToArray();

        return Task.FromResult(result);
    }

    public Task<RoleDetailsDto> GetByIDAsync(
        int roleID,
        CancellationToken cancellationToken = default)
    {
        var role = _db.Roles.FirstOrDefault(
            x => x.RoleID == roleID)
            ?? throw new NotFoundException(
                "Role was not found.");

        var allowedPermissionIDs =
            _db.RolePermissions
                .Where(
                    x => x.RoleID == roleID &&
                         x.IsAllowed)
                .Select(x => x.PermissionID)
                .ToHashSet();

        var permissions = _db.Permissions
            .OrderBy(x => x.ModuleName)
            .ThenBy(x => x.ActionName)
            .ToArray()
            .Select(x => new PermissionDto(
                x.PermissionID,
                x.ModuleName,
                x.ActionName,
                x.PermissionCode,
                x.Description,
                allowedPermissionIDs.Contains(
                    x.PermissionID)))
            .ToArray();

        var result = new RoleDetailsDto(
            role.RoleID,
            role.RoleCode,
            role.RoleName,
            role.Description,
            role.RoleLevel,
            role.IsActive,
            permissions);

        return Task.FromResult(result);
    }

    public async Task<int> CreateAsync(
        CreateRoleRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var code = request.RoleCode.NormalizeCode();

        if (_db.Roles.Any(x => x.RoleCode == code))
        {
            throw new ConflictException(
                "Role code is already in use.");
        }

        var role = new Role
        {
            RoleCode = code,
            RoleName = request.RoleName.Trim(),
            Description =
                request.Description.NullIfWhiteSpace(),
            RoleLevel = request.RoleLevel,
            IsActive = true
        };

        await _db.AddAsync(role, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return role.RoleID;
    }

    public async Task UpdateAsync(
        int roleID,
        UpdateRoleRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var role = _db.Roles.FirstOrDefault(
            x => x.RoleID == roleID)
            ?? throw new NotFoundException(
                "Role was not found.");

        if (role.RoleCode == RoleCodes.SuperAdmin &&
            !request.IsActive)
        {
            throw new BusinessRuleException(
                "The Super Admin role cannot be deactivated.");
        }

        role.RoleName = request.RoleName.Trim();
        role.Description =
            request.Description.NullIfWhiteSpace();
        role.RoleLevel = request.RoleLevel;
        role.IsActive = request.IsActive;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePermissionsAsync(
        int roleID,
        UpdateRolePermissionsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var role = _db.Roles.FirstOrDefault(
            x => x.RoleID == roleID)
            ?? throw new NotFoundException(
                "Role was not found.");

        if (role.RoleCode == RoleCodes.SuperAdmin)
        {
            throw new BusinessRuleException(
                "The Super Admin permission set is managed by the system seed.");
        }

        var permissionIDs =
            request.AllowedPermissionIDs
                .Distinct()
                .ToArray();

        var validPermissionCount =
            _db.Permissions.Count(
                x => permissionIDs.Contains(
                    x.PermissionID));

        if (validPermissionCount !=
            permissionIDs.Length)
        {
            throw new NotFoundException(
                "One or more permissions were not found.");
        }

        var existing =
            _db.RolePermissions
                .Where(x => x.RoleID == roleID)
                .ToArray();

        foreach (var item in existing)
            _db.Remove(item);

        foreach (var permissionID in permissionIDs)
        {
            var rolePermission =
                new RolePermission
                {
                    RoleID = roleID,
                    PermissionID = permissionID,
                    IsAllowed = true,
                    CreatedByUserID =
                        _currentUser.UserID,
                    CreatedAt = _clock.UtcNow
                };

            await _db.AddAsync(
                rolePermission,
                cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
