using MarketSphere.Domain.Common;

namespace MarketSphere.Domain.Entities.OrganizationSecurity;

public class Designation : SoftDeletableEntity
{
    public int DesignationID { get; set; }
    public string DesignationCode { get; set; } = string.Empty;
    public string DesignationName { get; set; } = string.Empty;
    public int HierarchyLevel { get; set; }
    public bool IsFieldRole { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
