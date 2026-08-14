using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.CRM.DTOs;

public sealed record ComplaintListDto(int ComplaintID, string ComplaintNo, int ClientID, ComplaintCategory ComplaintCategory, ComplaintPriority Priority, string Subject, ComplaintStatus Status, DateTime OpenedAt, DateTime? SLADueAt, int? AssignedEmployeeID);
public sealed record ComplaintDetailsDto(int ComplaintID, string ComplaintNo, int ClientID, int? OrderID, int? InvoiceID, int? DeliveryID, ComplaintCategory ComplaintCategory, ComplaintPriority Priority, string Subject, string Details, int? AssignedEmployeeID, ComplaintStatus Status, DateTime OpenedAt, DateTime? SLADueAt, DateTime? ResolvedAt, string? ResolutionNote, int? SatisfactionScore);
public sealed class SaveComplaintRequestDto { public string ComplaintNo { get; init; } = string.Empty; public int ClientID { get; init; } public int? OrderID { get; init; } public int? InvoiceID { get; init; } public int? DeliveryID { get; init; } public ComplaintCategory ComplaintCategory { get; init; } public ComplaintPriority Priority { get; init; } = ComplaintPriority.Normal; public string Subject { get; init; } = string.Empty; public string Details { get; init; } = string.Empty; public int? AssignedEmployeeID { get; init; } public DateTime? SLADueAt { get; init; } }
public sealed class ChangeComplaintStatusRequestDto { public ComplaintStatus Status { get; init; } public string? ResolutionNote { get; init; } public int? SatisfactionScore { get; init; } }
