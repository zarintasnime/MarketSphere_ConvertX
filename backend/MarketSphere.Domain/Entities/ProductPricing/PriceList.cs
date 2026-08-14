using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.ProductPricing;

public sealed class PriceList : AuditableEntity
{
    public int PriceListID { get; set; }
    public string PriceListCode { get; set; } = string.Empty;
    public string PriceListName { get; set; } = string.Empty;
    public SalesChannel Channel { get; set; }
    public int? ClientSegmentID { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public string CurrencyCode { get; set; } = "BDT";
    public PriceListStatus Status { get; set; } = PriceListStatus.Draft;

    public ClientSegment? ClientSegment { get; set; }
    public ICollection<PriceListItem> Items { get; set; } = new List<PriceListItem>();
}
