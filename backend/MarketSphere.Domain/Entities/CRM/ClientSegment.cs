using MarketSphere.Domain.Common;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.CRM;

public sealed class ClientSegment : SoftDeletableEntity
{
    public int ClientSegmentID { get; set; }
    public string SegmentCode { get; set; } = string.Empty;
    public string SegmentName { get; set; } = string.Empty;
    public ClientSegmentType SegmentType { get; set; }
    public string? Description { get; set; }
    public bool IsSystemSegment { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<ClientSegmentAssignment> Assignments { get; set; } = new List<ClientSegmentAssignment>();
}
