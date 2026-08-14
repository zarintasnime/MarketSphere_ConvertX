using Microsoft.EntityFrameworkCore;
using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Infrastructure.DTOs;
using MarketSphere.Application.Modules.Infrastructure.Interfaces;
using MarketSphere.Domain.Entities.Infrastructure;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;

namespace MarketSphere.Application.Modules.Infrastructure.Services;

public sealed class ApprovalService : IApprovalService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _clock;

    public ApprovalService(IApplicationDbContext db, ICurrentUserService currentUser, IDateTimeProvider clock) { _db = db; _currentUser = currentUser; _clock = clock; }

    public Task<PagedResult<ApprovalRequestDto>> GetQueueAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var query = _db.ApprovalRequests.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search)) { var search = request.Search.Trim(); query = query.Where(x => x.ReferenceType.Contains(search)); }
        var projected = query.OrderBy(x => x.Status).ThenByDescending(x => x.RequestedAt).Select(x => new ApprovalRequestDto(x.ApprovalRequestID, x.ReferenceType, x.ReferenceID, x.ApprovalType, x.ApprovalPolicyID, x.RequestedByUserID, x.RequestedAt, x.CurrentStepNo, x.Status, x.CompletedAt, x.Actions.OrderBy(a => a.ActionAt).Select(a => new ApprovalActionDto(a.ApprovalActionID, a.StepNo, a.ActionByUserID, a.ActionByUser.FullName, a.Action, a.ActionAt, a.Note, a.DelegatedFromUserID)).ToArray()));
        return InfrastructureServiceHelper.ToPagedAsync(projected, request, cancellationToken);
    }

    public async Task<ApprovalRequestDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.ApprovalRequests.AsNoTracking().Where(x => x.ApprovalRequestID == id).Select(x => new ApprovalRequestDto(x.ApprovalRequestID, x.ReferenceType, x.ReferenceID, x.ApprovalType, x.ApprovalPolicyID, x.RequestedByUserID, x.RequestedAt, x.CurrentStepNo, x.Status, x.CompletedAt, x.Actions.OrderBy(a => a.ActionAt).Select(a => new ApprovalActionDto(a.ApprovalActionID, a.StepNo, a.ActionByUserID, a.ActionByUser.FullName, a.Action, a.ActionAt, a.Note, a.DelegatedFromUserID)).ToArray())).SingleOrDefaultAsync(cancellationToken);
        return entity ?? throw new NotFoundException("Approval request was not found.");
    }

    public async Task<int> SavePolicyAsync(int? id, SaveApprovalPolicyRequestDto request, CancellationToken cancellationToken = default)
        => await _db.ExecuteInTransactionAsync(async ct =>
        {
            ValidatePolicy(request);
            ApprovalPolicy entity;
            if (id.HasValue)
            {
                entity = await InfrastructureServiceHelper.RequireAsync(_db.ApprovalPolicies.Where(x => x.ApprovalPolicyID == id), "Approval policy", ct);
                if (await _db.ApprovalRequests.AnyAsync(x => x.ApprovalPolicyID == id && (x.Status == ApprovalRequestStatus.Pending || x.Status == ApprovalRequestStatus.InProgress), ct)) throw new BusinessRuleException("A policy with active approval requests cannot be structurally edited.");
                var oldSteps = await _db.ApprovalStepDefinitions.Where(x => x.ApprovalPolicyID == id).ToListAsync(ct);
                var oldStepIDs = oldSteps.Select(x => x.ApprovalStepDefinitionID).ToArray();
                var oldAssignees = await _db.ApprovalStepAssignees.Where(x => oldStepIDs.Contains(x.ApprovalStepDefinitionID)).ToListAsync(ct);
                foreach (var item in oldAssignees) _db.Remove(item); foreach (var item in oldSteps) _db.Remove(item);
            }
            else { entity = new ApprovalPolicy(); await _db.AddAsync(entity, ct); }
            ApplyPolicy(entity, request); await _db.SaveChangesAsync(ct);
            foreach (var stepRequest in request.Steps.OrderBy(x => x.StepNo))
            {
                var step = new ApprovalStepDefinition { ApprovalPolicyID = entity.ApprovalPolicyID, StepNo = stepRequest.StepNo, StepName = stepRequest.StepName.Trim(), ApprovalMode = stepRequest.ApprovalMode, MinimumApprovals = stepRequest.MinimumApprovals, IsFinalStep = stepRequest.IsFinalStep, EscalationHours = stepRequest.EscalationHours };
                await _db.AddAsync(step, ct); await _db.SaveChangesAsync(ct);
                foreach (var assigneeRequest in stepRequest.Assignees) await _db.AddAsync(new ApprovalStepAssignee { ApprovalStepDefinitionID = step.ApprovalStepDefinitionID, AssigneeType = assigneeRequest.AssigneeType, RoleID = assigneeRequest.RoleID, DesignationID = assigneeRequest.DesignationID, UserID = assigneeRequest.UserID, EmployeeID = assigneeRequest.EmployeeID, IsFallback = assigneeRequest.IsFallback, Priority = assigneeRequest.Priority }, ct);
            }
            await _db.SaveChangesAsync(ct); return entity.ApprovalPolicyID;
        }, cancellationToken);

    public async Task<IReadOnlyCollection<ApprovalPolicyDto>> GetPoliciesAsync(CancellationToken cancellationToken = default)
        => await _db.ApprovalPolicies.AsNoTracking().OrderBy(x => x.ApprovalType).ThenByDescending(x => x.Priority).Select(x => new ApprovalPolicyDto(x.ApprovalPolicyID, x.ApprovalType, x.EntityType, x.BranchID, x.Channel, x.MinimumAmount, x.MaximumAmount, x.MinimumDiscountPercent, x.MaximumDiscountPercent, x.EffectiveFrom, x.EffectiveTo, x.Priority, x.IsActive, x.Steps.OrderBy(s => s.StepNo).Select(s => new ApprovalStepDto(s.ApprovalStepDefinitionID, s.StepNo, s.StepName, s.ApprovalMode, s.MinimumApprovals, s.IsFinalStep, s.EscalationHours, s.Assignees.OrderBy(a => a.Priority).Select(a => new ApprovalAssigneeDto(a.ApprovalStepAssigneeID, a.AssigneeType, a.RoleID, a.DesignationID, a.UserID, a.EmployeeID, a.IsFallback, a.Priority)).ToArray())).ToArray())).ToListAsync(cancellationToken);

    public async Task<int> CreateRequestAsync(CreateApprovalRequestDto request, CancellationToken cancellationToken = default)
        => await _db.ExecuteInTransactionAsync(async ct =>
        {
            var userID = RequireUser(); var referenceType = InfrastructureServiceHelper.Required(request.ReferenceType, "Reference type", 100).ToUpperInvariant();
            if (request.ReferenceID <= 0) throw new BusinessRuleException("Reference ID must be greater than zero.");
            if (await _db.ApprovalRequests.AnyAsync(x => x.ReferenceType == referenceType && x.ReferenceID == request.ReferenceID && x.ApprovalType == request.ApprovalType && (x.Status == ApprovalRequestStatus.Pending || x.Status == ApprovalRequestStatus.InProgress), ct)) throw new ConflictException("An active approval request already exists for this reference.");
            var now = _clock.UtcNow;
            var policies = await _db.ApprovalPolicies.AsNoTracking().Where(x => x.IsActive && x.ApprovalType == request.ApprovalType && x.EntityType == referenceType && x.EffectiveFrom <= now && (!x.EffectiveTo.HasValue || x.EffectiveTo >= now) && (!x.BranchID.HasValue || x.BranchID == request.BranchID) && (!x.Channel.HasValue || x.Channel == request.Channel) && (!x.MinimumAmount.HasValue || x.MinimumAmount <= request.Amount) && (!x.MaximumAmount.HasValue || x.MaximumAmount >= request.Amount) && (!x.MinimumDiscountPercent.HasValue || x.MinimumDiscountPercent <= request.DiscountPercent) && (!x.MaximumDiscountPercent.HasValue || x.MaximumDiscountPercent >= request.DiscountPercent)).OrderByDescending(x => x.BranchID.HasValue).ThenByDescending(x => x.Channel.HasValue).ThenByDescending(x => x.Priority).ToListAsync(ct);
            var policy = policies.FirstOrDefault() ?? throw new BusinessRuleException("No active approval policy matches this request.");
            if (!await _db.ApprovalStepDefinitions.AnyAsync(x => x.ApprovalPolicyID == policy.ApprovalPolicyID && x.StepNo == 1, ct)) throw new BusinessRuleException("The selected approval policy has no first step.");
            var entity = new ApprovalRequest { ReferenceType = referenceType, ReferenceID = request.ReferenceID, ApprovalType = request.ApprovalType, ApprovalPolicyID = policy.ApprovalPolicyID, RequestedByUserID = userID, RequestedAt = now, CurrentStepNo = 1, Status = ApprovalRequestStatus.Pending };
            await _db.AddAsync(entity, ct); await _db.SaveChangesAsync(ct);
            await _db.AddAsync(new ApprovalAction { ApprovalRequestID = entity.ApprovalRequestID, StepNo = 0, ActionByUserID = userID, Action = ApprovalActionType.Submitted, ActionAt = now }, ct);
            await _db.SaveChangesAsync(ct); return entity.ApprovalRequestID;
        }, cancellationToken);

    public async Task ActAsync(int id, ApprovalActionRequestDto request, CancellationToken cancellationToken = default)
        => await _db.ExecuteInTransactionAsync(async ct =>
        {
            var userID = RequireUser();
            var entity = await _db.ApprovalRequests.SingleOrDefaultAsync(x => x.ApprovalRequestID == id, ct) ?? throw new NotFoundException("Approval request was not found.");
            if (entity.Status is not (ApprovalRequestStatus.Pending or ApprovalRequestStatus.InProgress)) throw new BusinessRuleException("Only an active approval request can receive an action.");
            var step = await _db.ApprovalStepDefinitions.AsNoTracking().SingleOrDefaultAsync(x => x.ApprovalPolicyID == entity.ApprovalPolicyID && x.StepNo == entity.CurrentStepNo, ct) ?? throw new BusinessRuleException("Current approval step was not found.");
            await EnsureAuthorizedAsync(step.ApprovalStepDefinitionID, userID, ct);
            if (request.Action == ApprovalActionType.Delegated)
            {
                if (!request.DelegateToUserID.HasValue || !await _db.Users.AnyAsync(x => x.UserID == request.DelegateToUserID && x.Status == UserStatus.Active, ct)) throw new BusinessRuleException("Active delegate user is required.");
                await _db.AddAsync(new ApprovalStepAssignee { ApprovalStepDefinitionID = step.ApprovalStepDefinitionID, AssigneeType = ApprovalAssigneeType.User, UserID = request.DelegateToUserID, Priority = int.MaxValue }, ct);
            }
            var action = new ApprovalAction { ApprovalRequestID = id, StepNo = entity.CurrentStepNo, ActionByUserID = userID, Action = request.Action, ActionAt = _clock.UtcNow, Note = request.Note?.Trim(), DelegatedFromUserID = request.Action == ApprovalActionType.Delegated ? userID : null };
            await _db.AddAsync(action, ct);
            if (request.Action == ApprovalActionType.Rejected) { entity.Status = ApprovalRequestStatus.Rejected; entity.CompletedAt = _clock.UtcNow; }
            else if (request.Action == ApprovalActionType.Approved)
            {
                var previousApprovals = await _db.ApprovalActions.CountAsync(x => x.ApprovalRequestID == id && x.StepNo == entity.CurrentStepNo && x.Action == ApprovalActionType.Approved && x.ActionByUserID != userID, ct);
                var needed = step.ApprovalMode == ApprovalMode.AnyOne ? 1 : step.MinimumApprovals;
                if (previousApprovals + 1 >= needed)
                {
                    if (step.IsFinalStep) { entity.Status = ApprovalRequestStatus.Approved; entity.CompletedAt = _clock.UtcNow; }
                    else { entity.CurrentStepNo++; entity.Status = ApprovalRequestStatus.InProgress; }
                }
                else entity.Status = ApprovalRequestStatus.InProgress;
            }
            await _db.SaveChangesAsync(ct); return true;
        }, cancellationToken);

    public async Task CancelAsync(int id, string? note, CancellationToken cancellationToken = default)
    {
        var userID = RequireUser(); var entity = await InfrastructureServiceHelper.RequireAsync(_db.ApprovalRequests.Where(x => x.ApprovalRequestID == id), "Approval request", cancellationToken); if (entity.RequestedByUserID != userID) throw new ForbiddenBusinessActionException("Only the requester can cancel this approval request."); if (entity.Status is not (ApprovalRequestStatus.Pending or ApprovalRequestStatus.InProgress)) throw new BusinessRuleException("Only an active approval request can be cancelled."); entity.Status = ApprovalRequestStatus.Cancelled; entity.CompletedAt = _clock.UtcNow; await _db.AddAsync(new ApprovalAction { ApprovalRequestID = id, StepNo = entity.CurrentStepNo, ActionByUserID = userID, Action = ApprovalActionType.Cancelled, ActionAt = _clock.UtcNow, Note = note?.Trim() }, cancellationToken); await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureAuthorizedAsync(int stepID, int userID, CancellationToken cancellationToken)
    {
        var employee = await _db.Employees.AsNoTracking().Where(x => x.UserID == userID).Select(x => new { x.EmployeeID, x.DesignationID }).FirstOrDefaultAsync(cancellationToken);
        var roleIDs = await _db.UserRoles.AsNoTracking().Where(x => x.UserID == userID).Select(x => x.RoleID).ToListAsync(cancellationToken);
        var allowed = await _db.ApprovalStepAssignees.AnyAsync(x => x.ApprovalStepDefinitionID == stepID && ((x.AssigneeType == ApprovalAssigneeType.User && x.UserID == userID) || (x.AssigneeType == ApprovalAssigneeType.Role && x.RoleID.HasValue && roleIDs.Contains(x.RoleID.Value)) || (employee != null && x.AssigneeType == ApprovalAssigneeType.Employee && x.EmployeeID == employee.EmployeeID) || (employee != null && x.AssigneeType == ApprovalAssigneeType.Designation && x.DesignationID == employee.DesignationID)), cancellationToken);
        if (!allowed) throw new ForbiddenBusinessActionException("The current user is not an assignee for this approval step.");
    }

    private int RequireUser() => _currentUser.UserID ?? throw new ForbiddenBusinessActionException("Authenticated user is required.");
    private static void ValidatePolicy(SaveApprovalPolicyRequestDto request) { if (string.IsNullOrWhiteSpace(request.EntityType)) throw new BusinessRuleException("Entity type is required."); if (request.EffectiveTo < request.EffectiveFrom) throw new BusinessRuleException("Effective end cannot be earlier than effective start."); if (request.MaximumAmount < request.MinimumAmount || request.MaximumDiscountPercent < request.MinimumDiscountPercent) throw new BusinessRuleException("Policy threshold range is invalid."); if (request.Steps.Count == 0) throw new BusinessRuleException("At least one approval step is required."); var ordered = request.Steps.OrderBy(x => x.StepNo).ToArray(); if (ordered[0].StepNo != 1 || ordered.Select(x => x.StepNo).Distinct().Count() != ordered.Length || ordered.Where((x, index) => x.StepNo != index + 1).Any()) throw new BusinessRuleException("Approval steps must be unique and sequential starting at one."); if (ordered.Count(x => x.IsFinalStep) != 1 || !ordered[^1].IsFinalStep) throw new BusinessRuleException("Only the last step must be marked as final."); foreach (var step in ordered) { if (string.IsNullOrWhiteSpace(step.StepName) || step.MinimumApprovals < 1 || step.Assignees.Count == 0) throw new BusinessRuleException("Each approval step requires a name, minimum approval count and assignees."); foreach (var assignee in step.Assignees) { var count = new[] { assignee.RoleID, assignee.DesignationID, assignee.UserID, assignee.EmployeeID }.Count(x => x.HasValue); if (count != 1) throw new BusinessRuleException("Each approval assignee must contain exactly one assignee reference."); } } }
    private static void ApplyPolicy(ApprovalPolicy entity, SaveApprovalPolicyRequestDto request) { entity.ApprovalType = request.ApprovalType; entity.EntityType = request.EntityType.Trim().ToUpperInvariant(); entity.BranchID = request.BranchID; entity.Channel = request.Channel; entity.MinimumAmount = request.MinimumAmount; entity.MaximumAmount = request.MaximumAmount; entity.MinimumDiscountPercent = request.MinimumDiscountPercent; entity.MaximumDiscountPercent = request.MaximumDiscountPercent; entity.EffectiveFrom = request.EffectiveFrom; entity.EffectiveTo = request.EffectiveTo; entity.Priority = request.Priority; entity.IsActive = request.IsActive; }
}
