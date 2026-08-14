using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Mapping;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.MarketingField.DTOs;
using MarketSphere.Application.Modules.MarketingField.Interfaces;
using MarketSphere.Domain.Entities.MarketingField;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MarketSphere.Application.Modules.MarketingField.Services;

public sealed class CampaignService : ICampaignService
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;

    public CampaignService(IApplicationDbContext db, IDateTimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public Task<PagedResult<CampaignListDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var query = _db.Campaigns.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x => x.CampaignCode.Contains(search) || x.CampaignTitle.Contains(search));
        }
        var projected = query.OrderByDescending(x => x.StartDate).ThenBy(x => x.CampaignCode)
            .Select(x => new CampaignListDto(x.CampaignID, x.CampaignCode, x.CampaignTitle, x.Budget, x.ActualExpense, x.StartDate, x.EndDate, x.Channel, x.Status, x.CreatedByEmployeeID));
        return MarketingServiceHelper.ToPagedAsync(projected, request, cancellationToken);
    }

    public async Task<CampaignDetailsDto> GetByIdAsync(int campaignID, CancellationToken cancellationToken = default)
    {
        var campaign = await _db.Campaigns.AsNoTracking().SingleOrDefaultAsync(x => x.CampaignID == campaignID, cancellationToken)
            ?? throw new NotFoundException("Campaign was not found.");
        var targets = await _db.CampaignTargets.AsNoTracking().Where(x => x.CampaignID == campaignID)
            .OrderBy(x => x.CampaignTargetID).Select(x => new CampaignTargetDto(x.CampaignTargetID, x.CampaignID, x.TargetType, x.RegionID, x.AreaID, x.ClientSegmentID, x.ClientID, x.ProductCategoryID, x.SKUID, x.TargetValue)).ToListAsync(cancellationToken);
        var offers = await _db.CampaignOffers.AsNoTracking().Where(x => x.CampaignID == campaignID)
            .OrderBy(x => x.Priority).ThenBy(x => x.OfferCode).Select(x => new CampaignOfferDto(x.CampaignOfferID, x.CampaignID, x.OfferCode, x.OfferType, x.RuleJson, x.DiscountValue, x.FreeSKUID, x.Priority, x.UsageLimit, x.PerClientLimit, x.IsStackable, x.IsActive)).ToListAsync(cancellationToken);
        var expenses = await _db.CampaignExpenses.AsNoTracking().Where(x => x.CampaignID == campaignID)
            .OrderByDescending(x => x.ExpenseDate).Select(x => new CampaignExpenseDto(x.CampaignExpenseID, x.CampaignID, x.ExpenseDate, x.ExpenseCategory, x.Amount, x.VendorName, x.Description, x.Status)).ToListAsync(cancellationToken);
        var attributions = await _db.CampaignAttributions.AsNoTracking().Where(x => x.CampaignID == campaignID)
            .OrderBy(x => x.CampaignAttributionID).Select(x => new CampaignAttributionDto(x.CampaignAttributionID, x.CampaignID, x.LeadID, x.OpportunityID, x.QuotationID, x.OrderID, x.AttributionType, x.WeightPercent, x.AttributedAmount)).ToListAsync(cancellationToken);
        return new CampaignDetailsDto(campaign.CampaignID, campaign.CampaignCode, campaign.CampaignTitle, campaign.Objective, campaign.Budget, campaign.ActualExpense, campaign.StartDate, campaign.EndDate, campaign.Channel, campaign.Status, campaign.CreatedByEmployeeID, targets, offers, expenses, attributions);
    }

    public async Task<int> CreateAsync(SaveCampaignRequestDto request, CancellationToken cancellationToken = default)
    {
        ValidateCampaign(request);
        var code = request.CampaignCode.NormalizeCode();
        if (await _db.Campaigns.AnyAsync(x => x.CampaignCode == code, cancellationToken)) throw new ConflictException("Campaign code already exists.");
        await ValidateEmployeeAsync(request.CreatedByEmployeeID, cancellationToken);
        var entity = new Campaign();
        ApplyCampaign(entity, request, code);
        await _db.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.CampaignID;
    }

    public async Task UpdateAsync(int campaignID, SaveCampaignRequestDto request, CancellationToken cancellationToken = default)
    {
        ValidateCampaign(request);
        var entity = await MarketingServiceHelper.RequireAsync(_db.Campaigns.Where(x => x.CampaignID == campaignID), "Campaign", cancellationToken);
        EnsureCampaignEditable(entity);
        var code = request.CampaignCode.NormalizeCode();
        if (await _db.Campaigns.AnyAsync(x => x.CampaignCode == code && x.CampaignID != campaignID, cancellationToken)) throw new ConflictException("Campaign code already exists.");
        await ValidateEmployeeAsync(request.CreatedByEmployeeID, cancellationToken);
        ApplyCampaign(entity, request, code);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeStatusAsync(int campaignID, ChangeCampaignStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        var entity = await MarketingServiceHelper.RequireAsync(_db.Campaigns.Where(x => x.CampaignID == campaignID), "Campaign", cancellationToken);
        if (!IsAllowedTransition(entity.Status, request.Status)) throw new BusinessRuleException($"Campaign cannot move from {entity.Status} to {request.Status}.");
        if (request.Status == CampaignStatus.Active)
        {
            if (!await _db.CampaignTargets.AnyAsync(x => x.CampaignID == campaignID, cancellationToken)) throw new BusinessRuleException("At least one campaign target is required before activation.");
            if (entity.EndDate < DateOnly.FromDateTime(_clock.UtcNow)) throw new BusinessRuleException("An expired campaign cannot be activated.");
        }
        entity.Status = request.Status;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> AddTargetAsync(int campaignID, SaveCampaignTargetRequestDto request, CancellationToken cancellationToken = default)
    {
        var campaign = await MarketingServiceHelper.RequireAsync(_db.Campaigns.Where(x => x.CampaignID == campaignID), "Campaign", cancellationToken);
        EnsureCampaignEditable(campaign);
        await ValidateTargetAsync(request, cancellationToken);
        var target = new CampaignTarget { CampaignID = campaignID };
        ApplyTarget(target, request);
        await _db.AddAsync(target, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return target.CampaignTargetID;
    }

    public async Task UpdateTargetAsync(int campaignTargetID, SaveCampaignTargetRequestDto request, CancellationToken cancellationToken = default)
    {
        var target = await MarketingServiceHelper.RequireAsync(_db.CampaignTargets.Where(x => x.CampaignTargetID == campaignTargetID), "Campaign target", cancellationToken);
        var campaign = await MarketingServiceHelper.RequireAsync(_db.Campaigns.Where(x => x.CampaignID == target.CampaignID), "Campaign", cancellationToken);
        EnsureCampaignEditable(campaign);
        await ValidateTargetAsync(request, cancellationToken);
        ApplyTarget(target, request);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTargetAsync(int campaignTargetID, CancellationToken cancellationToken = default)
    {
        var target = await MarketingServiceHelper.RequireAsync(_db.CampaignTargets.Where(x => x.CampaignTargetID == campaignTargetID), "Campaign target", cancellationToken);
        var campaign = await MarketingServiceHelper.RequireAsync(_db.Campaigns.Where(x => x.CampaignID == target.CampaignID), "Campaign", cancellationToken);
        EnsureCampaignEditable(campaign);
        _db.Remove(target);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> AddOfferAsync(int campaignID, SaveCampaignOfferRequestDto request, CancellationToken cancellationToken = default)
    {
        var campaign = await MarketingServiceHelper.RequireAsync(_db.Campaigns.Where(x => x.CampaignID == campaignID), "Campaign", cancellationToken);
        EnsureCampaignEditable(campaign);
        ValidateOffer(request);
        await ValidateOfferReferencesAsync(request, cancellationToken);
        var code = request.OfferCode.NormalizeCode();
        if (await _db.CampaignOffers.AnyAsync(x => x.CampaignID == campaignID && x.OfferCode == code, cancellationToken)) throw new ConflictException("Offer code already exists in this campaign.");
        var offer = new CampaignOffer { CampaignID = campaignID };
        ApplyOffer(offer, request, code);
        await _db.AddAsync(offer, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return offer.CampaignOfferID;
    }

    public async Task UpdateOfferAsync(int campaignOfferID, SaveCampaignOfferRequestDto request, CancellationToken cancellationToken = default)
    {
        ValidateOffer(request);
        await ValidateOfferReferencesAsync(request, cancellationToken);
        var offer = await MarketingServiceHelper.RequireAsync(_db.CampaignOffers.Where(x => x.CampaignOfferID == campaignOfferID), "Campaign offer", cancellationToken);
        var campaign = await MarketingServiceHelper.RequireAsync(_db.Campaigns.Where(x => x.CampaignID == offer.CampaignID), "Campaign", cancellationToken);
        EnsureCampaignEditable(campaign);
        var code = request.OfferCode.NormalizeCode();
        if (await _db.CampaignOffers.AnyAsync(x => x.CampaignID == offer.CampaignID && x.OfferCode == code && x.CampaignOfferID != campaignOfferID, cancellationToken)) throw new ConflictException("Offer code already exists in this campaign.");
        ApplyOffer(offer, request, code);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteOfferAsync(int campaignOfferID, CancellationToken cancellationToken = default)
    {
        var offer = await MarketingServiceHelper.RequireAsync(_db.CampaignOffers.Where(x => x.CampaignOfferID == campaignOfferID), "Campaign offer", cancellationToken);
        var campaign = await MarketingServiceHelper.RequireAsync(_db.Campaigns.Where(x => x.CampaignID == offer.CampaignID), "Campaign", cancellationToken);
        EnsureCampaignEditable(campaign);
        _db.Remove(offer);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> AddExpenseAsync(int campaignID, SaveCampaignExpenseRequestDto request, CancellationToken cancellationToken = default)
    {
        var campaign = await MarketingServiceHelper.RequireAsync(_db.Campaigns.Where(x => x.CampaignID == campaignID), "Campaign", cancellationToken);
        ValidateExpense(campaign, request);
        var expense = new CampaignExpense { CampaignID = campaignID, Status = CampaignExpenseStatus.Draft };
        ApplyExpense(expense, request);
        await _db.AddAsync(expense, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return expense.CampaignExpenseID;
    }

    public async Task UpdateExpenseAsync(int campaignExpenseID, SaveCampaignExpenseRequestDto request, CancellationToken cancellationToken = default)
    {
        var expense = await MarketingServiceHelper.RequireAsync(_db.CampaignExpenses.Where(x => x.CampaignExpenseID == campaignExpenseID), "Campaign expense", cancellationToken);
        if (expense.Status is CampaignExpenseStatus.Approved or CampaignExpenseStatus.Posted) throw new BusinessRuleException("Approved or posted campaign expenses cannot be edited.");
        var campaign = await MarketingServiceHelper.RequireAsync(_db.Campaigns.Where(x => x.CampaignID == expense.CampaignID), "Campaign", cancellationToken);
        ValidateExpense(campaign, request);
        ApplyExpense(expense, request);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeExpenseStatusAsync(int campaignExpenseID, ChangeCampaignExpenseStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        await _db.ExecuteInTransactionAsync(async ct =>
        {
            var expense = await MarketingServiceHelper.RequireAsync(_db.CampaignExpenses.Where(x => x.CampaignExpenseID == campaignExpenseID), "Campaign expense", ct);
            if (!IsAllowedExpenseTransition(expense.Status, request.Status)) throw new BusinessRuleException($"Campaign expense cannot move from {expense.Status} to {request.Status}.");
            expense.Status = request.Status;
            await _db.SaveChangesAsync(ct);
            await RecalculateActualExpenseAsync(expense.CampaignID, ct);
            return 0;
        }, cancellationToken);
    }

    public async Task<int> AddAttributionAsync(int campaignID, SaveCampaignAttributionRequestDto request, CancellationToken cancellationToken = default)
    {
        await MarketingServiceHelper.RequireAsync(_db.Campaigns.Where(x => x.CampaignID == campaignID), "Campaign", cancellationToken);
        ValidateAttribution(request);
        await ValidateAttributionReferencesAsync(request, cancellationToken);
        var entity = new CampaignAttribution
        {
            CampaignID = campaignID,
            LeadID = request.LeadID,
            OpportunityID = request.OpportunityID,
            QuotationID = request.QuotationID,
            OrderID = request.OrderID,
            AttributionType = request.AttributionType,
            WeightPercent = request.WeightPercent,
            AttributedAmount = request.AttributedAmount
        };
        await _db.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.CampaignAttributionID;
    }

    public async Task<CampaignRoiDto> GetRoiAsync(int campaignID, CancellationToken cancellationToken = default)
    {
        var campaign = await MarketingServiceHelper.RequireAsync(_db.Campaigns.AsNoTracking().Where(x => x.CampaignID == campaignID), "Campaign", cancellationToken);
        var attributedAmount = await _db.CampaignAttributions.AsNoTracking().Where(x => x.CampaignID == campaignID).SumAsync(x => x.AttributedAmount ?? 0, cancellationToken);
        var roiAmount = attributedAmount - campaign.ActualExpense;
        decimal? roiPercent = campaign.ActualExpense == 0 ? null : Math.Round(roiAmount / campaign.ActualExpense * 100, 2);
        return new CampaignRoiDto(campaignID, campaign.Budget, campaign.ActualExpense, attributedAmount, roiAmount, roiPercent);
    }

    private async Task ValidateEmployeeAsync(int employeeID, CancellationToken cancellationToken)
    {
        if (!await _db.Employees.AnyAsync(x => x.EmployeeID == employeeID, cancellationToken)) throw new NotFoundException("Employee was not found.");
    }

    private async Task ValidateTargetAsync(SaveCampaignTargetRequestDto request, CancellationToken cancellationToken)
    {
        var selected = new[] { request.RegionID, request.AreaID, request.ClientSegmentID, request.ClientID, request.ProductCategoryID, request.SKUID }.Count(x => x.HasValue);
        ValidationHelper.Require(selected == 1, nameof(request.TargetType), "Exactly one target reference is required.");
        var matches = request.TargetType switch
        {
            CampaignTargetType.Region => request.RegionID.HasValue,
            CampaignTargetType.Area => request.AreaID.HasValue,
            CampaignTargetType.ClientSegment => request.ClientSegmentID.HasValue,
            CampaignTargetType.Client => request.ClientID.HasValue,
            CampaignTargetType.ProductCategory => request.ProductCategoryID.HasValue,
            CampaignTargetType.SKU => request.SKUID.HasValue,
            _ => false
        };
        ValidationHelper.Require(matches, nameof(request.TargetType), "Target type does not match the selected reference.");
        if (request.TargetValue.HasValue) ValidationHelper.Require(request.TargetValue >= 0, nameof(request.TargetValue), "Target value cannot be negative.");
        if (request.RegionID.HasValue && !await _db.Regions.AnyAsync(x => x.RegionID == request.RegionID, cancellationToken)) throw new NotFoundException("Region was not found.");
        if (request.AreaID.HasValue && !await _db.Areas.AnyAsync(x => x.AreaID == request.AreaID, cancellationToken)) throw new NotFoundException("Area was not found.");
        if (request.ClientSegmentID.HasValue && !await _db.ClientSegments.AnyAsync(x => x.ClientSegmentID == request.ClientSegmentID, cancellationToken)) throw new NotFoundException("Client segment was not found.");
        if (request.ClientID.HasValue && !await _db.Clients.AnyAsync(x => x.ClientID == request.ClientID, cancellationToken)) throw new NotFoundException("Client was not found.");
        if (request.ProductCategoryID.HasValue && !await _db.ProductCategories.AnyAsync(x => x.ProductCategoryID == request.ProductCategoryID && x.IsActive, cancellationToken)) throw new NotFoundException("Active product category was not found.");
        if (request.SKUID.HasValue && !await _db.SKUs.AnyAsync(x => x.SKUID == request.SKUID && x.IsActive, cancellationToken)) throw new NotFoundException("Active SKU was not found.");
    }

    private async Task ValidateOfferReferencesAsync(SaveCampaignOfferRequestDto request, CancellationToken cancellationToken)
    {
        if (request.FreeSKUID.HasValue && !await _db.SKUs.AnyAsync(x => x.SKUID == request.FreeSKUID && x.IsActive, cancellationToken))
            throw new NotFoundException("Active free-item SKU was not found.");
    }

    private async Task ValidateAttributionReferencesAsync(SaveCampaignAttributionRequestDto request, CancellationToken cancellationToken)
    {
        if (request.LeadID.HasValue && !await _db.Leads.AnyAsync(x => x.LeadID == request.LeadID, cancellationToken)) throw new NotFoundException("Lead was not found.");
        if (request.OpportunityID.HasValue && !await _db.Opportunities.AnyAsync(x => x.OpportunityID == request.OpportunityID, cancellationToken)) throw new NotFoundException("Opportunity was not found.");
        if (request.QuotationID.HasValue && !await _db.Quotations.AnyAsync(x => x.QuotationID == request.QuotationID, cancellationToken)) throw new NotFoundException("Quotation was not found.");
        if (request.OrderID.HasValue && !await _db.Orders.AnyAsync(x => x.OrderID == request.OrderID, cancellationToken)) throw new NotFoundException("Order was not found.");
    }

    private async Task RecalculateActualExpenseAsync(int campaignID, CancellationToken cancellationToken)
    {
        var campaign = await MarketingServiceHelper.RequireAsync(_db.Campaigns.Where(x => x.CampaignID == campaignID), "Campaign", cancellationToken);
        campaign.ActualExpense = await _db.CampaignExpenses.Where(x => x.CampaignID == campaignID && x.Status == CampaignExpenseStatus.Posted).SumAsync(x => x.Amount, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private static void ValidateCampaign(SaveCampaignRequestDto request)
    {
        ValidationHelper.RequireNotBlank(request.CampaignCode, nameof(request.CampaignCode), 30);
        ValidationHelper.RequireNotBlank(request.CampaignTitle, nameof(request.CampaignTitle), 200);
        ValidationHelper.RequireNotBlank(request.Objective, nameof(request.Objective), 1000);
        ValidationHelper.Require(request.Budget >= 0, nameof(request.Budget), "Budget cannot be negative.");
        ValidationHelper.Require(request.EndDate >= request.StartDate, nameof(request.EndDate), "End date must be on or after the start date.");
        ValidationHelper.Require(request.CreatedByEmployeeID > 0, nameof(request.CreatedByEmployeeID), "CreatedByEmployeeID must be greater than zero.");
    }

    private static void ApplyCampaign(Campaign entity, SaveCampaignRequestDto request, string code)
    {
        entity.CampaignCode = code; entity.CampaignTitle = request.CampaignTitle.Trim(); entity.Objective = request.Objective.Trim();
        entity.Budget = request.Budget; entity.StartDate = request.StartDate; entity.EndDate = request.EndDate;
        entity.Channel = request.Channel; entity.CreatedByEmployeeID = request.CreatedByEmployeeID;
    }

    private static void EnsureCampaignEditable(Campaign campaign)
    {
        if (campaign.Status is not (CampaignStatus.Draft or CampaignStatus.Rejected)) throw new BusinessRuleException("Only draft or rejected campaigns can be edited.");
    }

    private static bool IsAllowedTransition(CampaignStatus current, CampaignStatus next) => (current, next) switch
    {
        (CampaignStatus.Draft, CampaignStatus.Submitted) => true,
        (CampaignStatus.Submitted, CampaignStatus.Approved) => true,
        (CampaignStatus.Submitted, CampaignStatus.Rejected) => true,
        (CampaignStatus.Rejected, CampaignStatus.Draft) => true,
        (CampaignStatus.Approved, CampaignStatus.Active) => true,
        (CampaignStatus.Active, CampaignStatus.Paused) => true,
        (CampaignStatus.Paused, CampaignStatus.Active) => true,
        (CampaignStatus.Active, CampaignStatus.Completed) => true,
        (CampaignStatus.Paused, CampaignStatus.Completed) => true,
        (CampaignStatus.Completed, CampaignStatus.Evaluated) => true,
        _ => false
    };

    private static void ApplyTarget(CampaignTarget target, SaveCampaignTargetRequestDto request)
    {
        target.TargetType = request.TargetType; target.RegionID = request.RegionID; target.AreaID = request.AreaID;
        target.ClientSegmentID = request.ClientSegmentID; target.ClientID = request.ClientID;
        target.ProductCategoryID = request.ProductCategoryID; target.SKUID = request.SKUID; target.TargetValue = request.TargetValue;
    }

    private static void ValidateOffer(SaveCampaignOfferRequestDto request)
    {
        ValidationHelper.RequireNotBlank(request.OfferCode, nameof(request.OfferCode), 40);
        ValidationHelper.RequireNotBlank(request.RuleJson, nameof(request.RuleJson), 8000);
        try { using var _ = JsonDocument.Parse(request.RuleJson); } catch (JsonException) { ValidationHelper.Require(false, nameof(request.RuleJson), "RuleJson must contain valid JSON."); }
        if (request.DiscountValue.HasValue) ValidationHelper.Require(request.DiscountValue >= 0, nameof(request.DiscountValue), "Discount value cannot be negative.");
        if (request.UsageLimit.HasValue) ValidationHelper.Require(request.UsageLimit > 0, nameof(request.UsageLimit), "Usage limit must be greater than zero.");
        if (request.PerClientLimit.HasValue) ValidationHelper.Require(request.PerClientLimit > 0, nameof(request.PerClientLimit), "Per-client limit must be greater than zero.");
        if (request.OfferType == CampaignOfferType.FreeItem) ValidationHelper.Require(request.FreeSKUID.HasValue, nameof(request.FreeSKUID), "FreeSKUID is required for a free-item offer.");
    }

    private static void ApplyOffer(CampaignOffer offer, SaveCampaignOfferRequestDto request, string code)
    {
        offer.OfferCode = code; offer.OfferType = request.OfferType; offer.RuleJson = request.RuleJson.Trim(); offer.DiscountValue = request.DiscountValue;
        offer.FreeSKUID = request.FreeSKUID; offer.Priority = request.Priority; offer.UsageLimit = request.UsageLimit; offer.PerClientLimit = request.PerClientLimit;
        offer.IsStackable = request.IsStackable; offer.IsActive = request.IsActive;
    }

    private static void ValidateExpense(Campaign campaign, SaveCampaignExpenseRequestDto request)
    {
        ValidationHelper.RequireNotBlank(request.ExpenseCategory, nameof(request.ExpenseCategory), 100);
        ValidationHelper.Require(request.Amount > 0, nameof(request.Amount), "Amount must be greater than zero.");
        ValidationHelper.Require(request.ExpenseDate >= campaign.StartDate && request.ExpenseDate <= campaign.EndDate, nameof(request.ExpenseDate), "Expense date must be within the campaign period.");
    }

    private static void ApplyExpense(CampaignExpense expense, SaveCampaignExpenseRequestDto request)
    {
        expense.ExpenseDate = request.ExpenseDate; expense.ExpenseCategory = request.ExpenseCategory.Trim(); expense.Amount = request.Amount;
        expense.VendorName = request.VendorName.NullIfWhiteSpace(); expense.Description = request.Description.NullIfWhiteSpace();
    }

    private static bool IsAllowedExpenseTransition(CampaignExpenseStatus current, CampaignExpenseStatus next) => (current, next) switch
    {
        (CampaignExpenseStatus.Draft, CampaignExpenseStatus.Submitted) => true,
        (CampaignExpenseStatus.Submitted, CampaignExpenseStatus.Approved) => true,
        (CampaignExpenseStatus.Submitted, CampaignExpenseStatus.Rejected) => true,
        (CampaignExpenseStatus.Rejected, CampaignExpenseStatus.Draft) => true,
        (CampaignExpenseStatus.Approved, CampaignExpenseStatus.Posted) => true,
        _ => false
    };

    private static void ValidateAttribution(SaveCampaignAttributionRequestDto request)
    {
        ValidationHelper.Require(request.LeadID.HasValue || request.OpportunityID.HasValue || request.QuotationID.HasValue || request.OrderID.HasValue, nameof(request.LeadID), "At least one attribution stage reference is required.");
        MarketingServiceHelper.ValidatePercentage(request.WeightPercent, nameof(request.WeightPercent));
        if (request.AttributedAmount.HasValue) ValidationHelper.Require(request.AttributedAmount >= 0, nameof(request.AttributedAmount), "Attributed amount cannot be negative.");
    }
}
