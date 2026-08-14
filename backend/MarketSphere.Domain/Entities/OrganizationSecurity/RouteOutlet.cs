using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrganizationSecurity;

public sealed class RouteOutlet : AuditableEntity
{
    public int RouteOutletID { get; set; }

    public int RouteID { get; set; }

    public int ClientID { get; set; }

    public int SequenceNo { get; set; }

    public VisitFrequency VisitFrequency { get; set; }
        = VisitFrequency.Daily;

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public Route Route { get; set; } = null!;

    public Client Client { get; set; } = null!;
}