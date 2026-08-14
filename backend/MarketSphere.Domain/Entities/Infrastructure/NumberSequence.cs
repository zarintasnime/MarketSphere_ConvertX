using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrganizationSecurity;

namespace MarketSphere.Domain.Entities.Infrastructure;

public sealed class NumberSequence : AuditableEntity, IHasRowVersion
{
    public int NumberSequenceID { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public int? YearValue { get; set; }
    public int? BranchID { get; set; }
    public long LastNumber { get; set; }
    public int PaddingLength { get; set; } = 6;
    public string ResetPolicy { get; set; } = "YEARLY";
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Branch? Branch { get; set; }
}
