using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.CRM.DTOs;

public sealed record CrmTaskDto(int CRMTaskID, int? LeadID, int? ClientID, int? OpportunityID, int? ComplaintID, int? ReactivationCaseID, int AssignedEmployeeID, string Title, string? Description, TaskPriority Priority, DateTime DueAt, DateTime? ReminderAt, string? RecurrenceRule, CrmTaskStatus Status, DateTime? CompletedAt, DateTime? EscalatedAt);
public class SaveCrmTaskRequestDto { public int? LeadID { get; init; } public int? ClientID { get; init; } public int? OpportunityID { get; init; } public int? ComplaintID { get; init; } public int? ReactivationCaseID { get; init; } public int AssignedEmployeeID { get; init; } public string Title { get; init; } = string.Empty; public string? Description { get; init; } public TaskPriority Priority { get; init; } = TaskPriority.Normal; public DateTime DueAt { get; init; } public DateTime? ReminderAt { get; init; } public string? RecurrenceRule { get; init; } }
public sealed class ChangeCrmTaskStatusRequestDto { public CrmTaskStatus Status { get; init; } }
