using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Mapping;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Security;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.MarketingField.DTOs;
using MarketSphere.Application.Modules.MarketingField.Interfaces;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Entities.MarketingField;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.MarketingField.Services;

public sealed class FeedbackService : IFeedbackService
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;
    private readonly ICurrentUserService _currentUser;

    public FeedbackService(
        IApplicationDbContext db,
        IDateTimeProvider clock,
        ICurrentUserService currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    public Task<PagedResult<FeedbackListDto>> GetPagedAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Feedbacks.AsNoTracking();

        if (_currentUser.IsFieldUser())
        {
            var employeeID = _currentUser.RequireEmployeeID();
            query = query.Where(x => x.SubmittedByEmployeeID == employeeID);
        }

        var projected = query
            .OrderByDescending(x => x.SubmittedAt)
            .Select(x => new FeedbackListDto(
                x.FeedbackID,
                x.ClientID,
                x.LeadID,
                x.CampaignID,
                x.VisitID,
                x.SubmittedByEmployeeID,
                x.FeedbackType,
                x.Rating,
                x.SubmittedAt,
                x.IsFollowUpRequired));

        return MarketingServiceHelper.ToPagedAsync(
            projected,
            request,
            cancellationToken);
    }

    public async Task<FeedbackDetailsDto> GetByIdAsync(
        int feedbackID,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Feedbacks
            .AsNoTracking()
            .Where(x => x.FeedbackID == feedbackID);

        if (_currentUser.IsFieldUser())
        {
            var employeeID = _currentUser.RequireEmployeeID();
            query = query.Where(x => x.SubmittedByEmployeeID == employeeID);
        }

        return await query
            .Select(x => new FeedbackDetailsDto(
                x.FeedbackID,
                x.ClientID,
                x.LeadID,
                x.CampaignID,
                x.VisitID,
                x.SubmittedByEmployeeID,
                x.FeedbackType,
                x.Rating,
                x.Comments,
                x.SubmittedAt,
                x.IsFollowUpRequired))
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Feedback was not found.");
    }

    public async Task<int> CreateAsync(
        SaveFeedbackRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var submittedByEmployeeID =
            _currentUser.ResolveOptionalFieldEmployeeID(
                request.SubmittedByEmployeeID);

        Validate(request);

        await ValidateReferencesAsync(
            request,
            submittedByEmployeeID,
            cancellationToken);

        var entity = new Feedback();
        Apply(entity, request, submittedByEmployeeID);

        await _db.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        if (entity.IsFollowUpRequired &&
            entity.SubmittedByEmployeeID.HasValue)
        {
            await CreateFollowUpTaskAsync(entity, cancellationToken);
        }

        return entity.FeedbackID;
    }

    public async Task UpdateAsync(
        int feedbackID,
        SaveFeedbackRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = await MarketingServiceHelper.RequireAsync(
            _db.Feedbacks.Where(x => x.FeedbackID == feedbackID),
            "Feedback",
            cancellationToken);

        _currentUser.EnsureOptionalFieldRecordOwnership(
            entity.SubmittedByEmployeeID);

        var submittedByEmployeeID =
            _currentUser.ResolveOptionalFieldEmployeeID(
                request.SubmittedByEmployeeID);

        Validate(request);

        await ValidateReferencesAsync(
            request,
            submittedByEmployeeID,
            cancellationToken);

        Apply(entity, request, submittedByEmployeeID);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        int feedbackID,
        CancellationToken cancellationToken = default)
    {
        var entity = await MarketingServiceHelper.RequireAsync(
            _db.Feedbacks.Where(x => x.FeedbackID == feedbackID),
            "Feedback",
            cancellationToken);

        _currentUser.EnsureOptionalFieldRecordOwnership(
            entity.SubmittedByEmployeeID);

        _db.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private static void Validate(SaveFeedbackRequestDto request)
    {
        ValidationHelper.Require(
            request.ClientID.HasValue || request.LeadID.HasValue,
            nameof(request.ClientID),
            "ClientID or LeadID is required.");

        if (request.Rating.HasValue)
        {
            ValidationHelper.Require(
                request.Rating is >= 1 and <= 5,
                nameof(request.Rating),
                "Rating must be between 1 and 5.");
        }

        if (!string.IsNullOrWhiteSpace(request.Comments))
        {
            ValidationHelper.Require(
                request.Comments.Trim().Length <= 4000,
                nameof(request.Comments),
                "Comments cannot exceed 4000 characters.");
        }
    }

    private async Task ValidateReferencesAsync(
        SaveFeedbackRequestDto request,
        int? submittedByEmployeeID,
        CancellationToken cancellationToken)
    {
        if (request.ClientID.HasValue &&
            !await _db.Clients.AnyAsync(
                x => x.ClientID == request.ClientID.Value,
                cancellationToken))
        {
            throw new NotFoundException("Client was not found.");
        }

        if (request.LeadID.HasValue &&
            !await _db.Leads.AnyAsync(
                x => x.LeadID == request.LeadID.Value,
                cancellationToken))
        {
            throw new NotFoundException("Lead was not found.");
        }

        if (request.CampaignID.HasValue &&
            !await _db.Campaigns.AnyAsync(
                x => x.CampaignID == request.CampaignID.Value,
                cancellationToken))
        {
            throw new NotFoundException("Campaign was not found.");
        }

        if (submittedByEmployeeID.HasValue &&
            !await _db.Employees.AnyAsync(
                x => x.EmployeeID == submittedByEmployeeID.Value &&
                     x.Status == EmployeeStatus.Active,
                cancellationToken))
        {
            throw new NotFoundException("Active employee was not found.");
        }

        if (request.VisitID.HasValue)
        {
            var visit = await _db.Visits
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    x => x.VisitID == request.VisitID.Value,
                    cancellationToken)
                ?? throw new NotFoundException("Visit was not found.");

            if (submittedByEmployeeID.HasValue &&
                visit.EmployeeID != submittedByEmployeeID.Value)
            {
                throw new BusinessRuleException(
                    "Feedback employee must match the selected visit employee.");
            }

            if (request.ClientID.HasValue &&
                visit.ClientID != request.ClientID.Value)
            {
                throw new BusinessRuleException(
                    "Feedback client must match the selected visit client.");
            }
        }
    }

    private void Apply(
        Feedback entity,
        SaveFeedbackRequestDto request,
        int? submittedByEmployeeID)
    {
        entity.ClientID = request.ClientID;
        entity.LeadID = request.LeadID;
        entity.CampaignID = request.CampaignID;
        entity.VisitID = request.VisitID;
        entity.SubmittedByEmployeeID = submittedByEmployeeID;
        entity.FeedbackType = request.FeedbackType;
        entity.Rating = request.Rating;
        entity.Comments = request.Comments.NullIfWhiteSpace();
        entity.SubmittedAt = request.SubmittedAt ?? _clock.UtcNow;
        entity.IsFollowUpRequired = request.IsFollowUpRequired;
    }

    private async Task CreateFollowUpTaskAsync(
        Feedback entity,
        CancellationToken cancellationToken)
    {
        await _db.AddAsync(
            new CRMTask
            {
                LeadID = entity.LeadID,
                ClientID = entity.ClientID,
                AssignedEmployeeID = entity.SubmittedByEmployeeID!.Value,
                Title = $"Feedback follow-up #{entity.FeedbackID}",
                Description = entity.Comments,
                Priority = TaskPriority.Normal,
                DueAt = _clock.UtcNow.AddDays(2),
                Status = CrmTaskStatus.Open
            },
            cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
    }
}
