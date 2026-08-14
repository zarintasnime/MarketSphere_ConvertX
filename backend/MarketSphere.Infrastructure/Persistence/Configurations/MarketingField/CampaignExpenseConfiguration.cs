using MarketSphere.Domain.Entities.MarketingField;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.MarketingField;

public sealed class CampaignExpenseConfiguration : IEntityTypeConfiguration<CampaignExpense>
{
    public void Configure(EntityTypeBuilder<CampaignExpense> b)
    {
        b.ToTable("CampaignExpenses", t => t.HasCheckConstraint("CK_CampaignExpenses_Amount", "[Amount] > 0"));
        b.HasKey(x => x.CampaignExpenseID);
        b.Property(x => x.ExpenseCategory).HasMaxLength(100).IsRequired();
        b.Property(x => x.Amount).HasPrecision(18, 2);
        b.Property(x => x.VendorName).HasMaxLength(200);
        b.Property(x => x.Description).HasMaxLength(1000);
        b.HasIndex(x => new { x.CampaignID, x.ExpenseDate, x.Status });
        b.HasOne(x => x.Campaign).WithMany(x => x.Expenses).HasForeignKey(x => x.CampaignID).OnDelete(DeleteBehavior.Restrict);
    }
}
