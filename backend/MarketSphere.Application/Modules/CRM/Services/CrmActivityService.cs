using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Mapping;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.CRM.DTOs;
using MarketSphere.Application.Modules.CRM.Interfaces;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.CRM.Services;

public sealed class CrmActivityService : ICrmActivityService
{
    private readonly IApplicationDbContext _db;
    public CrmActivityService(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<CrmActivityDto>> GetTimelineAsync(int? leadID, int? clientID, int? opportunityID, CancellationToken cancellationToken = default)
    {
        ValidationHelper.Require(leadID.HasValue || clientID.HasValue || opportunityID.HasValue, nameof(leadID), BusinessRuleMessages.RelatedEntityRequired);
        var query = _db.CRMActivities.AsNoTracking();
        if (leadID.HasValue) query = query.Where(x => x.LeadID == leadID);
        if (clientID.HasValue) query = query.Where(x => x.ClientID == clientID);
        if (opportunityID.HasValue) query = query.Where(x => x.OpportunityID == opportunityID);
        return await Project(query.OrderByDescending(x => x.ActivityAt)).ToListAsync(cancellationToken);
    }

    public async Task<CrmActivityDto> GetByIdAsync(int activityID, CancellationToken cancellationToken = default)
        => await Project(_db.CRMActivities.AsNoTracking().Where(x => x.CRMActivityID == activityID)).SingleOrDefaultAsync(cancellationToken)
           ?? throw new NotFoundException("CRM activity was not found.");

    public async Task<int> CreateAsync(SaveCrmActivityRequestDto request, CancellationToken cancellationToken = default)
    {
        Validate(request);
        await ValidateReferencesAsync(request, cancellationToken);
        return await _db.ExecuteInTransactionAsync(async ct =>
        {
            var activity = new CRMActivity();
            Apply(activity, request);
            await _db.AddAsync(activity, ct);
            await _db.SaveChangesAsync(ct);
            foreach (var participantRequest in request.Participants)
            {
                var participant = CreateParticipant(activity.CRMActivityID, participantRequest);
                await _db.AddAsync(participant, ct);
            }
            await _db.SaveChangesAsync(ct);
            return activity.CRMActivityID;
        }, cancellationToken);
    }

    public async Task UpdateAsync(int activityID, SaveCrmActivityRequestDto request, CancellationToken cancellationToken = default)
    {
        Validate(request);
        await ValidateReferencesAsync(request, cancellationToken);
        await _db.ExecuteInTransactionAsync(async ct =>
        {
            var activity = await CrmServiceHelper.RequireAsync(_db.CRMActivities.Where(x => x.CRMActivityID == activityID), "CRM activity", ct);
            Apply(activity, request);
            var existing = await _db.CRMActivityParticipants.Where(x => x.CRMActivityID == activityID).ToListAsync(ct);
            foreach (var participant in existing) _db.Remove(participant);
            foreach (var participantRequest in request.Participants) await _db.AddAsync(CreateParticipant(activityID, participantRequest), ct);
            await _db.SaveChangesAsync(ct);
            return true;
        }, cancellationToken);
    }

    private async Task ValidateReferencesAsync(SaveCrmActivityRequestDto request, CancellationToken cancellationToken)
    {
        if (request.LeadID.HasValue && !await _db.Leads.AnyAsync(x => x.LeadID == request.LeadID, cancellationToken)) throw new NotFoundException("Lead was not found.");
        if (request.ClientID.HasValue && !await _db.Clients.AnyAsync(x => x.ClientID == request.ClientID, cancellationToken)) throw new NotFoundException("Client was not found.");
        if (request.OpportunityID.HasValue && !await _db.Opportunities.AnyAsync(x => x.OpportunityID == request.OpportunityID, cancellationToken)) throw new NotFoundException("Opportunity was not found.");
        if (request.PerformedByEmployeeID.HasValue && !await _db.Employees.AnyAsync(x => x.EmployeeID == request.PerformedByEmployeeID, cancellationToken)) throw new NotFoundException("Employee was not found.");
        foreach (var participant in request.Participants)
        {
            if (participant.EmployeeID.HasValue && !await _db.Employees.AnyAsync(x => x.EmployeeID == participant.EmployeeID, cancellationToken)) throw new NotFoundException("Participant employee was not found.");
            if (participant.ClientContactID.HasValue && !await _db.ClientContacts.AnyAsync(x => x.ClientContactID == participant.ClientContactID, cancellationToken)) throw new NotFoundException("Client contact was not found.");
        }
    }

    private static void Validate(SaveCrmActivityRequestDto request)
    {
        ValidationHelper.Require(request.LeadID.HasValue || request.ClientID.HasValue || request.OpportunityID.HasValue, nameof(request.LeadID), BusinessRuleMessages.RelatedEntityRequired);
        ValidationHelper.RequireNotBlank(request.Subject, nameof(request.Subject), 200);
        CrmServiceHelper.ValidateDateTimeRange(request.ScheduledStartAt, request.ScheduledEndAt, nameof(request.ScheduledEndAt));
        foreach (var p in request.Participants)
            ValidationHelper.Require(p.EmployeeID.HasValue || p.ClientContactID.HasValue || !string.IsNullOrWhiteSpace(p.ExternalName) || !string.IsNullOrWhiteSpace(p.ExternalEmail), nameof(request.Participants), BusinessRuleMessages.ParticipantIdentityRequired);
    }

    private static void Apply(CRMActivity entity, SaveCrmActivityRequestDto request)
    {
        entity.LeadID = request.LeadID;
        entity.ClientID = request.ClientID;
        entity.OpportunityID = request.OpportunityID;
        entity.ActivityType = request.ActivityType;
        entity.Subject = request.Subject.Trim();
        entity.Details = request.Details.NullIfWhiteSpace();
        entity.ActivityAt = request.ActivityAt;
        entity.ScheduledStartAt = request.ScheduledStartAt;
        entity.ScheduledEndAt = request.ScheduledEndAt;
        entity.LocationOrMeetingLink = request.LocationOrMeetingLink.NullIfWhiteSpace();
        entity.Agenda = request.Agenda.NullIfWhiteSpace();
        entity.ActivityStatus = request.ActivityStatus;
        entity.Outcome = request.Outcome.NullIfWhiteSpace();
        entity.NextActionAt = request.NextActionAt;
        entity.PerformedByEmployeeID = request.PerformedByEmployeeID;
    }

    private static CRMActivityParticipant CreateParticipant(int activityID, SaveCrmActivityParticipantRequestDto request) => new()
    {
        CRMActivityID = activityID,
        EmployeeID = request.EmployeeID,
        ClientContactID = request.ClientContactID,
        ExternalName = request.ExternalName.NullIfWhiteSpace(),
        ExternalEmail = request.ExternalEmail.NullIfWhiteSpace()?.NormalizeEmail(),
        ParticipantRole = request.ParticipantRole,
        AttendanceStatus = request.AttendanceStatus
    };

    private static IQueryable<CrmActivityDto> Project(IQueryable<CRMActivity> query)
        => query.Select(x => new CrmActivityDto(x.CRMActivityID, x.LeadID, x.ClientID, x.OpportunityID, x.ActivityType, x.Subject, x.Details, x.ActivityAt, x.ScheduledStartAt, x.ScheduledEndAt, x.LocationOrMeetingLink, x.Agenda, x.ActivityStatus, x.Outcome, x.NextActionAt, x.PerformedByEmployeeID,
            x.Participants.OrderBy(p => p.CRMActivityParticipantID).Select(p => new CrmActivityParticipantDto(p.CRMActivityParticipantID, p.EmployeeID, p.ClientContactID, p.ExternalName, p.ExternalEmail, p.ParticipantRole, p.AttendanceStatus)).ToList()));
}
