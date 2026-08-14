using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Infrastructure;

public sealed class ApprovalRequest : AuditableEntity, IHasRowVersion
{
    public int ApprovalRequestID { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public int ReferenceID { get; set; }
    public ApprovalType ApprovalType { get; set; }
    public int ApprovalPolicyID { get; set; }
    public int RequestedByUserID { get; set; }
    public DateTime RequestedAt { get; set; }
    public int CurrentStepNo { get; set; } = 1;
    public ApprovalRequestStatus Status { get; set; } = ApprovalRequestStatus.Pending;
    public DateTime? CompletedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public ApprovalPolicy ApprovalPolicy { get; set; } = null!;
    public User RequestedByUser { get; set; } = null!;
    public ICollection<ApprovalAction> Actions { get; set; } = new List<ApprovalAction>();
}
