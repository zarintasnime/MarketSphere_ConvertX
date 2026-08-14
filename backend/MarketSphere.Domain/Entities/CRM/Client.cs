using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.CRM;

public sealed class Client : SoftDeletableEntity
{
    public int ClientID { get; set; }
    public string ClientCode { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public ClientType ClientType { get; set; }
    public SalesChannel Channel { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string Address { get; set; } = string.Empty;
    public decimal? GPSLat { get; set; }
    public decimal? GPSLng { get; set; }
    public int? RegionID { get; set; }
    public int? AreaID { get; set; }
    public int? TerritoryID { get; set; }
    public ClientLifecycleStatus LifecycleStatus { get; set; } = ClientLifecycleStatus.Active;
    public ClientRiskStatus RiskStatus { get; set; } = ClientRiskStatus.Normal;
    public DateTime? LastOrderAt { get; set; }
    public bool IsActive { get; set; } = true;

    public Region? Region { get; set; }
    public Area? Area { get; set; }
    public Territory? Territory { get; set; }
    public ICollection<ClientContact> Contacts { get; set; } = new List<ClientContact>();
    public ClientCreditProfile? CreditProfile { get; set; }
    public ICollection<ClientSegmentAssignment> SegmentAssignments { get; set; } = new List<ClientSegmentAssignment>();
    public ICollection<RouteOutlet> RouteOutlets { get; set; } = new List<RouteOutlet>();
}
