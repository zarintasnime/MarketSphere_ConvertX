using MarketSphere.Domain.Common;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Procurement;

public sealed class Supplier : SoftDeletableEntity
{
    public int SupplierID { get; set; }
    public string SupplierCode { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public int PaymentTermsDays { get; set; }
    public SupplierStatus Status { get; set; } = SupplierStatus.Active;

    public ICollection<SupplierProduct> SupplierProducts { get; set; } = new List<SupplierProduct>();
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}
