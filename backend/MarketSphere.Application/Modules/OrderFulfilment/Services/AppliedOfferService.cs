using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.OrderFulfilment.DTOs;
using MarketSphere.Application.Modules.OrderFulfilment.Interfaces;
using MarketSphere.Domain.Entities.OrderFulfilment;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.OrderFulfilment.Services;

public sealed class AppliedOfferService : IAppliedOfferService
{
    private readonly IApplicationDbContext _db; private readonly IDateTimeProvider _clock; private readonly ICurrentUserService _currentUser;
    public AppliedOfferService(IApplicationDbContext db, IDateTimeProvider clock, ICurrentUserService currentUser) { _db = db; _clock = clock; _currentUser = currentUser; }
    public async Task<IReadOnlyCollection<AppliedOfferDto>> GetForOrderAsync(int orderID, CancellationToken cancellationToken = default) => await _db.AppliedOffers.AsNoTracking().Where(x => x.OrderID == orderID || x.OrderItem!.OrderID == orderID).OrderBy(x => x.AppliedOfferID).Select(x => new AppliedOfferDto(x.AppliedOfferID, x.QuotationID, x.QuotationItemID, x.OrderID, x.OrderItemID, x.CampaignOfferID, x.BenefitType, x.BenefitAmount, x.FreeSKUID, x.FreeQuantity, x.RuleSnapshotJson, x.UsageCount, x.AppliedAt, x.AppliedByUserID)).ToListAsync(cancellationToken);
    public async Task<int> ApplyAsync(ApplyOfferRequestDto request, CancellationToken cancellationToken = default)
    {
        var userID = _currentUser.UserID ?? throw new ForbiddenBusinessActionException("Authenticated user is required.");
        var parents = new[] { request.QuotationID, request.QuotationItemID, request.OrderID, request.OrderItemID }.Count(x => x.HasValue); if (parents != 1) throw new BusinessRuleException(BusinessRuleMessages.OfferParentInvalid);
        var offer = await OrderFulfilmentServiceHelper.RequireAsync(_db.CampaignOffers.Include(x => x.Campaign).Where(x => x.CampaignOfferID == request.CampaignOfferID), "Campaign offer", cancellationToken);
        if (!offer.IsActive || offer.Campaign.Status != CampaignStatus.Active) throw new BusinessRuleException("An active campaign offer is required.");
        if (request.UsageCount <= 0 || request.BenefitAmount < 0 || request.FreeQuantity < 0) throw new BusinessRuleException("Applied-offer values are invalid.");
        if (request.FreeSKUID.HasValue && !await _db.SKUs.AnyAsync(x => x.SKUID == request.FreeSKUID && x.IsActive, cancellationToken)) throw new NotFoundException("Free-item SKU was not found.");
        var existingUsage = await _db.AppliedOffers.Where(x => x.CampaignOfferID == request.CampaignOfferID).SumAsync(x => (int?)x.UsageCount, cancellationToken) ?? 0;
        if (offer.UsageLimit.HasValue && existingUsage + request.UsageCount > offer.UsageLimit.Value) throw new BusinessRuleException("Campaign offer usage limit has been exceeded.");
        var entity = new AppliedOffer { QuotationID = request.QuotationID, QuotationItemID = request.QuotationItemID, OrderID = request.OrderID, OrderItemID = request.OrderItemID, CampaignOfferID = request.CampaignOfferID, BenefitType = request.BenefitType, BenefitAmount = request.BenefitAmount, FreeSKUID = request.FreeSKUID, FreeQuantity = request.FreeQuantity, RuleSnapshotJson = string.IsNullOrWhiteSpace(request.RuleSnapshotJson) ? "{}" : request.RuleSnapshotJson, UsageCount = request.UsageCount, AppliedAt = _clock.UtcNow, AppliedByUserID = userID };
        await _db.AddAsync(entity, cancellationToken); await _db.SaveChangesAsync(cancellationToken); return entity.AppliedOfferID;
    }
    public async Task RemoveAsync(int id, CancellationToken cancellationToken = default) { var entity = await OrderFulfilmentServiceHelper.RequireAsync(_db.AppliedOffers.Where(x => x.AppliedOfferID == id), "Applied offer", cancellationToken); if (entity.OrderID.HasValue && await _db.Orders.AnyAsync(x => x.OrderID == entity.OrderID && x.Status != OrderStatus.Draft, cancellationToken)) throw new BusinessRuleException("An offer on a non-draft order cannot be removed."); _db.Remove(entity); await _db.SaveChangesAsync(cancellationToken); }
}
