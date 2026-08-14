using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.MarketingField;

public sealed class Visit : AuditableEntity
{
    public int VisitID { get; set; }
    public int EmployeeID { get; set; }
    public int ClientID { get; set; }
    public int? RouteID { get; set; }
    public int? CampaignID { get; set; }
    public VisitType VisitType { get; set; }
    public DateTime CheckInAt { get; set; }
    public DateTime? CheckOutAt { get; set; }
    public decimal CheckInGPSLat { get; set; }
    public decimal CheckInGPSLng { get; set; }
    public decimal? CheckOutGPSLat { get; set; }
    public decimal? CheckOutGPSLng { get; set; }
    public decimal? AccuracyMeters { get; set; }
    public bool IsSuspiciousLocation { get; set; }
    public string? Note { get; set; }
    public VisitStatus Status { get; set; } = VisitStatus.CheckedIn;

    public Employee Employee { get; set; } = null!;
    public Client Client { get; set; } = null!;
    public Route? Route { get; set; }
    public Campaign? Campaign { get; set; }
    public ICollection<SamplingLog> SamplingLogs { get; set; } = new List<SamplingLog>();
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    public ICollection<MarketObservation> MarketObservations { get; set; } = new List<MarketObservation>();
    public ICollection<BPSellOut> BPSellOuts { get; set; } = new List<BPSellOut>();
}
