using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Mapping;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.MarketingField.DTOs;
using MarketSphere.Application.Modules.MarketingField.Interfaces;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Entities.MarketingField;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.MarketingField.Services;

public sealed class SamplingService : ISamplingService
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;
    public SamplingService(IApplicationDbContext db, IDateTimeProvider clock) { _db = db; _clock = clock; }

    public Task<PagedResult<SamplingLogListDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var projected = _db.SamplingLogs.AsNoTracking().OrderByDescending(x => x.SampleDate)
            .Select(x => new SamplingLogListDto(x.SamplingLogID, x.VisitID, x.CampaignID, x.ClientID, x.LeadID, x.EmployeeID, x.SKUID, x.IssuedQuantity, x.ConsumedQuantity, x.ReturnedQuantity, x.DamagedQuantity, x.SampleDate, x.Outcome, x.FollowUpRequired));
        return MarketingServiceHelper.ToPagedAsync(projected, request, cancellationToken);
    }

    public async Task<SamplingLogDetailsDto> GetByIdAsync(int samplingLogID, CancellationToken cancellationToken = default)
        => await _db.SamplingLogs.AsNoTracking().Where(x => x.SamplingLogID == samplingLogID)
            .Select(x => new SamplingLogDetailsDto(x.SamplingLogID, x.VisitID, x.CampaignID, x.ClientID, x.LeadID, x.EmployeeID, x.SKUID, x.IssuedQuantity, x.ConsumedQuantity, x.ReturnedQuantity, x.DamagedQuantity, x.SampleDate, x.FeedbackSummary, x.Outcome, x.FollowUpRequired, x.IssueStockMovementID, x.ReturnStockMovementID, x.DamageStockMovementID))
            .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException("Sampling log was not found.");

    public async Task<int> CreateAsync(SaveSamplingLogRequestDto request, CancellationToken cancellationToken = default)
    {
        Validate(request); await ValidateReferencesAsync(request, cancellationToken);
        var entity = new SamplingLog(); Apply(entity, request);
        await _db.AddAsync(entity, cancellationToken); await _db.SaveChangesAsync(cancellationToken);
        if (request.FollowUpRequired) await CreateFollowUpTaskAsync(entity, cancellationToken);
        return entity.SamplingLogID;
    }

    public async Task UpdateAsync(int samplingLogID, SaveSamplingLogRequestDto request, CancellationToken cancellationToken = default)
    {
        Validate(request); await ValidateReferencesAsync(request, cancellationToken);
        var entity = await MarketingServiceHelper.RequireAsync(_db.SamplingLogs.Where(x => x.SamplingLogID == samplingLogID), "Sampling log", cancellationToken);
        if (entity.IssueStockMovementID.HasValue || entity.ReturnStockMovementID.HasValue || entity.DamageStockMovementID.HasValue) throw new BusinessRuleException("A sampling log linked to stock movements cannot be edited.");
        Apply(entity, request); await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int samplingLogID, CancellationToken cancellationToken = default)
    {
        var entity = await MarketingServiceHelper.RequireAsync(_db.SamplingLogs.Where(x => x.SamplingLogID == samplingLogID), "Sampling log", cancellationToken);
        if (entity.IssueStockMovementID.HasValue || entity.ReturnStockMovementID.HasValue || entity.DamageStockMovementID.HasValue) throw new BusinessRuleException("A sampling log linked to stock movements cannot be deleted.");
        _db.Remove(entity); await _db.SaveChangesAsync(cancellationToken);
    }

    private static void Validate(SaveSamplingLogRequestDto request)
    {
        ValidationHelper.Require(request.EmployeeID > 0, nameof(request.EmployeeID), "EmployeeID must be greater than zero.");
        ValidationHelper.Require(request.SKUID > 0, nameof(request.SKUID), "SKUID must be greater than zero.");
        ValidationHelper.Require(request.ClientID.HasValue || request.LeadID.HasValue, nameof(request.ClientID), "ClientID or LeadID is required.");
        ValidationHelper.Require(request.IssuedQuantity > 0, nameof(request.IssuedQuantity), "Issued quantity must be greater than zero.");
        ValidationHelper.Require(request.ConsumedQuantity >= 0 && request.ReturnedQuantity >= 0 && request.DamagedQuantity >= 0, nameof(request.ConsumedQuantity), "Sampling quantities cannot be negative.");
        ValidationHelper.Require(request.IssuedQuantity == request.ConsumedQuantity + request.ReturnedQuantity + request.DamagedQuantity, nameof(request.IssuedQuantity), "Issued quantity must equal consumed, returned and damaged quantities.");
    }

    private async Task ValidateReferencesAsync(SaveSamplingLogRequestDto request, CancellationToken cancellationToken)
    {
        if (!await _db.Employees.AnyAsync(x => x.EmployeeID == request.EmployeeID, cancellationToken)) throw new NotFoundException("Employee was not found.");
        if (!await _db.SKUs.AnyAsync(x => x.SKUID == request.SKUID && x.IsActive, cancellationToken)) throw new NotFoundException("Active SKU was not found.");
        if (request.VisitID.HasValue && !await _db.Visits.AnyAsync(x => x.VisitID == request.VisitID, cancellationToken)) throw new NotFoundException("Visit was not found.");
        if (request.CampaignID.HasValue && !await _db.Campaigns.AnyAsync(x => x.CampaignID == request.CampaignID, cancellationToken)) throw new NotFoundException("Campaign was not found.");
        if (request.ClientID.HasValue && !await _db.Clients.AnyAsync(x => x.ClientID == request.ClientID, cancellationToken)) throw new NotFoundException("Client was not found.");
        if (request.LeadID.HasValue && !await _db.Leads.AnyAsync(x => x.LeadID == request.LeadID, cancellationToken)) throw new NotFoundException("Lead was not found.");
    }

    private static void Apply(SamplingLog entity, SaveSamplingLogRequestDto request)
    {
        entity.VisitID = request.VisitID; entity.CampaignID = request.CampaignID; entity.ClientID = request.ClientID; entity.LeadID = request.LeadID;
        entity.EmployeeID = request.EmployeeID; entity.SKUID = request.SKUID; entity.IssuedQuantity = request.IssuedQuantity;
        entity.ConsumedQuantity = request.ConsumedQuantity; entity.ReturnedQuantity = request.ReturnedQuantity; entity.DamagedQuantity = request.DamagedQuantity;
        entity.SampleDate = request.SampleDate; entity.FeedbackSummary = request.FeedbackSummary.NullIfWhiteSpace(); entity.Outcome = request.Outcome; entity.FollowUpRequired = request.FollowUpRequired;
    }

    private async Task CreateFollowUpTaskAsync(SamplingLog entity, CancellationToken cancellationToken)
    {
        await _db.AddAsync(new CRMTask
        {
            LeadID = entity.LeadID,
            ClientID = entity.ClientID,
            AssignedEmployeeID = entity.EmployeeID,
            Title = $"Sampling follow-up #{entity.SamplingLogID}",
            Description = entity.FeedbackSummary,
            Priority = TaskPriority.Normal,
            DueAt = _clock.UtcNow.AddDays(2),
            Status = CrmTaskStatus.Open
        }, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
