using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.MarketingField;
using MarketSphere.Domain.Entities.OrderFulfilment;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.CRM;

public sealed class ReactivationCase : AuditableEntity
{
    public int ReactivationCaseID { get; set; }
    public int ClientID { get; set; }
    public DateTime InactiveAt { get; set; }
    public string? ChurnReason { get; set; }
    public int? CampaignID { get; set; }
    public int AssignedEmployeeID { get; set; }
    public DateTime OpenedAt { get; set; }
    public ReactivationCaseStatus Status { get; set; } = ReactivationCaseStatus.Open;
    public ReactivationResult? ReactivationResult { get; set; }
    public DateTime? ReactivatedAt { get; set; }
    public int? RepeatOrderID { get; set; }

    public Client Client { get; set; } = null!;
    public Employee AssignedEmployee { get; set; } = null!;
    public Campaign? Campaign { get; set; }
    public Order? RepeatOrder { get; set; }
}
