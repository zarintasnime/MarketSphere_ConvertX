using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Entities.ProductPricing;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.MarketingField;

public sealed class MarketObservation : AuditableEntity
{
    public int MarketObservationID { get; set; }
    public int VisitID { get; set; }
    public int ClientID { get; set; }
    public int EmployeeID { get; set; }
    public MarketObservationType ObservationType { get; set; }
    public int? SKUID { get; set; }
    public AvailabilityStatus? AvailabilityStatus { get; set; }
    public int? FacingCount { get; set; }
    public decimal? PlanogramScore { get; set; }
    public decimal? DisplayScore { get; set; }
    public string? CompetitorBrand { get; set; }
    public string? CompetitorProduct { get; set; }
    public decimal? CompetitorPrice { get; set; }
    public string? CompetitorOffer { get; set; }
    public string? Note { get; set; }

    public Visit Visit { get; set; } = null!;
    public Client Client { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
    public SKU? SKU { get; set; }
}
