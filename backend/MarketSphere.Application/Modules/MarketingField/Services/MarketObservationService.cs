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

public sealed class MarketObservationService : IMarketObservationService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public MarketObservationService(
        IApplicationDbContext db,
        ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public Task<PagedResult<MarketObservationListDto>> GetPagedAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.MarketObservations.AsNoTracking();

        if (_currentUser.IsFieldUser())
        {
            var employeeID = _currentUser.RequireEmployeeID();
            query = query.Where(x => x.EmployeeID == employeeID);
        }

        var projected = query
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new MarketObservationListDto(
                x.MarketObservationID,
                x.VisitID,
                x.ClientID,
                x.EmployeeID,
                x.ObservationType,
                x.SKUID,
                x.AvailabilityStatus,
                x.CompetitorBrand,
                x.CompetitorPrice));

        return MarketingServiceHelper.ToPagedAsync(
            projected,
            request,
            cancellationToken);
    }

    public async Task<MarketObservationDetailsDto> GetByIdAsync(
        int marketObservationID,
        CancellationToken cancellationToken = default)
    {
        var query = _db.MarketObservations
            .AsNoTracking()
            .Where(x => x.MarketObservationID == marketObservationID);

        if (_currentUser.IsFieldUser())
        {
            var employeeID = _currentUser.RequireEmployeeID();
            query = query.Where(x => x.EmployeeID == employeeID);
        }

        return await query
            .Select(x => new MarketObservationDetailsDto(
                x.MarketObservationID,
                x.VisitID,
                x.ClientID,
                x.EmployeeID,
                x.ObservationType,
                x.SKUID,
                x.AvailabilityStatus,
                x.FacingCount,
                x.PlanogramScore,
                x.DisplayScore,
                x.CompetitorBrand,
                x.CompetitorProduct,
                x.CompetitorPrice,
                x.CompetitorOffer,
                x.Note))
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(
                "Market observation was not found.");
    }

    public async Task<int> CreateAsync(
        SaveMarketObservationRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var employeeID = _currentUser.ResolveFieldEmployeeID(
            request.EmployeeID);

        Validate(request, employeeID);

        await ValidateReferencesAsync(
            request,
            employeeID,
            cancellationToken);

        var entity = new MarketObservation();
        Apply(entity, request, employeeID);

        await _db.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return entity.MarketObservationID;
    }

    public async Task UpdateAsync(
        int marketObservationID,
        SaveMarketObservationRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = await MarketingServiceHelper.RequireAsync(
            _db.MarketObservations.Where(
                x => x.MarketObservationID == marketObservationID),
            "Market observation",
            cancellationToken);

        _currentUser.EnsureFieldRecordOwnership(entity.EmployeeID);

        var employeeID = _currentUser.ResolveFieldEmployeeID(
            request.EmployeeID);

        Validate(request, employeeID);

        await ValidateReferencesAsync(
            request,
            employeeID,
            cancellationToken);

        Apply(entity, request, employeeID);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        int marketObservationID,
        CancellationToken cancellationToken = default)
    {
        var entity = await MarketingServiceHelper.RequireAsync(
            _db.MarketObservations.Where(
                x => x.MarketObservationID == marketObservationID),
            "Market observation",
            cancellationToken);

        _currentUser.EnsureFieldRecordOwnership(entity.EmployeeID);

        _db.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private static void Validate(
        SaveMarketObservationRequestDto request,
        int employeeID)
    {
        ValidationHelper.Require(
            request.VisitID > 0 &&
            request.ClientID > 0 &&
            employeeID > 0,
            nameof(request.VisitID),
            "VisitID, ClientID and EmployeeID must be greater than zero.");

        if (request.FacingCount.HasValue)
        {
            ValidationHelper.Require(
                request.FacingCount >= 0,
                nameof(request.FacingCount),
                "Facing count cannot be negative.");
        }

        MarketingServiceHelper.ValidateScore(
            request.PlanogramScore,
            nameof(request.PlanogramScore));

        MarketingServiceHelper.ValidateScore(
            request.DisplayScore,
            nameof(request.DisplayScore));

        if (request.CompetitorPrice.HasValue)
        {
            ValidationHelper.Require(
                request.CompetitorPrice >= 0,
                nameof(request.CompetitorPrice),
                "Competitor price cannot be negative.");
        }

        if (request.ObservationType == MarketObservationType.Availability)
        {
            ValidationHelper.Require(
                request.AvailabilityStatus.HasValue,
                nameof(request.AvailabilityStatus),
                "Availability status is required.");
        }

        if (request.ObservationType == MarketObservationType.Competitor)
        {
            ValidationHelper.Require(
                !string.IsNullOrWhiteSpace(request.CompetitorBrand) ||
                !string.IsNullOrWhiteSpace(request.CompetitorProduct),
                nameof(request.CompetitorBrand),
                "Competitor brand or product is required.");
        }
    }

    private async Task ValidateReferencesAsync(
        SaveMarketObservationRequestDto request,
        int employeeID,
        CancellationToken cancellationToken)
    {
        var visit = await _db.Visits
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.VisitID == request.VisitID,
                cancellationToken)
            ?? throw new NotFoundException("Visit was not found.");

        if (visit.ClientID != request.ClientID)
        {
            throw new BusinessRuleException(
                "Observation client must match the visit client.");
        }

        if (visit.EmployeeID != employeeID)
        {
            throw new BusinessRuleException(
                "Observation employee must match the visit employee.");
        }

        if (request.SKUID.HasValue &&
            !await _db.SKUs.AnyAsync(
                x => x.SKUID == request.SKUID.Value && x.IsActive,
                cancellationToken))
        {
            throw new NotFoundException("Active SKU was not found.");
        }
    }

    private static void Apply(
        MarketObservation entity,
        SaveMarketObservationRequestDto request,
        int employeeID)
    {
        entity.VisitID = request.VisitID;
        entity.ClientID = request.ClientID;
        entity.EmployeeID = employeeID;
        entity.ObservationType = request.ObservationType;
        entity.SKUID = request.SKUID;
        entity.AvailabilityStatus = request.AvailabilityStatus;
        entity.FacingCount = request.FacingCount;
        entity.PlanogramScore = request.PlanogramScore;
        entity.DisplayScore = request.DisplayScore;
        entity.CompetitorBrand = request.CompetitorBrand.NullIfWhiteSpace();
        entity.CompetitorProduct = request.CompetitorProduct.NullIfWhiteSpace();
        entity.CompetitorPrice = request.CompetitorPrice;
        entity.CompetitorOffer = request.CompetitorOffer.NullIfWhiteSpace();
        entity.Note = request.Note.NullIfWhiteSpace();
    }
}
