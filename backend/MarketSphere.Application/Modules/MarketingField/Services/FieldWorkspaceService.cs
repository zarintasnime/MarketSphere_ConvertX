using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Security;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.MarketingField.DTOs;
using MarketSphere.Application.Modules.MarketingField.Interfaces;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.MarketingField.Services;

public sealed class FieldWorkspaceService : IFieldWorkspaceService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _clock;

    public FieldWorkspaceService(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IDateTimeProvider clock)
    {
        _db = db;
        _currentUser = currentUser;
        _clock = clock;
    }

    public async Task<FieldWorkspaceSummaryDto> GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var employeeID = _currentUser.RequireEmployeeID();
        var userID = _currentUser.RequireUserID();
        var today = _clock.UtcToday;
        var dayStart = today.ToDateTime(TimeOnly.MinValue);
        var dayEnd = today.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var employee = await _db.Employees
            .AsNoTracking()
            .Where(x => x.EmployeeID == employeeID)
            .Select(x => new
            {
                x.EmployeeID,
                x.EmployeeCode,
                EmployeeName = x.User != null
                    ? x.User.FullName
                    : x.EmployeeCode,
                x.Designation.DesignationName,
                x.BranchID,
                x.Branch.BranchName
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Employee was not found.");

        var assignedClients = await GetAssignedClientsAsync(
            new PagedRequest
            {
                PageNumber = 1,
                PageSize = 1
            },
            cancellationToken);

        var todayVisitCount = await _db.Visits
            .AsNoTracking()
            .CountAsync(
                x => x.EmployeeID == employeeID &&
                     x.CheckInAt >= dayStart &&
                     x.CheckInAt < dayEnd,
                cancellationToken);

        var completedVisitCount = await _db.Visits
            .AsNoTracking()
            .CountAsync(
                x => x.EmployeeID == employeeID &&
                     x.CheckInAt >= dayStart &&
                     x.CheckInAt < dayEnd &&
                     x.Status == VisitStatus.Completed,
                cancellationToken);

        var unreadNotificationCount = await _db.Notifications
            .AsNoTracking()
            .CountAsync(
                x => x.UserID == userID &&
                     !x.IsRead &&
                     (!x.ExpiresAt.HasValue || x.ExpiresAt > _clock.UtcNow),
                cancellationToken);

        return new FieldWorkspaceSummaryDto(
            employee.EmployeeID,
            employee.EmployeeCode,
            employee.EmployeeName,
            employee.DesignationName,
            employee.BranchID,
            employee.BranchName,
            assignedClients.TotalCount,
            todayVisitCount,
            completedVisitCount,
            unreadNotificationCount,
            await GetActiveVisitAsync(cancellationToken));
    }

    public async Task<PagedResult<FieldAssignedClientDto>> GetAssignedClientsAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        PaginationValidator.Validate(request);

        var employeeID = _currentUser.RequireEmployeeID();
        var today = _clock.UtcToday;
        var currentDay = today.DayOfWeek;

        var routeAssignments = await (
            from assignment in _db.EmployeeRouteAssignments.AsNoTracking()
            join route in _db.Routes.AsNoTracking()
                on assignment.RouteID equals route.RouteID
            where assignment.EmployeeID == employeeID &&
                  assignment.Status == AssignmentStatus.Active &&
                  assignment.EffectiveFrom <= today &&
                  (!assignment.EffectiveTo.HasValue ||
                   assignment.EffectiveTo.Value >= today) &&
                  (!assignment.DayOfWeek.HasValue ||
                   assignment.DayOfWeek.Value == currentDay) &&
                  route.IsActive
            select new
            {
                route.RouteID,
                route.RouteCode,
                route.RouteName
            })
            .ToListAsync(cancellationToken);

        var routeIDs = routeAssignments
            .Select(x => x.RouteID)
            .Distinct()
            .ToArray();

        var routeClients = routeIDs.Length == 0
            ? new List<FieldAssignedClientDto>()
            : await (
                from routeOutlet in _db.RouteOutlets.AsNoTracking()
                join route in _db.Routes.AsNoTracking()
                    on routeOutlet.RouteID equals route.RouteID
                join client in _db.Clients.AsNoTracking()
                    on routeOutlet.ClientID equals client.ClientID
                where routeIDs.Contains(routeOutlet.RouteID) &&
                      routeOutlet.EffectiveFrom <= today &&
                      (!routeOutlet.EffectiveTo.HasValue ||
                       routeOutlet.EffectiveTo.Value >= today) &&
                      client.IsActive
                select new FieldAssignedClientDto(
                    client.ClientID,
                    client.ClientCode,
                    client.ClientName,
                    client.ClientType,
                    client.Channel,
                    client.Phone,
                    client.Address,
                    client.GPSLat,
                    client.GPSLng,
                    client.RegionID,
                    client.AreaID,
                    client.TerritoryID,
                    route.RouteID,
                    route.RouteCode,
                    route.RouteName,
                    routeOutlet.SequenceNo))
                .ToListAsync(cancellationToken);

        var employeeScope = await _db.Employees
            .AsNoTracking()
            .Where(x => x.EmployeeID == employeeID)
            .Select(x => new
            {
                x.RegionID,
                x.AreaID,
                x.TerritoryID
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Employee was not found.");

        var assignedScopes = await _db.EmployeeTerritoryAssignments
            .AsNoTracking()
            .Where(x =>
                x.EmployeeID == employeeID &&
                x.EffectiveFrom <= today &&
                (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= today))
            .Select(x => new
            {
                x.RegionID,
                x.AreaID,
                x.TerritoryID
            })
            .ToListAsync(cancellationToken);

        var regionIDs = assignedScopes
            .Select(x => x.RegionID)
            .Append(employeeScope.RegionID)
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .Distinct()
            .ToArray();

        var areaIDs = assignedScopes
            .Select(x => x.AreaID)
            .Append(employeeScope.AreaID)
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .Distinct()
            .ToArray();

        var territoryIDs = assignedScopes
            .Select(x => x.TerritoryID)
            .Append(employeeScope.TerritoryID)
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .Distinct()
            .ToArray();

        var geographyClients =
            regionIDs.Length == 0 &&
            areaIDs.Length == 0 &&
            territoryIDs.Length == 0
                ? new List<FieldAssignedClientDto>()
                : await _db.Clients
                    .AsNoTracking()
                    .Where(x =>
                        x.IsActive &&
                        ((x.RegionID.HasValue &&
                          regionIDs.Contains(x.RegionID.Value)) ||
                         (x.AreaID.HasValue &&
                          areaIDs.Contains(x.AreaID.Value)) ||
                         (x.TerritoryID.HasValue &&
                          territoryIDs.Contains(x.TerritoryID.Value))))
                    .Select(x => new FieldAssignedClientDto(
                        x.ClientID,
                        x.ClientCode,
                        x.ClientName,
                        x.ClientType,
                        x.Channel,
                        x.Phone,
                        x.Address,
                        x.GPSLat,
                        x.GPSLng,
                        x.RegionID,
                        x.AreaID,
                        x.TerritoryID,
                        null,
                        null,
                        null,
                        null))
                    .ToListAsync(cancellationToken);

        var merged = routeClients
            .Concat(geographyClients)
            .GroupBy(x => x.ClientID)
            .Select(group => group
                .OrderByDescending(x => x.RouteID.HasValue)
                .ThenBy(x => x.SequenceNo ?? int.MaxValue)
                .First())
            .AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            merged = merged.Where(x =>
                x.ClientCode.Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase) ||
                x.ClientName.Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase) ||
                (x.Phone?.Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase) ?? false));
        }

        var ordered = merged
            .OrderBy(x => x.RouteName ?? string.Empty)
            .ThenBy(x => x.SequenceNo ?? int.MaxValue)
            .ThenBy(x => x.ClientName)
            .ToList();

        var totalCount = ordered.Count;
        var items = ordered
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToArray();

        return PagedResult<FieldAssignedClientDto>.Create(
            items,
            totalCount,
            request.PageNumber,
            request.PageSize);
    }

    public Task<PagedResult<FieldVisitListDto>> GetMyVisitsAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var employeeID = _currentUser.RequireEmployeeID();
        var query = _db.Visits
            .AsNoTracking()
            .Where(x => x.EmployeeID == employeeID);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x =>
                x.Client.ClientCode.Contains(search) ||
                x.Client.ClientName.Contains(search));
        }

        var projected = query
            .OrderByDescending(x => x.CheckInAt)
            .Select(x => new FieldVisitListDto(
                x.VisitID,
                x.ClientID,
                x.Client.ClientCode,
                x.Client.ClientName,
                x.RouteID,
                x.Route == null ? null : x.Route.RouteName,
                x.CampaignID,
                x.VisitType,
                x.CheckInAt,
                x.CheckOutAt,
                x.Status,
                x.IsSuspiciousLocation));

        return MarketingServiceHelper.ToPagedAsync(
            projected,
            request,
            cancellationToken);
    }

    public async Task<FieldActiveVisitDto?> GetActiveVisitAsync(
        CancellationToken cancellationToken = default)
    {
        var employeeID = _currentUser.RequireEmployeeID();

        return await _db.Visits
            .AsNoTracking()
            .Where(x =>
                x.EmployeeID == employeeID &&
                x.Status == VisitStatus.CheckedIn)
            .OrderByDescending(x => x.CheckInAt)
            .Select(x => new FieldActiveVisitDto(
                x.VisitID,
                x.EmployeeID,
                x.ClientID,
                x.Client.ClientCode,
                x.Client.ClientName,
                x.RouteID,
                x.Route == null ? null : x.Route.RouteName,
                x.CampaignID,
                x.VisitType,
                x.CheckInAt,
                x.CheckInGPSLat,
                x.CheckInGPSLng,
                x.AccuracyMeters,
                x.IsSuspiciousLocation,
                x.Note))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
