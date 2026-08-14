using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Infrastructure;

public sealed class ApprovalAction
{
    public int ApprovalActionID { get; set; }
    public int ApprovalRequestID { get; set; }
    public int StepNo { get; set; }
    public int ActionByUserID { get; set; }
    public ApprovalActionType Action { get; set; }
    public DateTime ActionAt { get; set; }
    public string? Note { get; set; }
    public int? DelegatedFromUserID { get; set; }

    public ApprovalRequest ApprovalRequest { get; set; } = null!;
    public User ActionByUser { get; set; } = null!;
    public User? DelegatedFromUser { get; set; }
}
