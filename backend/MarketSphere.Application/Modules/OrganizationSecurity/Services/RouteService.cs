using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Mapping;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.OrganizationSecurity.DTOs;
using MarketSphere.Application.Modules.OrganizationSecurity.Interfaces;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;

namespace MarketSphere.Application.Modules.OrganizationSecurity.Services;

public sealed class RouteService : IRouteService
{
    private readonly IApplicationDbContext _db;

    public RouteService(IApplicationDbContext db)
    {
        _db = db;
    }

    public Task<IReadOnlyCollection<RouteDto>> GetRoutesAsync(
        int? territoryID = null,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Routes;

        if (territoryID.HasValue)
            query = query.Where(x => x.TerritoryID == territoryID.Value);

        IReadOnlyCollection<RouteDto> items = query
            .OrderBy(x => x.RouteName)
            .Select(x => new RouteDto(
                x.RouteID,
                x.TerritoryID,
                x.RouteCode,
                x.RouteName,
                x.DayOfWeek,
                x.VisitFrequency,
                x.IsActive))
            .ToArray();

        return Task.FromResult(items);
    }

    public async Task<int> CreateRouteAsync(
        CreateRouteRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (!_db.Territories.Any(
                x => x.TerritoryID == request.TerritoryID &&
                     x.IsActive))
        {
            throw new NotFoundException(
                $"Territory with ID {request.TerritoryID} was not found or is inactive.");
        }

        ValidationHelper.RequireNotBlank(
            request.RouteCode,
            nameof(request.RouteCode),
            50);

        ValidationHelper.RequireNotBlank(
            request.RouteName,
            nameof(request.RouteName),
            150);

        var code = request.RouteCode.NormalizeCode();

        if (_db.Routes.Any(
                x => x.TerritoryID == request.TerritoryID &&
                     x.RouteCode == code))
        {
            throw new ConflictException(
                $"Route code '{code}' already exists for this territory.");
        }

        var route = new Route
        {
            TerritoryID = request.TerritoryID,
            RouteCode = code,
            RouteName = request.RouteName.Trim(),
            DayOfWeek = request.DayOfWeek,
            VisitFrequency = request.VisitFrequency,
            IsActive = true
        };

        await _db.AddAsync(route, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return route.RouteID;
    }

    public async Task UpdateRouteAsync(
        int routeID,
        UpdateRouteRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var route = RequireRoute(routeID);

        ValidationHelper.RequireNotBlank(
            request.RouteName,
            nameof(request.RouteName),
            150);

        route.RouteName = request.RouteName.Trim();
        route.DayOfWeek = request.DayOfWeek;
        route.VisitFrequency = request.VisitFrequency;
        route.IsActive = request.IsActive;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<IReadOnlyCollection<RouteOutletDto>> GetRouteOutletsAsync(
        int routeID,
        CancellationToken cancellationToken = default)
    {
        RequireRoute(routeID);

        IReadOnlyCollection<RouteOutletDto> items =
            _db.RouteOutlets
                .Where(x => x.RouteID == routeID)
                .OrderBy(x => x.SequenceNo)
                .Select(x => new RouteOutletDto(
                    x.RouteOutletID,
                    x.RouteID,
                    x.ClientID,
                    x.SequenceNo,
                    x.VisitFrequency,
                    x.EffectiveFrom,
                    x.EffectiveTo))
                .ToArray();

        return Task.FromResult(items);
    }

    public async Task<int> AddRouteOutletAsync(
        CreateRouteOutletRequestDto request,
        CancellationToken cancellationToken = default)
    {
        RequireRoute(request.RouteID);
        ValidatePeriod(request.EffectiveFrom, request.EffectiveTo);

        if (request.SequenceNo <= 0)
        {
            throw new BusinessRuleException(
                "Route outlet sequence number must be greater than zero.");
        }

        if (request.ClientID <= 0)
        {
            throw new BusinessRuleException(
                "Client ID must be greater than zero.");
        }

        var overlaps = _db.RouteOutlets.Any(
            x => x.RouteID == request.RouteID &&
                 x.ClientID == request.ClientID &&
                 x.EffectiveFrom <=
                     (request.EffectiveTo ?? DateOnly.MaxValue) &&
                 (x.EffectiveTo ?? DateOnly.MaxValue) >=
                     request.EffectiveFrom);

        if (overlaps)
        {
            throw new ConflictException(
                "An overlapping route-outlet assignment already exists.");
        }

        if (_db.RouteOutlets.Any(
                x => x.RouteID == request.RouteID &&
                     x.SequenceNo == request.SequenceNo &&
                     x.EffectiveFrom <=
                         (request.EffectiveTo ?? DateOnly.MaxValue) &&
                     (x.EffectiveTo ?? DateOnly.MaxValue) >=
                         request.EffectiveFrom))
        {
            throw new ConflictException(
                "The route sequence is already used during the selected period.");
        }

        var routeOutlet = new RouteOutlet
        {
            RouteID = request.RouteID,
            ClientID = request.ClientID,
            SequenceNo = request.SequenceNo,
            VisitFrequency = request.VisitFrequency,
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo
        };

        await _db.AddAsync(routeOutlet, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return routeOutlet.RouteOutletID;
    }

    public async Task<int> AssignEmployeeRouteAsync(
        CreateEmployeeRouteAssignmentRequestDto request,
        CancellationToken cancellationToken = default)
    {
        RequireActiveEmployee(request.EmployeeID);
        RequireRoute(request.RouteID);
        ValidatePeriod(request.EffectiveFrom, request.EffectiveTo);

        var overlaps = _db.EmployeeRouteAssignments.Any(
            x => x.EmployeeID == request.EmployeeID &&
                 x.RouteID == request.RouteID &&
                 x.Status == AssignmentStatus.Active &&
                 x.EffectiveFrom <=
                     (request.EffectiveTo ?? DateOnly.MaxValue) &&
                 (x.EffectiveTo ?? DateOnly.MaxValue) >=
                     request.EffectiveFrom);

        if (overlaps)
        {
            throw new ConflictException(
                "An overlapping employee route assignment already exists.");
        }

        if (request.IsPrimary &&
            _db.EmployeeRouteAssignments.Any(
                x => x.EmployeeID == request.EmployeeID &&
                     x.IsPrimary &&
                     x.Status == AssignmentStatus.Active &&
                     x.EffectiveFrom <=
                         (request.EffectiveTo ?? DateOnly.MaxValue) &&
                     (x.EffectiveTo ?? DateOnly.MaxValue) >=
                         request.EffectiveFrom &&
                     (x.DayOfWeek == request.DayOfWeek ||
                      x.DayOfWeek == null ||
                      request.DayOfWeek == null)))
        {
            throw new ConflictException(
                "An overlapping primary route assignment already exists.");
        }

        var assignment = new EmployeeRouteAssignment
        {
            EmployeeID = request.EmployeeID,
            RouteID = request.RouteID,
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo,
            DayOfWeek = request.DayOfWeek,
            IsPrimary = request.IsPrimary,
            Status = AssignmentStatus.Active
        };

        await _db.AddAsync(assignment, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return assignment.EmployeeRouteAssignmentID;
    }

    public async Task<int> AssignEmployeeTerritoryAsync(
        CreateEmployeeTerritoryAssignmentRequestDto request,
        CancellationToken cancellationToken = default)
    {
        RequireActiveEmployee(request.EmployeeID);
        ValidatePeriod(request.EffectiveFrom, request.EffectiveTo);
        ValidateScope(request);

        var overlaps = _db.EmployeeTerritoryAssignments.Any(
            x => x.EmployeeID == request.EmployeeID &&
                 x.ScopeType == request.ScopeType &&
                 x.RegionID == request.RegionID &&
                 x.AreaID == request.AreaID &&
                 x.TerritoryID == request.TerritoryID &&
                 x.EffectiveFrom <=
                     (request.EffectiveTo ?? DateOnly.MaxValue) &&
                 (x.EffectiveTo ?? DateOnly.MaxValue) >=
                     request.EffectiveFrom);

        if (overlaps)
        {
            throw new ConflictException(
                "An overlapping employee territory assignment already exists.");
        }

        if (request.IsPrimary &&
            _db.EmployeeTerritoryAssignments.Any(
                x => x.EmployeeID == request.EmployeeID &&
                     x.IsPrimary &&
                     x.EffectiveFrom <=
                         (request.EffectiveTo ?? DateOnly.MaxValue) &&
                     (x.EffectiveTo ?? DateOnly.MaxValue) >=
                         request.EffectiveFrom))
        {
            throw new ConflictException(
                "An overlapping primary territory assignment already exists.");
        }

        var assignment = new EmployeeTerritoryAssignment
        {
            EmployeeID = request.EmployeeID,
            ScopeType = request.ScopeType,
            RegionID = request.RegionID,
            AreaID = request.AreaID,
            TerritoryID = request.TerritoryID,
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo,
            IsPrimary = request.IsPrimary
        };

        await _db.AddAsync(assignment, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return assignment.EmployeeTerritoryAssignmentID;
    }

    private Route RequireRoute(int routeID) =>
        _db.Routes.FirstOrDefault(
            x => x.RouteID == routeID &&
                 x.IsActive)
        ?? throw new NotFoundException(
            $"Route with ID {routeID} was not found or is inactive.");

    private void RequireActiveEmployee(int employeeID)
    {
        if (!_db.Employees.Any(
                x => x.EmployeeID == employeeID &&
                     x.Status == EmployeeStatus.Active))
        {
            throw new NotFoundException(
                $"Employee with ID {employeeID} was not found or is inactive.");
        }
    }

    private void ValidateScope(
        CreateEmployeeTerritoryAssignmentRequestDto request)
    {
        var selectedCount = new[]
        {
            request.RegionID.HasValue,
            request.AreaID.HasValue,
            request.TerritoryID.HasValue
        }.Count(x => x);

        if (selectedCount != 1)
        {
            throw new BusinessRuleException(
                "Exactly one geography identifier must be supplied.");
        }

        switch (request.ScopeType)
        {
            case GeographyScopeType.Region
                when request.RegionID.HasValue:
                if (!_db.Regions.Any(
                        x => x.RegionID == request.RegionID.Value &&
                             x.IsActive))
                {
                    throw new NotFoundException(
                        "The selected region was not found or is inactive.");
                }
                break;

            case GeographyScopeType.Area
                when request.AreaID.HasValue:
                if (!_db.Areas.Any(
                        x => x.AreaID == request.AreaID.Value &&
                             x.IsActive))
                {
                    throw new NotFoundException(
                        "The selected area was not found or is inactive.");
                }
                break;

            case GeographyScopeType.Territory
                when request.TerritoryID.HasValue:
                if (!_db.Territories.Any(
                        x => x.TerritoryID == request.TerritoryID.Value &&
                             x.IsActive))
                {
                    throw new NotFoundException(
                        "The selected territory was not found or is inactive.");
                }
                break;

            default:
                throw new BusinessRuleException(
                    "The geography identifier does not match the selected scope type.");
        }
    }

    private static void ValidatePeriod(
        DateOnly effectiveFrom,
        DateOnly? effectiveTo)
    {
        if (effectiveTo.HasValue &&
            effectiveTo.Value < effectiveFrom)
        {
            throw new BusinessRuleException(
                "Effective-to date cannot be earlier than effective-from date.");
        }
    }
}
