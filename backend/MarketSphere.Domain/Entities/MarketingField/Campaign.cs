using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.MarketingField;

public sealed class Campaign : AuditableEntity
{
    public int CampaignID { get; set; }
    public string CampaignCode { get; set; } = string.Empty;
    public string CampaignTitle { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public SalesChannel Channel { get; set; }
    public CampaignStatus Status { get; set; } = CampaignStatus.Draft;
    public int CreatedByEmployeeID { get; set; }
    public decimal ActualExpense { get; set; }

    public Employee CreatedByEmployee { get; set; } = null!;
    public ICollection<CampaignTarget> Targets { get; set; } = new List<CampaignTarget>();
    public ICollection<CampaignOffer> Offers { get; set; } = new List<CampaignOffer>();
    public ICollection<CampaignExpense> Expenses { get; set; } = new List<CampaignExpense>();
    public ICollection<CampaignAttribution> Attributions { get; set; } = new List<CampaignAttribution>();
    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
    public ICollection<BPSellOut> BPSellOuts { get; set; } = new List<BPSellOut>();
}
