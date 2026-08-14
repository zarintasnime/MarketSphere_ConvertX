using MarketSphere.Domain.Common;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.MarketingField;

public sealed class CampaignExpense : AuditableEntity
{
    public int CampaignExpenseID { get; set; }
    public int CampaignID { get; set; }
    public DateOnly ExpenseDate { get; set; }
    public string ExpenseCategory { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? VendorName { get; set; }
    public string? Description { get; set; }
    public CampaignExpenseStatus Status { get; set; } = CampaignExpenseStatus.Draft;

    public Campaign Campaign { get; set; } = null!;
}
