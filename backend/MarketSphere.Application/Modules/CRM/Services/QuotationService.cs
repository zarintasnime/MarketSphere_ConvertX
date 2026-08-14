using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Mapping;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.CRM.DTOs;
using MarketSphere.Application.Modules.CRM.Interfaces;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.CRM.Services;

public sealed class QuotationService : IQuotationService
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;
    public QuotationService(IApplicationDbContext db, IDateTimeProvider clock) { _db = db; _clock = clock; }

    public Task<PagedResult<QuotationListDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var query = _db.Quotations.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x => x.QuotationNo.Contains(search) || x.Client.ClientName.Contains(search));
        }
        var projected = query.OrderByDescending(x => x.CreatedAt).Select(x => new QuotationListDto(x.QuotationID, x.QuotationNo, x.VersionNo, x.ClientID, x.OpportunityID, x.ValidFrom, x.ValidUntil, x.Status, x.NetAmount));
        return CrmServiceHelper.ToPagedAsync(projected, request, cancellationToken);
    }

    public async Task<QuotationDetailsDto> GetByIdAsync(int quotationID, CancellationToken cancellationToken = default)
        => await Project(_db.Quotations.AsNoTracking().Where(x => x.QuotationID == quotationID)).SingleOrDefaultAsync(cancellationToken)
           ?? throw new NotFoundException("Quotation was not found.");

    public async Task<int> CreateDraftAsync(SaveQuotationRequestDto request, CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);
        await ValidateReferencesAsync(request, cancellationToken);
        return await _db.ExecuteInTransactionAsync(async ct =>
        {
            var number = request.QuotationNo.NormalizeCode();
            if (await _db.Quotations.AnyAsync(x => x.QuotationNo == number && x.VersionNo == 1, ct))
                throw new ConflictException(BusinessRuleMessages.QuotationVersionConflict);
            var quotation = new Quotation { QuotationNo = number, VersionNo = 1, Status = QuotationStatus.Draft };
            ApplyHeader(quotation, request);
            await _db.AddAsync(quotation, ct);
            await _db.SaveChangesAsync(ct);
            await ReplaceItemsAsync(quotation, request.Items, ct);
            await _db.SaveChangesAsync(ct);
            return quotation.QuotationID;
        }, cancellationToken);
    }

    public async Task UpdateDraftAsync(int quotationID, SaveQuotationRequestDto request, CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);
        await ValidateReferencesAsync(request, cancellationToken);
        await _db.ExecuteInTransactionAsync(async ct =>
        {
            var quotation = await CrmServiceHelper.RequireAsync(_db.Quotations.Where(x => x.QuotationID == quotationID), "Quotation", ct);
            if (quotation.Status != QuotationStatus.Draft) throw new BusinessRuleException(BusinessRuleMessages.QuotationImmutable);
            var number = request.QuotationNo.NormalizeCode();
            if (await _db.Quotations.AnyAsync(x => x.QuotationNo == number && x.VersionNo == quotation.VersionNo && x.QuotationID != quotationID, ct))
                throw new ConflictException(BusinessRuleMessages.QuotationVersionConflict);
            quotation.QuotationNo = number;
            ApplyHeader(quotation, request);
            await ReplaceItemsAsync(quotation, request.Items, ct);
            await _db.SaveChangesAsync(ct);
            return true;
        }, cancellationToken);
    }

    public async Task<int> CreateNewVersionAsync(int quotationID, CancellationToken cancellationToken = default)
    {
        return await _db.ExecuteInTransactionAsync(async ct =>
        {
            var source = await CrmServiceHelper.RequireAsync(_db.Quotations.Where(x => x.QuotationID == quotationID), "Quotation", ct);
            if (source.Status is QuotationStatus.Accepted or QuotationStatus.Converted)
                throw new BusinessRuleException("An accepted or converted quotation cannot be versioned.");
            var rootID = source.RootQuotationID ?? source.QuotationID;
            var maxVersion = await _db.Quotations.Where(x => x.QuotationID == rootID || x.RootQuotationID == rootID).MaxAsync(x => x.VersionNo, ct);
            var version = new Quotation
            {
                RootQuotationID = rootID,
                VersionNo = maxVersion + 1,
                QuotationNo = source.QuotationNo,
                OpportunityID = source.OpportunityID,
                ClientID = source.ClientID,
                CampaignID = source.CampaignID,
                PriceListID = source.PriceListID,
                ValidFrom = source.ValidFrom,
                ValidUntil = source.ValidUntil,
                Status = QuotationStatus.Draft,
                GrossAmount = source.GrossAmount,
                DiscountAmount = source.DiscountAmount,
                TaxAmount = source.TaxAmount,
                NetAmount = source.NetAmount,
                Terms = source.Terms
            };
            await _db.AddAsync(version, ct);
            await _db.SaveChangesAsync(ct);
            var sourceItems = await _db.QuotationItems.AsNoTracking().Where(x => x.QuotationID == quotationID).ToListAsync(ct);
            foreach (var item in sourceItems)
            {
                await _db.AddAsync(new QuotationItem
                {
                    QuotationID = version.QuotationID,
                    SKUID = item.SKUID,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountPercent = item.DiscountPercent,
                    DiscountAmount = item.DiscountAmount,
                    TaxAmount = item.TaxAmount,
                    LineTotal = item.LineTotal,
                    Note = item.Note
                }, ct);
            }
            await _db.SaveChangesAsync(ct);
            return version.QuotationID;
        }, cancellationToken);
    }

    public async Task ChangeStatusAsync(int quotationID, ChangeQuotationStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        var quotation = await CrmServiceHelper.RequireAsync(_db.Quotations.Where(x => x.QuotationID == quotationID), "Quotation", cancellationToken);
        if (quotation.Status == request.Status) return;
        var allowed = quotation.Status switch
        {
            QuotationStatus.Draft => request.Status is QuotationStatus.Submitted or QuotationStatus.Rejected,
            QuotationStatus.Submitted => request.Status is QuotationStatus.Reviewed or QuotationStatus.Accepted or QuotationStatus.Rejected or QuotationStatus.Expired,
            QuotationStatus.Reviewed => request.Status is QuotationStatus.Accepted or QuotationStatus.Rejected or QuotationStatus.Expired,
            QuotationStatus.Accepted => request.Status is QuotationStatus.Converted,
            _ => false
        };
        if (!allowed) throw new BusinessRuleException(BusinessRuleMessages.InvalidStatusTransition);
        if (request.Status is QuotationStatus.Submitted or QuotationStatus.Reviewed or QuotationStatus.Accepted)
        {
            if (!await _db.QuotationItems.AnyAsync(x => x.QuotationID == quotationID, cancellationToken))
                throw new BusinessRuleException("A quotation must contain at least one item.");
            if (quotation.ValidUntil < _clock.UtcToday)
                throw new BusinessRuleException("An expired quotation cannot be submitted or accepted.");
        }
        if (request.Status == QuotationStatus.Accepted)
        {
            var rootID = quotation.RootQuotationID ?? quotation.QuotationID;
            var anotherAccepted = await _db.Quotations.AnyAsync(x => (x.QuotationID == rootID || x.RootQuotationID == rootID) && x.QuotationID != quotationID && x.Status == QuotationStatus.Accepted, cancellationToken);
            if (anotherAccepted) throw new ConflictException("Another quotation version is already accepted.");
            quotation.AcceptedAt = _clock.UtcNow;
        }
        quotation.Status = request.Status;
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateReferencesAsync(SaveQuotationRequestDto request, CancellationToken cancellationToken)
    {
        if (!await _db.Clients.AnyAsync(x => x.ClientID == request.ClientID && x.IsActive, cancellationToken)) throw new NotFoundException("Active client was not found.");
        if (request.OpportunityID.HasValue && !await _db.Opportunities.AnyAsync(x => x.OpportunityID == request.OpportunityID, cancellationToken)) throw new NotFoundException("Opportunity was not found.");
        if (request.CampaignID.HasValue && !await _db.Campaigns.AnyAsync(x => x.CampaignID == request.CampaignID, cancellationToken)) throw new NotFoundException("Campaign was not found.");
        var skuIDs = request.Items.Select(x => x.SKUID).Distinct().ToArray();
        var activeSKUCount = await _db.SKUs.CountAsync(x => skuIDs.Contains(x.SKUID) && x.IsActive && x.Product.IsActive, cancellationToken);
        if (activeSKUCount != skuIDs.Length) throw new NotFoundException("One or more active SKUs were not found.");
        if (request.PriceListID.HasValue)
        {
            var priceList = await _db.PriceLists.AsNoTracking()
                .SingleOrDefaultAsync(x => x.PriceListID == request.PriceListID, cancellationToken)
                ?? throw new NotFoundException("Price list was not found.");
            if (priceList.Status != PriceListStatus.Active || priceList.EffectiveFrom > request.ValidFrom || (priceList.EffectiveTo.HasValue && priceList.EffectiveTo.Value < request.ValidFrom))
                throw new BusinessRuleException("The selected price list is not active for the quotation date.");
            var priceItems = await _db.PriceListItems.AsNoTracking()
                .Where(x => x.PriceListID == request.PriceListID && skuIDs.Contains(x.SKUID))
                .ToDictionaryAsync(x => x.SKUID, cancellationToken);
            if (priceItems.Count != skuIDs.Length)
                throw new BusinessRuleException("Every quotation SKU must exist in the selected price list.");
            foreach (var item in request.Items)
            {
                var priceItem = priceItems[item.SKUID];
                if (item.UnitPrice != priceItem.UnitPrice)
                    throw new BusinessRuleException("Quotation unit price must match the selected price list.");
                if (item.DiscountPercent > priceItem.MaximumDiscountPercent)
                    throw new BusinessRuleException("Quotation discount exceeds the selected price-list limit.");
                if (priceItem.MinimumOrderQuantity.HasValue && item.Quantity < priceItem.MinimumOrderQuantity.Value)
                    throw new BusinessRuleException("Quotation quantity is below the price-list minimum order quantity.");
            }
        }
    }

    private static void ValidateRequest(SaveQuotationRequestDto request)
    {
        ValidationHelper.RequireNotBlank(request.QuotationNo, nameof(request.QuotationNo), 40);
        CrmServiceHelper.ValidatePositiveId(request.ClientID, nameof(request.ClientID));
        CrmServiceHelper.ValidateDateRange(request.ValidFrom, request.ValidUntil, nameof(request.ValidUntil));
        ValidationHelper.Require(request.Items.Count > 0, nameof(request.Items), "At least one quotation item is required.");
        foreach (var item in request.Items)
        {
            CrmServiceHelper.ValidatePositiveId(item.SKUID, nameof(item.SKUID));
            ValidationHelper.Require(item.Quantity > 0, nameof(item.Quantity), "Quantity must be greater than zero.");
            ValidationHelper.Require(item.UnitPrice >= 0, nameof(item.UnitPrice), "Unit price cannot be negative.");
            ValidationHelper.Require(item.DiscountPercent is >= 0 and <= 100, nameof(item.DiscountPercent), "Discount percent must be between 0 and 100.");
            ValidationHelper.Require(item.TaxAmount >= 0, nameof(item.TaxAmount), "Tax amount cannot be negative.");
        }
    }

    private static void ApplyHeader(Quotation quotation, SaveQuotationRequestDto request)
    {
        quotation.OpportunityID = request.OpportunityID;
        quotation.ClientID = request.ClientID;
        quotation.CampaignID = request.CampaignID;
        quotation.PriceListID = request.PriceListID;
        quotation.ValidFrom = request.ValidFrom;
        quotation.ValidUntil = request.ValidUntil;
        quotation.Terms = request.Terms.NullIfWhiteSpace();
    }

    private async Task ReplaceItemsAsync(Quotation quotation, IReadOnlyCollection<SaveQuotationItemRequestDto> requests, CancellationToken cancellationToken)
    {
        var existing = await _db.QuotationItems.Where(x => x.QuotationID == quotation.QuotationID).ToListAsync(cancellationToken);
        foreach (var item in existing) _db.Remove(item);
        decimal gross = 0, discount = 0, tax = 0;
        foreach (var request in requests)
        {
            var lineGross = decimal.Round(request.Quantity * request.UnitPrice, 2, MidpointRounding.AwayFromZero);
            var lineDiscount = decimal.Round(lineGross * request.DiscountPercent / 100m, 2, MidpointRounding.AwayFromZero);
            var lineTotal = lineGross - lineDiscount + request.TaxAmount;
            gross += lineGross; discount += lineDiscount; tax += request.TaxAmount;
            await _db.AddAsync(new QuotationItem { QuotationID = quotation.QuotationID, SKUID = request.SKUID, Quantity = request.Quantity, UnitPrice = request.UnitPrice, DiscountPercent = request.DiscountPercent, DiscountAmount = lineDiscount, TaxAmount = request.TaxAmount, LineTotal = lineTotal, Note = request.Note.NullIfWhiteSpace() }, cancellationToken);
        }
        quotation.GrossAmount = gross;
        quotation.DiscountAmount = discount;
        quotation.TaxAmount = tax;
        quotation.NetAmount = gross - discount + tax;
    }

    private static IQueryable<QuotationDetailsDto> Project(IQueryable<Quotation> query)
        => query.Select(x => new QuotationDetailsDto(x.QuotationID, x.RootQuotationID, x.VersionNo, x.QuotationNo, x.OpportunityID, x.ClientID, x.CampaignID, x.PriceListID, x.ValidFrom, x.ValidUntil, x.Status, x.GrossAmount, x.DiscountAmount, x.TaxAmount, x.NetAmount, x.Terms, x.AcceptedAt, x.Items.OrderBy(i => i.QuotationItemID).Select(i => new QuotationItemDto(i.QuotationItemID, i.SKUID, i.Quantity, i.UnitPrice, i.DiscountPercent, i.DiscountAmount, i.TaxAmount, i.LineTotal, i.Note)).ToList()));
}
