namespace MarketSphere.Domain.Common;

public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public int? CreatedByUserID { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedByUserID { get; set; }
}
