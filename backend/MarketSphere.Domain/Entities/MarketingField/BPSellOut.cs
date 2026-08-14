using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.MarketingField;

public sealed class BPSellOut : AuditableEntity
{
    public int BPSellOutID { get; set; }
    public int EmployeeID { get; set; }
    public int ClientID { get; set; }
    public int? VisitID { get; set; }
    public int? CampaignID { get; set; }
    public DateOnly SellOutDate { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalValue { get; set; }
    public decimal? GPSLat { get; set; }
    public decimal? GPSLng { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public int? VerifiedByEmployeeID { get; set; }
    public DateTime? VerifiedAt { get; set; }

    public Employee Employee { get; set; } = null!;
    public Client Client { get; set; } = null!;
    public Visit? Visit { get; set; }
    public Campaign? Campaign { get; set; }
    public Employee? VerifiedByEmployee { get; set; }
    public ICollection<BPSellOutItem> Items { get; set; } = new List<BPSellOutItem>();
}
