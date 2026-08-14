using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.MarketingField;
using MarketSphere.Domain.Entities.ProductPricing;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.CRM;

public sealed class Quotation : AuditableEntity
{
    public int QuotationID { get; set; }
    public int? RootQuotationID { get; set; }
    public int VersionNo { get; set; } = 1;
    public string QuotationNo { get; set; } = string.Empty;
    public int? OpportunityID { get; set; }
    public int ClientID { get; set; }
    public int? CampaignID { get; set; }
    public int? PriceListID { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidUntil { get; set; }
    public QuotationStatus Status { get; set; } = QuotationStatus.Draft;
    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public string? Terms { get; set; }
    public DateTime? AcceptedAt { get; set; }

    public Quotation? RootQuotation { get; set; }
    public ICollection<Quotation> Versions { get; set; } = new List<Quotation>();
    public Opportunity? Opportunity { get; set; }
    public Client Client { get; set; } = null!;
    public Campaign? Campaign { get; set; }
    public PriceList? PriceList { get; set; }
    public ICollection<QuotationItem> Items { get; set; } = new List<QuotationItem>();
}
