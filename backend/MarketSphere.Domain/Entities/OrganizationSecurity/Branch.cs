using MarketSphere.Domain.Common;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.OrganizationSecurity;

public class Branch : SoftDeletableEntity
{
    public int BranchID { get; set; }
    public int CompanyID { get; set; }
    public string BranchCode { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public BranchType BranchType { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
