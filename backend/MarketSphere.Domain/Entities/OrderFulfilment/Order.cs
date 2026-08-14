using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Entities.Infrastructure;
using MarketSphere.Domain.Entities.MarketingField;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Entities.ProductPricing;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrderFulfilment;

public sealed class Order : AuditableEntity, IHasRowVersion
{
    public int OrderID { get; set; }
    public string OrderNo { get; set; } = string.Empty;
    public int ClientID { get; set; }
    public int? EmployeeID { get; set; }
    public SalesChannel Channel { get; set; }
    public OrderSource OrderSource { get; set; }
    public int? CampaignID { get; set; }
    public int? QuotationID { get; set; }
    public int? ModernTradePurchaseOrderID { get; set; }
    public int? PriceListID { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? RequestedDeliveryDate { get; set; }
    public string DeliveryAddressSnapshot { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public CreditCheckStatus CreditCheckStatus { get; set; } = CreditCheckStatus.Pending;
    public int? ApprovalRequestID { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Client Client { get; set; } = null!;
    public Employee? Employee { get; set; }
    public Campaign? Campaign { get; set; }
    public Quotation? Quotation { get; set; }
    public ModernTradePurchaseOrder? ModernTradePurchaseOrder { get; set; }
    public PriceList? PriceList { get; set; }
    public ApprovalRequest? ApprovalRequest { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<AppliedOffer> AppliedOffers { get; set; } = new List<AppliedOffer>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<PickList> PickLists { get; set; } = new List<PickList>();
    public ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
}
