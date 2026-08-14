using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrderFulfilment;

public sealed class PaymentAllocation : AuditableEntity
{
    public int PaymentAllocationID { get; set; }
    public int PaymentID { get; set; }
    public int InvoiceID { get; set; }
    public PaymentAllocationType AllocationType { get; set; }
    public decimal AllocatedAmount { get; set; }
    public int? ReversalOfPaymentAllocationID { get; set; }
    public DateTime AllocatedAt { get; set; }
    public int AllocatedByUserID { get; set; }

    public Payment Payment { get; set; } = null!;
    public Invoice Invoice { get; set; } = null!;
    public PaymentAllocation? ReversalOfPaymentAllocation { get; set; }
    public ICollection<PaymentAllocation> Reversals { get; set; } = new List<PaymentAllocation>();
    public User AllocatedByUser { get; set; } = null!;
}
