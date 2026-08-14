using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Entities.Infrastructure;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrderFulfilment;

public sealed class Payment : AuditableEntity
{
    public int PaymentID { get; set; }
    public string PaymentNo { get; set; } = string.Empty;
    public int ClientID { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public decimal Amount { get; set; }
    public string? ReferenceNo { get; set; }
    public int? ProofFileAttachmentID { get; set; }
    public CustomerPaymentStatus Status { get; set; } = CustomerPaymentStatus.Pending;
    public int ReceivedByUserID { get; set; }

    public Client Client { get; set; } = null!;
    public User ReceivedByUser { get; set; } = null!;
    public FileAttachment? ProofFileAttachment { get; set; }
    public ICollection<PaymentAllocation> Allocations { get; set; } = new List<PaymentAllocation>();
}
