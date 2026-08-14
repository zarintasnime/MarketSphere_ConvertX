using MarketSphere.Domain.Entities.OrderFulfilment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrderFulfilment;

public sealed class AppliedOfferConfiguration : IEntityTypeConfiguration<AppliedOffer>
{
    public void Configure(EntityTypeBuilder<AppliedOffer> builder)
    {
        builder.ToTable("AppliedOffers", t => t.HasCheckConstraint("CK_AppliedOffers_Parent", "(CASE WHEN [QuotationID] IS NULL THEN 0 ELSE 1 END + CASE WHEN [QuotationItemID] IS NULL THEN 0 ELSE 1 END + CASE WHEN [OrderID] IS NULL THEN 0 ELSE 1 END + CASE WHEN [OrderItemID] IS NULL THEN 0 ELSE 1 END) = 1"));
        builder.HasKey(x => x.AppliedOfferID);
        builder.Property(x => x.BenefitType).HasConversion<int>();
        builder.Property(x => x.BenefitAmount).HasPrecision(18, 2);
        builder.Property(x => x.FreeQuantity).HasPrecision(18, 3);
        builder.Property(x => x.RuleSnapshotJson).HasColumnType("nvarchar(max)").IsRequired();
        builder.HasIndex(x => new { x.CampaignOfferID, x.OrderID, x.OrderItemID });
        builder.HasOne(x => x.Quotation).WithMany().HasForeignKey(x => x.QuotationID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.QuotationItem).WithMany().HasForeignKey(x => x.QuotationItemID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Order).WithMany(x => x.AppliedOffers).HasForeignKey(x => x.OrderID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.OrderItem).WithMany(x => x.AppliedOffers).HasForeignKey(x => x.OrderItemID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.CampaignOffer).WithMany().HasForeignKey(x => x.CampaignOfferID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.FreeSKU).WithMany().HasForeignKey(x => x.FreeSKUID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AppliedByUser).WithMany().HasForeignKey(x => x.AppliedByUserID).OnDelete(DeleteBehavior.Restrict);
    }
}
