using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Infrastructure;

public sealed class ApprovalPolicy : AuditableEntity
{
    public int ApprovalPolicyID { get; set; }
    public ApprovalType ApprovalType { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int? BranchID { get; set; }
    public SalesChannel? Channel { get; set; }
    public decimal? MinimumAmount { get; set; }
    public decimal? MaximumAmount { get; set; }
    public decimal? MinimumDiscountPercent { get; set; }
    public decimal? MaximumDiscountPercent { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public int Priority { get; set; }
    public bool IsActive { get; set; } = true;

    public Branch? Branch { get; set; }
    public ICollection<ApprovalStepDefinition> Steps { get; set; } = new List<ApprovalStepDefinition>();
    public ICollection<ApprovalRequest> Requests { get; set; } = new List<ApprovalRequest>();
}
