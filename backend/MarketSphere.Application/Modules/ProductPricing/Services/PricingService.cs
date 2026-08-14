using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Mapping;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.ProductPricing.DTOs;
using MarketSphere.Application.Modules.ProductPricing.Interfaces;
using MarketSphere.Domain.Entities.ProductPricing;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.ProductPricing.Services;

public sealed class PricingService : IPricingService
{
    private readonly IApplicationDbContext _db;

    public PricingService(IApplicationDbContext db) => _db = db;

    public Task<PagedResult<PriceListListDto>> GetPriceListsAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.PriceLists.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x =>
                x.PriceListCode.Contains(search) ||
                x.PriceListName.Contains(search) ||
                (x.ClientSegment != null && x.ClientSegment.SegmentName.Contains(search)));
        }

        var projected = query.OrderByDescending(x => x.EffectiveFrom)
            .ThenBy(x => x.PriceListName)
            .Select(x => new PriceListListDto(
                x.PriceListID,
                x.PriceListCode,
                x.PriceListName,
                x.Channel,
                x.ClientSegmentID,
                x.ClientSegment != null ? x.ClientSegment.SegmentName : null,
                x.EffectiveFrom,
                x.EffectiveTo,
                x.CurrencyCode,
                x.Status));
        return ProductPricingServiceHelper.ToPagedAsync(projected, request, cancellationToken);
    }

    public async Task<PriceListDetailsDto> GetPriceListByIdAsync(
        int priceListID,
        CancellationToken cancellationToken = default)
    {
        var header = await _db.PriceLists.AsNoTracking()
            .Where(x => x.PriceListID == priceListID)
            .Select(x => new
            {
                x.PriceListID,
                x.PriceListCode,
                x.PriceListName,
                x.Channel,
                x.ClientSegmentID,
                ClientSegmentName = x.ClientSegment != null ? x.ClientSegment.SegmentName : null,
                x.EffectiveFrom,
                x.EffectiveTo,
                x.CurrencyCode,
                x.Status
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Price list was not found.");

        var items = await _db.PriceListItems.AsNoTracking()
            .Where(x => x.PriceListID == priceListID)
            .OrderBy(x => x.SKU.SKUName)
            .Select(x => new PriceListItemDto(
                x.PriceListItemID,
                x.SKUID,
                x.SKU.SKUCode,
                x.SKU.SKUName,
                x.UnitPrice,
                x.MaximumDiscountPercent,
                x.MinimumOrderQuantity))
            .ToListAsync(cancellationToken);

        return new PriceListDetailsDto(
            header.PriceListID,
            header.PriceListCode,
            header.PriceListName,
            header.Channel,
            header.ClientSegmentID,
            header.ClientSegmentName,
            header.EffectiveFrom,
            header.EffectiveTo,
            header.CurrencyCode,
            header.Status,
            items);
    }

    public async Task<int> CreatePriceListAsync(
        SavePriceListRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidatePriceList(request);
        await ValidatePriceListReferencesAsync(request, cancellationToken);
        var code = request.PriceListCode.NormalizeCode();
        if (await _db.PriceLists.AnyAsync(x => x.PriceListCode == code, cancellationToken))
            throw new ConflictException("Price list code already exists.");

        return await _db.ExecuteInTransactionAsync(async ct =>
        {
            var entity = new PriceList { Status = PriceListStatus.Draft };
            ApplyPriceList(entity, request, code);
            await _db.AddAsync(entity, ct);
            await _db.SaveChangesAsync(ct);
            await ReplacePriceListItemsAsync(entity.PriceListID, request.Items, ct);
            await _db.SaveChangesAsync(ct);
            return entity.PriceListID;
        }, cancellationToken);
    }

    public async Task UpdatePriceListAsync(
        int priceListID,
        SavePriceListRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidatePriceList(request);
        await ValidatePriceListReferencesAsync(request, cancellationToken);
        await _db.ExecuteInTransactionAsync(async ct =>
        {
            var entity = await ProductPricingServiceHelper.RequireAsync(
                _db.PriceLists.Where(x => x.PriceListID == priceListID),
                "Price list",
                ct);
            if (entity.Status != PriceListStatus.Draft)
                throw new BusinessRuleException("Only draft price lists can be edited.");

            var code = request.PriceListCode.NormalizeCode();
            if (await _db.PriceLists.AnyAsync(
                    x => x.PriceListCode == code && x.PriceListID != priceListID,
                    ct))
                throw new ConflictException("Price list code already exists.");

            ApplyPriceList(entity, request, code);
            var existing = await _db.PriceListItems.Where(x => x.PriceListID == priceListID).ToListAsync(ct);
            foreach (var item in existing)
                _db.Remove(item);
            await _db.SaveChangesAsync(ct);
            await ReplacePriceListItemsAsync(priceListID, request.Items, ct);
            await _db.SaveChangesAsync(ct);
            return 0;
        }, cancellationToken);
    }

    public async Task ChangePriceListStatusAsync(
        int priceListID,
        ChangePriceListStatusRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = await ProductPricingServiceHelper.RequireAsync(
            _db.PriceLists.Where(x => x.PriceListID == priceListID),
            "Price list",
            cancellationToken);
        if (entity.Status == request.Status)
            return;

        var allowed = entity.Status switch
        {
            PriceListStatus.Draft => request.Status is PriceListStatus.Active or PriceListStatus.Inactive,
            PriceListStatus.Active => request.Status is PriceListStatus.Inactive or PriceListStatus.Expired,
            PriceListStatus.Inactive => request.Status is PriceListStatus.Active or PriceListStatus.Expired,
            _ => false
        };
        if (!allowed)
            throw new BusinessRuleException($"Price list cannot move from {entity.Status} to {request.Status}.");

        if (request.Status == PriceListStatus.Active)
        {
            if (!await _db.PriceListItems.AnyAsync(x => x.PriceListID == priceListID, cancellationToken))
                throw new BusinessRuleException("A price list must contain at least one item before activation.");
            await EnsureNoActivePriceListOverlapAsync(entity, cancellationToken);
        }

        entity.Status = request.Status;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<PagedResult<StandardDiscountRuleDto>> GetDiscountRulesAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.StandardDiscountRules.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x => x.RuleName.Contains(search));
        }
        var projected = query.OrderByDescending(x => x.IsActive)
            .ThenByDescending(x => x.EffectiveFrom)
            .ThenBy(x => x.RuleName)
            .Select(x => new StandardDiscountRuleDto(
                x.StandardDiscountRuleID,
                x.RuleName,
                x.Channel,
                x.ClientSegmentID,
                x.SKUID,
                x.ProductCategoryID,
                x.MinQuantity,
                x.MaxDiscountPercent,
                x.RequiresApproval,
                x.EffectiveFrom,
                x.EffectiveTo,
                x.IsActive));
        return ProductPricingServiceHelper.ToPagedAsync(projected, request, cancellationToken);
    }

    public async Task<int> CreateDiscountRuleAsync(
        SaveStandardDiscountRuleRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidateDiscountRule(request);
        await ValidateDiscountRuleReferencesAsync(request, cancellationToken);
        if (request.IsActive)
            await EnsureNoDiscountRuleOverlapAsync(null, request, cancellationToken);

        var entity = new StandardDiscountRule();
        ApplyDiscountRule(entity, request);
        await _db.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.StandardDiscountRuleID;
    }

    public async Task UpdateDiscountRuleAsync(
        int standardDiscountRuleID,
        SaveStandardDiscountRuleRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidateDiscountRule(request);
        await ValidateDiscountRuleReferencesAsync(request, cancellationToken);
        if (request.IsActive)
            await EnsureNoDiscountRuleOverlapAsync(standardDiscountRuleID, request, cancellationToken);

        var entity = await ProductPricingServiceHelper.RequireAsync(
            _db.StandardDiscountRules.Where(x => x.StandardDiscountRuleID == standardDiscountRuleID),
            "Standard discount rule",
            cancellationToken);
        ApplyDiscountRule(entity, request);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task SetDiscountRuleStatusAsync(
        int standardDiscountRuleID,
        ChangeMasterStatusRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = await ProductPricingServiceHelper.RequireAsync(
            _db.StandardDiscountRules.Where(x => x.StandardDiscountRuleID == standardDiscountRuleID),
            "Standard discount rule",
            cancellationToken);

        if (request.IsActive && !entity.IsActive)
        {
            var dto = new SaveStandardDiscountRuleRequestDto
            {
                RuleName = entity.RuleName,
                Channel = entity.Channel,
                ClientSegmentID = entity.ClientSegmentID,
                SKUID = entity.SKUID,
                ProductCategoryID = entity.ProductCategoryID,
                MinQuantity = entity.MinQuantity,
                MaxDiscountPercent = entity.MaxDiscountPercent,
                RequiresApproval = entity.RequiresApproval,
                EffectiveFrom = entity.EffectiveFrom,
                EffectiveTo = entity.EffectiveTo,
                IsActive = true
            };
            await EnsureNoDiscountRuleOverlapAsync(standardDiscountRuleID, dto, cancellationToken);
        }

        entity.IsActive = request.IsActive;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<PriceResolutionDto> ResolvePriceAsync(
        PriceResolutionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidationHelper.Require(request.SKUID > 0, nameof(request.SKUID), "SKUID must be greater than zero.");
        ValidationHelper.Require(request.Quantity > 0, nameof(request.Quantity), "Quantity must be greater than zero.");
        if (!await _db.SKUs.AnyAsync(x => x.SKUID == request.SKUID && x.IsActive && x.Product.IsActive, cancellationToken))
            throw new NotFoundException("Active SKU was not found.");
        if (request.ClientSegmentID.HasValue && !await _db.ClientSegments.AnyAsync(
                x => x.ClientSegmentID == request.ClientSegmentID && x.IsActive,
                cancellationToken))
            throw new NotFoundException("Active client segment was not found.");

        var candidate = await _db.PriceListItems.AsNoTracking()
            .Where(x =>
                x.SKUID == request.SKUID &&
                x.PriceList.Status == PriceListStatus.Active &&
                x.PriceList.Channel == request.Channel &&
                x.PriceList.EffectiveFrom <= request.PriceDate &&
                (!x.PriceList.EffectiveTo.HasValue || x.PriceList.EffectiveTo.Value >= request.PriceDate) &&
                (!x.MinimumOrderQuantity.HasValue || request.Quantity >= x.MinimumOrderQuantity.Value) &&
                (x.PriceList.ClientSegmentID == request.ClientSegmentID || x.PriceList.ClientSegmentID == null))
            .OrderByDescending(x => x.PriceList.ClientSegmentID.HasValue)
            .ThenByDescending(x => x.PriceList.EffectiveFrom)
            .Select(x => new
            {
                x.PriceListItemID,
                x.PriceListID,
                x.UnitPrice,
                x.MaximumDiscountPercent,
                x.PriceList.CurrencyCode
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BusinessRuleException(BusinessRuleMessages.PriceListIsNotApplicable);

        var productCategoryID = await _db.SKUs.AsNoTracking()
            .Where(x => x.SKUID == request.SKUID)
            .Select(x => x.Product.ProductCategoryID)
            .SingleAsync(cancellationToken);

        var rules = await _db.StandardDiscountRules.AsNoTracking()
            .Where(x =>
                x.IsActive &&
                x.Channel == request.Channel &&
                x.EffectiveFrom <= request.PriceDate &&
                (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= request.PriceDate) &&
                (!x.MinQuantity.HasValue || request.Quantity >= x.MinQuantity.Value) &&
                (!x.ClientSegmentID.HasValue || x.ClientSegmentID == request.ClientSegmentID) &&
                (!x.SKUID.HasValue || x.SKUID == request.SKUID) &&
                (!x.ProductCategoryID.HasValue || x.ProductCategoryID == productCategoryID))
            .Select(x => new
            {
                Entity = x,
                Specificity = (x.SKUID.HasValue ? 4 : 0) +
                              (x.ProductCategoryID.HasValue ? 2 : 0) +
                              (x.ClientSegmentID.HasValue ? 1 : 0)
            })
            .OrderByDescending(x => x.Specificity)
            .ThenByDescending(x => x.Entity.MinQuantity ?? 0)
            .ThenByDescending(x => x.Entity.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);

        var ruleDiscount = rules?.Entity.MaxDiscountPercent ?? 0m;
        var effectiveMaximum = rules is null
            ? candidate.MaximumDiscountPercent
            : Math.Min(candidate.MaximumDiscountPercent, ruleDiscount);
        return new PriceResolutionDto(
            request.SKUID,
            candidate.PriceListID,
            candidate.PriceListItemID,
            candidate.UnitPrice,
            candidate.MaximumDiscountPercent,
            ruleDiscount,
            effectiveMaximum,
            rules?.Entity.RequiresApproval ?? false,
            rules?.Entity.StandardDiscountRuleID,
            candidate.CurrencyCode);
    }

    private async Task ValidatePriceListReferencesAsync(
        SavePriceListRequestDto request,
        CancellationToken cancellationToken)
    {
        if (request.ClientSegmentID.HasValue && !await _db.ClientSegments.AnyAsync(
                x => x.ClientSegmentID == request.ClientSegmentID && x.IsActive,
                cancellationToken))
            throw new NotFoundException("Active client segment was not found.");

        var skuIDs = request.Items.Select(x => x.SKUID).Distinct().ToArray();
        var existingCount = await _db.SKUs.CountAsync(
            x => skuIDs.Contains(x.SKUID) && x.IsActive && x.Product.IsActive,
            cancellationToken);
        if (existingCount != skuIDs.Length)
            throw new NotFoundException("One or more active SKUs were not found.");
    }

    private async Task EnsureNoActivePriceListOverlapAsync(
        PriceList entity,
        CancellationToken cancellationToken)
    {
        var candidates = await _db.PriceLists.AsNoTracking()
            .Where(x =>
                x.PriceListID != entity.PriceListID &&
                x.Status == PriceListStatus.Active &&
                x.Channel == entity.Channel &&
                x.ClientSegmentID == entity.ClientSegmentID)
            .Select(x => new { x.EffectiveFrom, x.EffectiveTo })
            .ToListAsync(cancellationToken);
        if (candidates.Any(x => ProductPricingServiceHelper.PeriodsOverlap(
                entity.EffectiveFrom,
                entity.EffectiveTo,
                x.EffectiveFrom,
                x.EffectiveTo)))
            throw new ConflictException(BusinessRuleMessages.PriceListPeriodConflict);
    }

    private async Task ValidateDiscountRuleReferencesAsync(
        SaveStandardDiscountRuleRequestDto request,
        CancellationToken cancellationToken)
    {
        if (request.ClientSegmentID.HasValue && !await _db.ClientSegments.AnyAsync(
                x => x.ClientSegmentID == request.ClientSegmentID && x.IsActive,
                cancellationToken))
            throw new NotFoundException("Active client segment was not found.");
        if (request.SKUID.HasValue && !await _db.SKUs.AnyAsync(
                x => x.SKUID == request.SKUID && x.IsActive,
                cancellationToken))
            throw new NotFoundException("Active SKU was not found.");
        if (request.ProductCategoryID.HasValue && !await _db.ProductCategories.AnyAsync(
                x => x.ProductCategoryID == request.ProductCategoryID && x.IsActive,
                cancellationToken))
            throw new NotFoundException("Active product category was not found.");
    }

    private async Task EnsureNoDiscountRuleOverlapAsync(
        int? currentRuleID,
        SaveStandardDiscountRuleRequestDto request,
        CancellationToken cancellationToken)
    {
        var candidates = await _db.StandardDiscountRules.AsNoTracking()
            .Where(x =>
                x.IsActive &&
                (!currentRuleID.HasValue || x.StandardDiscountRuleID != currentRuleID.Value) &&
                x.Channel == request.Channel &&
                x.ClientSegmentID == request.ClientSegmentID &&
                x.SKUID == request.SKUID &&
                x.ProductCategoryID == request.ProductCategoryID &&
                x.MinQuantity == request.MinQuantity)
            .Select(x => new { x.EffectiveFrom, x.EffectiveTo })
            .ToListAsync(cancellationToken);
        if (candidates.Any(x => ProductPricingServiceHelper.PeriodsOverlap(
                request.EffectiveFrom,
                request.EffectiveTo,
                x.EffectiveFrom,
                x.EffectiveTo)))
            throw new ConflictException(BusinessRuleMessages.DiscountRulePeriodConflict);
    }

    private async Task ReplacePriceListItemsAsync(
        int priceListID,
        IReadOnlyCollection<SavePriceListItemRequestDto> requests,
        CancellationToken cancellationToken)
    {
        foreach (var request in requests)
        {
            await _db.AddAsync(new PriceListItem
            {
                PriceListID = priceListID,
                SKUID = request.SKUID,
                UnitPrice = request.UnitPrice,
                MaximumDiscountPercent = request.MaximumDiscountPercent,
                MinimumOrderQuantity = request.MinimumOrderQuantity
            }, cancellationToken);
        }
    }

    private static void ValidatePriceList(SavePriceListRequestDto request)
    {
        ValidationHelper.RequireNotBlank(request.PriceListCode, nameof(request.PriceListCode), 40);
        ValidationHelper.RequireNotBlank(request.PriceListName, nameof(request.PriceListName), 150);
        ValidationHelper.RequireNotBlank(request.CurrencyCode, nameof(request.CurrencyCode), 3);
        ValidationHelper.Require(request.CurrencyCode.Trim().Length == 3, nameof(request.CurrencyCode), "Currency code must contain exactly three characters.");
        ValidationHelper.Require(!request.EffectiveTo.HasValue || request.EffectiveTo.Value >= request.EffectiveFrom, nameof(request.EffectiveTo), "EffectiveTo must be on or after EffectiveFrom.");
        ValidationHelper.Require(request.Items.Count > 0, nameof(request.Items), "At least one price-list item is required.");
        ValidationHelper.Require(request.Items.Select(x => x.SKUID).Distinct().Count() == request.Items.Count, nameof(request.Items), "Duplicate SKUs are not allowed in a price list.");
        foreach (var item in request.Items)
        {
            ValidationHelper.Require(item.SKUID > 0, nameof(item.SKUID), "SKUID must be greater than zero.");
            ValidationHelper.Require(item.UnitPrice >= 0, nameof(item.UnitPrice), "Unit price cannot be negative.");
            ValidationHelper.Require(item.MaximumDiscountPercent is >= 0 and <= 100, nameof(item.MaximumDiscountPercent), "Maximum discount percent must be between 0 and 100.");
            if (item.MinimumOrderQuantity.HasValue)
                ValidationHelper.Require(item.MinimumOrderQuantity > 0, nameof(item.MinimumOrderQuantity), "Minimum order quantity must be greater than zero.");
        }
    }

    private static void ApplyPriceList(
        PriceList entity,
        SavePriceListRequestDto request,
        string code)
    {
        entity.PriceListCode = code;
        entity.PriceListName = request.PriceListName.Trim();
        entity.Channel = request.Channel;
        entity.ClientSegmentID = request.ClientSegmentID;
        entity.EffectiveFrom = request.EffectiveFrom;
        entity.EffectiveTo = request.EffectiveTo;
        entity.CurrencyCode = request.CurrencyCode.Trim().ToUpperInvariant();
    }

    private static void ValidateDiscountRule(SaveStandardDiscountRuleRequestDto request)
    {
        ValidationHelper.RequireNotBlank(request.RuleName, nameof(request.RuleName), 150);
        ValidationHelper.Require(!request.EffectiveTo.HasValue || request.EffectiveTo.Value >= request.EffectiveFrom, nameof(request.EffectiveTo), "EffectiveTo must be on or after EffectiveFrom.");
        ValidationHelper.Require(request.MaxDiscountPercent is >= 0 and <= 100, nameof(request.MaxDiscountPercent), "Maximum discount percent must be between 0 and 100.");
        if (request.MinQuantity.HasValue)
            ValidationHelper.Require(request.MinQuantity > 0, nameof(request.MinQuantity), "Minimum quantity must be greater than zero.");
        ValidationHelper.Require(!(request.SKUID.HasValue && request.ProductCategoryID.HasValue), nameof(request.SKUID), "A discount rule cannot target both a SKU and a product category.");
    }

    private static void ApplyDiscountRule(
        StandardDiscountRule entity,
        SaveStandardDiscountRuleRequestDto request)
    {
        entity.RuleName = request.RuleName.Trim();
        entity.Channel = request.Channel;
        entity.ClientSegmentID = request.ClientSegmentID;
        entity.SKUID = request.SKUID;
        entity.ProductCategoryID = request.ProductCategoryID;
        entity.MinQuantity = request.MinQuantity;
        entity.MaxDiscountPercent = request.MaxDiscountPercent;
        entity.RequiresApproval = request.RequiresApproval;
        entity.EffectiveFrom = request.EffectiveFrom;
        entity.EffectiveTo = request.EffectiveTo;
        entity.IsActive = request.IsActive;
    }
}
