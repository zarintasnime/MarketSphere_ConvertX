using MarketSphere.Domain.Common;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Infrastructure;

public sealed class ApprovalStepDefinition : AuditableEntity
{
    public int ApprovalStepDefinitionID { get; set; }
    public int ApprovalPolicyID { get; set; }
    public int StepNo { get; set; }
    public string StepName { get; set; } = string.Empty;
    public ApprovalMode ApprovalMode { get; set; }
    public int MinimumApprovals { get; set; } = 1;
    public bool IsFinalStep { get; set; }
    public int? EscalationHours { get; set; }

    public ApprovalPolicy ApprovalPolicy { get; set; } = null!;
    public ICollection<ApprovalStepAssignee> Assignees { get; set; } = new List<ApprovalStepAssignee>();
}
