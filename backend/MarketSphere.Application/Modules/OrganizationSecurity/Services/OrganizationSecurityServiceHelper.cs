using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.OrganizationSecurity.DTOs;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;

namespace MarketSphere.Application.Modules.OrganizationSecurity.Services;

internal static class OrganizationSecurityServiceHelper
{
    public static User RequireUser(
        IApplicationDbContext db,
        int userID) =>
        db.Users.FirstOrDefault(x => x.UserID == userID)
        ?? throw new NotFoundException("User was not found.");

    public static Employee RequireEmployee(
        IApplicationDbContext db,
        int employeeID) =>
        db.Employees.FirstOrDefault(x => x.EmployeeID == employeeID)
        ?? throw new NotFoundException("Employee was not found.");

    public static void ValidateDateRange(
        DateOnly start,
        DateOnly? end)
    {
        if (end.HasValue && end.Value < start)
        {
            throw new AppValidationException(
                "The end date cannot be earlier than the start date.");
        }
    }

    public static void ValidateEmployeeGeography(
        IApplicationDbContext db,
        int? regionID,
        int? areaID,
        int? territoryID)
    {
        if (territoryID.HasValue)
        {
            var territory = db.Territories.FirstOrDefault(
                x => x.TerritoryID == territoryID.Value)
                ?? throw new NotFoundException("Territory was not found.");

            var area = db.Areas.First(x => x.AreaID == territory.AreaID);

            if (areaID.HasValue && areaID.Value != territory.AreaID)
            {
                throw new BusinessRuleException(
                    "The territory does not belong to the selected area.");
            }

            if (regionID.HasValue && regionID.Value != area.RegionID)
            {
                throw new BusinessRuleException(
                    "The territory does not belong to the selected region.");
            }
        }
        else if (areaID.HasValue)
        {
            var area = db.Areas.FirstOrDefault(
                x => x.AreaID == areaID.Value)
                ?? throw new NotFoundException("Area was not found.");

            if (regionID.HasValue && regionID.Value != area.RegionID)
            {
                throw new BusinessRuleException(
                    "The area does not belong to the selected region.");
            }
        }
        else if (regionID.HasValue &&
                 !db.Regions.Any(x => x.RegionID == regionID.Value))
        {
            throw new NotFoundException("Region was not found.");
        }
    }

    public static CurrentUserInfo BuildCurrentUserInfo(
        IApplicationDbContext db,
        User user)
    {
        var employeeID = db.Employees
            .Where(x =>
                x.UserID == user.UserID &&
                x.Status == EmployeeStatus.Active)
            .Select(x => (int?)x.EmployeeID)
            .SingleOrDefault();

        var roleCodes = (
            from userRole in db.UserRoles
            join role in db.Roles
                on userRole.RoleID equals role.RoleID
            where userRole.UserID == user.UserID && role.IsActive
            orderby role.RoleLevel
            select role.RoleCode)
            .Distinct()
            .ToArray();

        var permissionCodes = (
            from userRole in db.UserRoles
            join rolePermission in db.RolePermissions
                on userRole.RoleID equals rolePermission.RoleID
            join permission in db.Permissions
                on rolePermission.PermissionID equals permission.PermissionID
            where userRole.UserID == user.UserID &&
                  rolePermission.IsAllowed
            select permission.PermissionCode)
            .Distinct()
            .ToArray();

        return new CurrentUserInfo(
            user.UserID,
            employeeID,
            user.FullName,
            user.Email,
            roleCodes,
            permissionCodes);
    }

    public static AuthenticatedUserDto ToAuthenticatedUserDto(
        IApplicationDbContext db,
        User user)
    {
        var info = BuildCurrentUserInfo(db, user);

        return new AuthenticatedUserDto(
            user.UserID,
            info.EmployeeID,
            user.FullName,
            user.Email,
            user.MustChangePassword,
            info.RoleCodes,
            info.PermissionCodes);
    }
}
