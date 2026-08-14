using MarketSphere.Domain.Common;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Inventory;

public sealed class Warehouse : SoftDeletableEntity
{
    public int WarehouseID { get; set; }
    public int BranchID { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public WarehouseType WarehouseType { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    public Branch Branch { get; set; } = null!;
}
