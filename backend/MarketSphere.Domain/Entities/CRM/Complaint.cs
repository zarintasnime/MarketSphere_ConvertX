using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrderFulfilment;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.CRM;

public sealed class Complaint : AuditableEntity
{
    public int ComplaintID { get; set; }
    public string ComplaintNo { get; set; } = string.Empty;
    public int ClientID { get; set; }
    public int? OrderID { get; set; }
    public int? InvoiceID { get; set; }
    public int? DeliveryID { get; set; }
    public ComplaintCategory ComplaintCategory { get; set; }
    public ComplaintPriority Priority { get; set; } = ComplaintPriority.Normal;
    public string Subject { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public int? AssignedEmployeeID { get; set; }
    public ComplaintStatus Status { get; set; } = ComplaintStatus.Open;
    public DateTime OpenedAt { get; set; }
    public DateTime? SLADueAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolutionNote { get; set; }
    public int? SatisfactionScore { get; set; }

    public Client Client { get; set; } = null!;
    public Order? Order { get; set; }
    public Invoice? Invoice { get; set; }
    public Delivery? Delivery { get; set; }
    public Employee? AssignedEmployee { get; set; }
}
