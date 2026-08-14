using MarketSphere.Application.Common.Interfaces;
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

public sealed class BPSellOutService : IBPSellOutService
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;
    private readonly ICurrentUserService _currentUser;

    public BPSellOutService(
        IApplicationDbContext db,
        IDateTimeProvider clock,
        ICurrentUserService currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    public Task<PagedResult<BPSellOutListDto>> GetPagedAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.BPSellOuts.AsNoTracking();

        if (_currentUser.IsFieldUser())
        {
            var employeeID = _currentUser.RequireEmployeeID();
            query = query.Where(x => x.EmployeeID == employeeID);
        }

        var projected = query
            .OrderByDescending(x => x.SellOutDate)
            .ThenByDescending(x => x.BPSellOutID)
            .Select(x => new BPSellOutListDto(
                x.BPSellOutID,
                x.EmployeeID,
                x.ClientID,
                x.VisitID,
                x.CampaignID,
                x.SellOutDate,
                x.TotalQuantity,
                x.TotalValue,
                x.VerificationStatus,
                x.VerifiedByEmployeeID,
                x.VerifiedAt));

        return MarketingServiceHelper.ToPagedAsync(
            projected,
            request,
            cancellationToken);
    }

    public async Task<BPSellOutDetailsDto> GetByIdAsync(
        int bpSellOutID,
        CancellationToken cancellationToken = default)
    {
        var query = _db.BPSellOuts
            .AsNoTracking()
            .Where(x => x.BPSellOutID == bpSellOutID);

        if (_currentUser.IsFieldUser())
        {
            var employeeID = _currentUser.RequireEmployeeID();
            query = query.Where(x => x.EmployeeID == employeeID);
        }

        var entity = await query.SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("BP sell-out was not found.");

        var items = await _db.BPSellOutItems
            .AsNoTracking()
            .Where(x => x.BPSellOutID == bpSellOutID)
            .OrderBy(x => x.BPSellOutItemID)
            .Select(x => new BPSellOutItemDto(
                x.BPSellOutItemID,
                x.SKUID,
                x.QuantitySold,
                x.UnitSellingPrice,
                x.LineValue))
            .ToListAsync(cancellationToken);

        return new BPSellOutDetailsDto(
            entity.BPSellOutID,
            entity.EmployeeID,
            entity.ClientID,
            entity.VisitID,
            entity.CampaignID,
            entity.SellOutDate,
            entity.TotalQuantity,
            entity.TotalValue,
            entity.GPSLat,
            entity.GPSLng,
            entity.VerificationStatus,
            entity.VerifiedByEmployeeID,
            entity.VerifiedAt,
            items);
    }

    public async Task<int> CreateAsync(
        SaveBPSellOutRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var employeeID = _currentUser.ResolveFieldEmployeeID(
            request.EmployeeID);

        Validate(request, employeeID);

        await ValidateReferencesAsync(
            request,
            employeeID,
            cancellationToken);

        return await _db.ExecuteInTransactionAsync(
            async ct =>
            {
                var entity = new BPSellOut
                {
                    EmployeeID = employeeID,
                    ClientID = request.ClientID,
                    VisitID = request.VisitID,
                    CampaignID = request.CampaignID,
                    SellOutDate = request.SellOutDate,
                    GPSLat = request.GPSLat,
                    GPSLng = request.GPSLng,
                    VerificationStatus = VerificationStatus.Pending
                };

                await _db.AddAsync(entity, ct);
                await _db.SaveChangesAsync(ct);

                await ReplaceItemsAsync(entity, request.Items, ct);
                await _db.SaveChangesAsync(ct);

                return entity.BPSellOutID;
            },
            cancellationToken);
    }

    public async Task UpdateAsync(
        int bpSellOutID,
        SaveBPSellOutRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var employeeID = _currentUser.ResolveFieldEmployeeID(
            request.EmployeeID);

        Validate(request, employeeID);

        await ValidateReferencesAsync(
            request,
            employeeID,
            cancellationToken);

        await _db.ExecuteInTransactionAsync(
            async ct =>
            {
                var entity = await MarketingServiceHelper.RequireAsync(
                    _db.BPSellOuts.Where(x => x.BPSellOutID == bpSellOutID),
                    "BP sell-out",
                    ct);

                _currentUser.EnsureFieldRecordOwnership(entity.EmployeeID);

                if (entity.VerificationStatus != VerificationStatus.Pending)
                {
                    throw new BusinessRuleException(
                        "A verified or rejected sell-out cannot be edited.");
                }

                entity.EmployeeID = employeeID;
                entity.ClientID = request.ClientID;
                entity.VisitID = request.VisitID;
                entity.CampaignID = request.CampaignID;
                entity.SellOutDate = request.SellOutDate;
                entity.GPSLat = request.GPSLat;
                entity.GPSLng = request.GPSLng;

                var oldItems = await _db.BPSellOutItems
                    .Where(x => x.BPSellOutID == bpSellOutID)
                    .ToListAsync(ct);

                foreach (var item in oldItems)
                    _db.Remove(item);

                await _db.SaveChangesAsync(ct);
                await ReplaceItemsAsync(entity, request.Items, ct);
                await _db.SaveChangesAsync(ct);

                return 0;
            },
            cancellationToken);
    }

    public async Task VerifyAsync(
        int bpSellOutID,
        VerifyBPSellOutRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidationHelper.Require(
            request.VerificationStatus is
                VerificationStatus.Verified or
                VerificationStatus.Rejected,
            nameof(request.VerificationStatus),
            "Verification status must be Verified or Rejected.");

        if (!await _db.Employees.AnyAsync(
                x => x.EmployeeID == request.VerifiedByEmployeeID,
                cancellationToken))
        {
            throw new NotFoundException("Verifier employee was not found.");
        }

        var entity = await MarketingServiceHelper.RequireAsync(
            _db.BPSellOuts.Where(x => x.BPSellOutID == bpSellOutID),
            "BP sell-out",
            cancellationToken);

        if (entity.VerificationStatus != VerificationStatus.Pending)
        {
            throw new ConflictException(
                "The sell-out has already been reviewed.");
        }

        if (!await _db.BPSellOutItems.AnyAsync(
                x => x.BPSellOutID == bpSellOutID,
                cancellationToken))
        {
            throw new BusinessRuleException(
                "Sell-out items are required before verification.");
        }

        entity.VerificationStatus = request.VerificationStatus;
        entity.VerifiedByEmployeeID = request.VerifiedByEmployeeID;
        entity.VerifiedAt = _clock.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
    }

    private static void Validate(
        SaveBPSellOutRequestDto request,
        int employeeID)
    {
        ValidationHelper.Require(
            employeeID > 0 && request.ClientID > 0,
            nameof(request.EmployeeID),
            "EmployeeID and ClientID must be greater than zero.");

        MarketingServiceHelper.ValidateGps(
            request.GPSLat,
            request.GPSLng);

        ValidationHelper.Require(
            request.Items.Count > 0,
            nameof(request.Items),
            "At least one sell-out item is required.");

        ValidationHelper.Require(
            request.Items.All(x => x.SKUID > 0 && x.QuantitySold > 0),
            nameof(request.Items),
            "Every item must have a positive SKUID and quantity.");

        ValidationHelper.Require(
            request.Items.All(
                x => !x.UnitSellingPrice.HasValue ||
                     x.UnitSellingPrice.Value >= 0),
            nameof(request.Items),
            "Unit selling price cannot be negative.");

        ValidationHelper.Require(
            request.Items.Select(x => x.SKUID).Distinct().Count() ==
            request.Items.Count,
            nameof(request.Items),
            "Duplicate SKUs are not allowed.");
    }

    private async Task ValidateReferencesAsync(
        SaveBPSellOutRequestDto request,
        int employeeID,
        CancellationToken cancellationToken)
    {
        if (!await _db.Employees.AnyAsync(
                x => x.EmployeeID == employeeID &&
                     x.Status == EmployeeStatus.Active,
                cancellationToken))
        {
            throw new NotFoundException("Active employee was not found.");
        }

        if (!await _db.Clients.AnyAsync(
                x => x.ClientID == request.ClientID && x.IsActive,
                cancellationToken))
        {
            throw new NotFoundException("Active client was not found.");
        }

        if (request.CampaignID.HasValue &&
            !await _db.Campaigns.AnyAsync(
                x => x.CampaignID == request.CampaignID.Value,
                cancellationToken))
        {
            throw new NotFoundException("Campaign was not found.");
        }

        var skuIDs = request.Items
            .Select(x => x.SKUID)
            .Distinct()
            .ToArray();

        var activeSKUCount = await _db.SKUs.CountAsync(
            x => skuIDs.Contains(x.SKUID) && x.IsActive,
            cancellationToken);

        if (activeSKUCount != skuIDs.Length)
        {
            throw new NotFoundException(
                "One or more active SKUs were not found.");
        }

        if (request.VisitID.HasValue)
        {
            var visit = await _db.Visits
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    x => x.VisitID == request.VisitID.Value,
                    cancellationToken)
                ?? throw new NotFoundException("Visit was not found.");

            if (visit.EmployeeID != employeeID ||
                visit.ClientID != request.ClientID)
            {
                throw new BusinessRuleException(
                    "Sell-out employee and client must match the selected visit.");
            }
        }
    }

    private async Task ReplaceItemsAsync(
        BPSellOut entity,
        IReadOnlyCollection<SaveBPSellOutItemRequestDto> requests,
        CancellationToken cancellationToken)
    {
        var totalQuantity = 0m;
        var totalValue = 0m;

        foreach (var request in requests)
        {
            var lineValue = request.UnitSellingPrice.HasValue
                ? Math.Round(
                    request.QuantitySold * request.UnitSellingPrice.Value,
                    2)
                : (decimal?)null;

            await _db.AddAsync(
                new BPSellOutItem
                {
                    BPSellOutID = entity.BPSellOutID,
                    SKUID = request.SKUID,
                    QuantitySold = request.QuantitySold,
                    UnitSellingPrice = request.UnitSellingPrice,
                    LineValue = lineValue
                },
                cancellationToken);

            totalQuantity += request.QuantitySold;
            totalValue += lineValue ?? 0;
        }

        entity.TotalQuantity = totalQuantity;
        entity.TotalValue = totalValue;
    }
}
