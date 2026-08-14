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

public sealed class CrmTaskService : ICrmTaskService
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;
    public CrmTaskService(IApplicationDbContext db, IDateTimeProvider clock) { _db = db; _clock = clock; }

    public Task<PagedResult<CrmTaskDto>> GetPagedAsync(PagedRequest request, int? assignedEmployeeID, bool overdueOnly, CancellationToken cancellationToken = default)
    {
        var query = _db.CRMTasks.AsNoTracking();
        if (assignedEmployeeID.HasValue) query = query.Where(x => x.AssignedEmployeeID == assignedEmployeeID);
        if (overdueOnly) query = query.Where(x => x.Status != CrmTaskStatus.Completed && x.Status != CrmTaskStatus.Cancelled && x.DueAt < _clock.UtcNow);
        if (!string.IsNullOrWhiteSpace(request.Search)) { var search = request.Search.Trim(); query = query.Where(x => x.Title.Contains(search) || (x.Description != null && x.Description.Contains(search))); }
        return CrmServiceHelper.ToPagedAsync(Project(query.OrderBy(x => x.Status).ThenBy(x => x.DueAt)), request, cancellationToken);
    }

    public async Task<CrmTaskDto> GetByIdAsync(int taskID, CancellationToken cancellationToken = default)
        => await Project(_db.CRMTasks.AsNoTracking().Where(x => x.CRMTaskID == taskID)).SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException("CRM task was not found.");

    public async Task<int> CreateAsync(SaveCrmTaskRequestDto request, CancellationToken cancellationToken = default)
    {
        Validate(request); await ValidateReferencesAsync(request, cancellationToken);
        var entity = new CRMTask(); Apply(entity, request);
        await _db.AddAsync(entity, cancellationToken); await _db.SaveChangesAsync(cancellationToken); return entity.CRMTaskID;
    }

    public async Task UpdateAsync(int taskID, SaveCrmTaskRequestDto request, CancellationToken cancellationToken = default)
    {
        Validate(request); await ValidateReferencesAsync(request, cancellationToken);
        var entity = await CrmServiceHelper.RequireAsync(_db.CRMTasks.Where(x => x.CRMTaskID == taskID), "CRM task", cancellationToken);
        if (entity.Status == CrmTaskStatus.Completed) throw new BusinessRuleException("A completed task cannot be edited.");
        Apply(entity, request); await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeStatusAsync(int taskID, ChangeCrmTaskStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        var entity = await CrmServiceHelper.RequireAsync(_db.CRMTasks.Where(x => x.CRMTaskID == taskID), "CRM task", cancellationToken);
        var allowed = entity.Status switch
        {
            CrmTaskStatus.Open => request.Status is CrmTaskStatus.InProgress or CrmTaskStatus.Completed or CrmTaskStatus.Cancelled,
            CrmTaskStatus.InProgress => request.Status is CrmTaskStatus.Open or CrmTaskStatus.Completed or CrmTaskStatus.Cancelled,
            _ => false
        };
        if (entity.Status != request.Status && !allowed) throw new BusinessRuleException(BusinessRuleMessages.InvalidStatusTransition);
        entity.Status = request.Status;
        entity.CompletedAt = request.Status == CrmTaskStatus.Completed ? _clock.UtcNow : null;
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateReferencesAsync(SaveCrmTaskRequestDto r, CancellationToken ct)
    {
        if (!await _db.Employees.AnyAsync(x => x.EmployeeID == r.AssignedEmployeeID, ct)) throw new NotFoundException("Assigned employee was not found.");
        if (r.LeadID.HasValue && !await _db.Leads.AnyAsync(x => x.LeadID == r.LeadID, ct)) throw new NotFoundException("Lead was not found.");
        if (r.ClientID.HasValue && !await _db.Clients.AnyAsync(x => x.ClientID == r.ClientID, ct)) throw new NotFoundException("Client was not found.");
        if (r.OpportunityID.HasValue && !await _db.Opportunities.AnyAsync(x => x.OpportunityID == r.OpportunityID, ct)) throw new NotFoundException("Opportunity was not found.");
        if (r.ComplaintID.HasValue && !await _db.Complaints.AnyAsync(x => x.ComplaintID == r.ComplaintID, ct)) throw new NotFoundException("Complaint was not found.");
        if (r.ReactivationCaseID.HasValue && !await _db.ReactivationCases.AnyAsync(x => x.ReactivationCaseID == r.ReactivationCaseID, ct)) throw new NotFoundException("Reactivation case was not found.");
    }

    private static void Validate(SaveCrmTaskRequestDto r)
    {
        ValidationHelper.RequireNotBlank(r.Title, nameof(r.Title), 200);
        CrmServiceHelper.ValidatePositiveId(r.AssignedEmployeeID, nameof(r.AssignedEmployeeID));
        if (r.ReminderAt.HasValue) ValidationHelper.Require(r.ReminderAt <= r.DueAt, nameof(r.ReminderAt), "ReminderAt must be on or before DueAt.");
    }
    private static void Apply(CRMTask e, SaveCrmTaskRequestDto r) { e.LeadID = r.LeadID; e.ClientID = r.ClientID; e.OpportunityID = r.OpportunityID; e.ComplaintID = r.ComplaintID; e.ReactivationCaseID = r.ReactivationCaseID; e.AssignedEmployeeID = r.AssignedEmployeeID; e.Title = r.Title.Trim(); e.Description = r.Description.NullIfWhiteSpace(); e.Priority = r.Priority; e.DueAt = r.DueAt; e.ReminderAt = r.ReminderAt; e.RecurrenceRule = r.RecurrenceRule.NullIfWhiteSpace(); }
    private static IQueryable<CrmTaskDto> Project(IQueryable<CRMTask> q) => q.Select(x => new CrmTaskDto(x.CRMTaskID, x.LeadID, x.ClientID, x.OpportunityID, x.ComplaintID, x.ReactivationCaseID, x.AssignedEmployeeID, x.Title, x.Description, x.Priority, x.DueAt, x.ReminderAt, x.RecurrenceRule, x.Status, x.CompletedAt, x.EscalatedAt));
}
