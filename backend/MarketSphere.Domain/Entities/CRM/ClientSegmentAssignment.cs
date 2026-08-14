using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrganizationSecurity;

namespace MarketSphere.Domain.Entities.CRM;

public sealed class ClientSegmentAssignment : AuditableEntity
{
    public int ClientSegmentAssignmentID { get; set; }
    public int ClientID { get; set; }
    public int ClientSegmentID { get; set; }
    public DateTime AssignedAt { get; set; }
    public int AssignedByUserID { get; set; }
    public DateTime? EffectiveTo { get; set; }

    public Client Client { get; set; } = null!;
    public ClientSegment ClientSegment { get; set; } = null!;
    public User AssignedByUser { get; set; } = null!;
}
