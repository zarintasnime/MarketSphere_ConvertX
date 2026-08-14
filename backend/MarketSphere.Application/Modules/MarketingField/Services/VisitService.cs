using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Mapping;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Security;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.MarketingField.DTOs;
using MarketSphere.Application.Modules.MarketingField.Interfaces;
using MarketSphere.Domain.Entities.MarketingField;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.MarketingField.Services;

public sealed class VisitService : IVisitService
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;
    private readonly ICurrentUserService _currentUser;

    public VisitService(
        IApplicationDbContext db,
        IDateTimeProvider clock,
        ICurrentUserService currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    public Task<PagedResult<VisitListDto>> GetPagedAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Visits.AsNoTracking();

        if (_currentUser.IsFieldUser())
        {
            var employeeID = _currentUser.RequireEmployeeID();
            query = query.Where(x => x.EmployeeID == employeeID);
        }

        var projected = query
            .OrderByDescending(x => x.CheckInAt)
            .Select(x => new VisitListDto(
                x.VisitID,
                x.EmployeeID,
                x.ClientID,
                x.RouteID,
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

    public async Task<VisitDetailsDto> GetByIdAsync(
        int visitID,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Visits
            .AsNoTracking()
            .Where(x => x.VisitID == visitID);

        if (_currentUser.IsFieldUser())
        {
            var employeeID = _currentUser.RequireEmployeeID();
            query = query.Where(x => x.EmployeeID == employeeID);
        }

        return await query
            .Select(x => new VisitDetailsDto(
                x.VisitID,
                x.EmployeeID,
                x.ClientID,
                x.RouteID,
                x.CampaignID,
                x.VisitType,
                x.CheckInAt,
                x.CheckOutAt,
                x.CheckInGPSLat,
                x.CheckInGPSLng,
                x.CheckOutGPSLat,
                x.CheckOutGPSLng,
                x.AccuracyMeters,
                x.IsSuspiciousLocation,
                x.Note,
                x.Status))
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Visit was not found.");
    }

    public async Task<int> CheckInAsync(
        CheckInVisitRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var employeeID = _currentUser.ResolveFieldEmployeeID(
            request.EmployeeID);

        ValidationHelper.Require(
            employeeID > 0,
            nameof(request.EmployeeID),
            "EmployeeID must be greater than zero.");

        ValidationHelper.Require(
            request.ClientID > 0,
            nameof(request.ClientID),
            "ClientID must be greater than zero.");

        MarketingServiceHelper.ValidateGps(
            request.CheckInGPSLat,
            request.CheckInGPSLng);

        if (request.AccuracyMeters.HasValue)
        {
            ValidationHelper.Require(
                request.AccuracyMeters > 0,
                nameof(request.AccuracyMeters),
                "Accuracy must be greater than zero.");
        }

        await ValidateReferencesAsync(
            request,
            employeeID,
            cancellationToken);

        if (await _db.Visits.AnyAsync(
                x => x.EmployeeID == employeeID &&
                     x.Status == VisitStatus.CheckedIn,
                cancellationToken))
        {
            throw new ConflictException(
                "The employee already has an open visit.");
        }

        var checkInAt = request.CheckInAt ?? _clock.UtcNow;

        var entity = new Visit
        {
            EmployeeID = employeeID,
            ClientID = request.ClientID,
            RouteID = request.RouteID,
            CampaignID = request.CampaignID,
            VisitType = request.VisitType,
            CheckInAt = checkInAt,
            CheckInGPSLat = request.CheckInGPSLat,
            CheckInGPSLng = request.CheckInGPSLng,
            AccuracyMeters = request.AccuracyMeters,
            IsSuspiciousLocation =
                request.AccuracyMeters.HasValue &&
                request.AccuracyMeters > 100,
            Note = request.Note.NullIfWhiteSpace(),
            Status = VisitStatus.CheckedIn
        };

        await _db.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return entity.VisitID;
    }

    public async Task CheckOutAsync(
        int visitID,
        CheckOutVisitRequestDto request,
        CancellationToken cancellationToken = default)
    {
        MarketingServiceHelper.ValidateGps(
            request.CheckOutGPSLat,
            request.CheckOutGPSLng);

        var entity = await MarketingServiceHelper.RequireAsync(
            _db.Visits.Where(x => x.VisitID == visitID),
            "Visit",
            cancellationToken);

        _currentUser.EnsureFieldRecordOwnership(entity.EmployeeID);

        if (entity.Status != VisitStatus.CheckedIn)
        {
            throw new BusinessRuleException(
                "Only a checked-in visit can be completed.");
        }

        var checkOutAt = request.CheckOutAt ?? _clock.UtcNow;

        ValidationHelper.Require(
            checkOutAt >= entity.CheckInAt,
            nameof(request.CheckOutAt),
            "Checkout time must be on or after check-in time.");

        entity.CheckOutAt = checkOutAt;
        entity.CheckOutGPSLat = request.CheckOutGPSLat;
        entity.CheckOutGPSLng = request.CheckOutGPSLng;
        entity.Note = request.Note.NullIfWhiteSpace() ?? entity.Note;
        entity.Status = VisitStatus.Completed;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task CancelAsync(
        int visitID,
        CancelVisitRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidationHelper.RequireNotBlank(
            request.Reason,
            nameof(request.Reason),
            500);

        var entity = await MarketingServiceHelper.RequireAsync(
            _db.Visits.Where(x => x.VisitID == visitID),
            "Visit",
            cancellationToken);

        _currentUser.EnsureFieldRecordOwnership(entity.EmployeeID);

        if (entity.Status != VisitStatus.CheckedIn)
        {
            throw new BusinessRuleException(
                "Only a checked-in visit can be cancelled.");
        }

        entity.Status = VisitStatus.Cancelled;
        entity.Note = request.Reason.Trim();
        entity.CheckOutAt = _clock.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateReferencesAsync(
        CheckInVisitRequestDto request,
        int employeeID,
        CancellationToken cancellationToken)
    {
        if (!await _db.Employees.AnyAsync(
                x => x.EmployeeID == employeeID &&
                     x.Status == EmployeeStatus.Active,
                cancellationToken))
        {
            throw new NotFoundException(
                "Active employee was not found.");
        }

        if (!await _db.Clients.AnyAsync(
                x => x.ClientID == request.ClientID && x.IsActive,
                cancellationToken))
        {
            throw new NotFoundException(
                "Active client was not found.");
        }

        if (request.RouteID.HasValue &&
            !await _db.Routes.AnyAsync(
                x => x.RouteID == request.RouteID.Value && x.IsActive,
                cancellationToken))
        {
            throw new NotFoundException(
                "Active route was not found.");
        }

        if (_currentUser.IsFieldUser())
        {
            await ValidateFieldClientScopeAsync(
                employeeID,
                request.ClientID,
                request.RouteID,
                cancellationToken);
        }

        if (request.CampaignID.HasValue)
        {
            var today = _clock.UtcToday;

            if (!await _db.Campaigns.AnyAsync(
                    x => x.CampaignID == request.CampaignID.Value &&
                         x.Status == CampaignStatus.Active &&
                         x.StartDate <= today &&
                         x.EndDate >= today,
                    cancellationToken))
            {
                throw new BusinessRuleException(
                    "The selected campaign is not active for the visit date.");
            }
        }
    }

    private async Task ValidateFieldClientScopeAsync(
        int employeeID,
        int clientID,
        int? routeID,
        CancellationToken cancellationToken)
    {
        var today = _clock.UtcToday;

        if (routeID.HasValue)
        {
            var assignedToRoute = await (
                from assignment in _db.EmployeeRouteAssignments
                join routeOutlet in _db.RouteOutlets
                    on assignment.RouteID equals routeOutlet.RouteID
                where assignment.EmployeeID == employeeID &&
                      assignment.RouteID == routeID.Value &&
                      routeOutlet.ClientID == clientID &&
                      assignment.Status == AssignmentStatus.Active &&
                      assignment.EffectiveFrom <= today &&
                      (!assignment.EffectiveTo.HasValue ||
                       assignment.EffectiveTo.Value >= today) &&
                      routeOutlet.EffectiveFrom <= today &&
                      (!routeOutlet.EffectiveTo.HasValue ||
                       routeOutlet.EffectiveTo.Value >= today)
                select routeOutlet.RouteOutletID)
                .AnyAsync(cancellationToken);

            if (!assignedToRoute)
            {
                throw new ForbiddenBusinessActionException(
                    "The selected client is not assigned to the current employee route.");
            }

            return;
        }

        var employee = await _db.Employees
            .AsNoTracking()
            .Where(x => x.EmployeeID == employeeID)
            .Select(x => new
            {
                x.RegionID,
                x.AreaID,
                x.TerritoryID
            })
            .SingleAsync(cancellationToken);

        var client = await _db.Clients
            .AsNoTracking()
            .Where(x => x.ClientID == clientID)
            .Select(x => new
            {
                x.RegionID,
                x.AreaID,
                x.TerritoryID
            })
            .SingleAsync(cancellationToken);

        var directScopeMatch =
            (employee.TerritoryID.HasValue &&
             employee.TerritoryID == client.TerritoryID) ||
            (employee.AreaID.HasValue &&
             employee.AreaID == client.AreaID) ||
            (employee.RegionID.HasValue &&
             employee.RegionID == client.RegionID);

        if (directScopeMatch)
            return;

        var assignedScopeMatch = await _db.EmployeeTerritoryAssignments
            .AsNoTracking()
            .AnyAsync(
                x => x.EmployeeID == employeeID &&
                     x.EffectiveFrom <= today &&
                     (!x.EffectiveTo.HasValue ||
                      x.EffectiveTo.Value >= today) &&
                     ((x.TerritoryID.HasValue &&
                       x.TerritoryID == client.TerritoryID) ||
                      (x.AreaID.HasValue &&
                       x.AreaID == client.AreaID) ||
                      (x.RegionID.HasValue &&
                       x.RegionID == client.RegionID)),
                cancellationToken);

        if (!assignedScopeMatch)
        {
            throw new ForbiddenBusinessActionException(
                "The selected client is outside the current employee scope.");
        }
    }
}
