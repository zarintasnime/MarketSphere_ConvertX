using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MarketSphere.Domain.Entities.OrderFulfilment;

namespace MarketSphere.Infrastructure.Persistence.Configurations.OrderFulfilment;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders", t => t.HasCheckConstraint("CK_Orders_Amounts", "[GrossAmount] >= 0 AND [DiscountAmount] >= 0 AND [TaxAmount] >= 0 AND [NetAmount] >= 0"));
        builder.HasKey(x => x.OrderID);
        builder.Property(x => x.OrderNo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Channel).HasConversion<int>();
        builder.Property(x => x.OrderSource).HasConversion<int>();
        builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.CreditCheckStatus).HasConversion<int>();
        builder.Property(x => x.DeliveryAddressSnapshot).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.GrossAmount).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.NetAmount).HasPrecision(18, 2);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => x.OrderNo).IsUnique();
        builder.HasIndex(x => x.QuotationID).IsUnique().HasFilter("[QuotationID] IS NOT NULL");
        builder.HasIndex(x => x.ModernTradePurchaseOrderID).IsUnique().HasFilter("[ModernTradePurchaseOrderID] IS NOT NULL");
        builder.HasIndex(x => new { x.ClientID, x.Status, x.OrderDate });
        builder.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Campaign).WithMany().HasForeignKey(x => x.CampaignID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Quotation).WithMany().HasForeignKey(x => x.QuotationID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ModernTradePurchaseOrder).WithOne(x => x.ConvertedOrder).HasForeignKey<Order>(x => x.ModernTradePurchaseOrderID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PriceList).WithMany().HasForeignKey(x => x.PriceListID).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ApprovalRequest).WithMany().HasForeignKey(x => x.ApprovalRequestID).OnDelete(DeleteBehavior.Restrict);
    }
}
