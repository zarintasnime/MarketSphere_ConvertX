using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Entities.OrderFulfilment;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.MarketingField;

public sealed class CampaignAttribution : AuditableEntity
{
    public int CampaignAttributionID { get; set; }
    public int CampaignID { get; set; }
    public int? LeadID { get; set; }
    public int? OpportunityID { get; set; }
    public int? QuotationID { get; set; }
    public int? OrderID { get; set; }
    public CampaignAttributionType AttributionType { get; set; }
    public decimal WeightPercent { get; set; }
    public decimal? AttributedAmount { get; set; }

    public Campaign Campaign { get; set; } = null!;
    public Lead? Lead { get; set; }
    public Opportunity? Opportunity { get; set; }
    public Quotation? Quotation { get; set; }
    public Order? Order { get; set; }
}
