using MarketSphere.Domain.Entities.MarketingField;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketSphere.Infrastructure.Persistence.Configurations.MarketingField;

public sealed class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> b)
    {
        b.ToTable("Feedbacks", t =>
        {
            t.HasCheckConstraint("CK_Feedbacks_Party", "[ClientID] IS NOT NULL OR [LeadID] IS NOT NULL");
            t.HasCheckConstraint("CK_Feedbacks_Rating", "[Rating] IS NULL OR ([Rating] >= 1 AND [Rating] <= 5)");
        });
        b.HasKey(x => x.FeedbackID);
        b.Property(x => x.Comments).HasMaxLength(4000);
        b.HasIndex(x => new { x.CampaignID, x.SubmittedAt });
        b.HasIndex(x => new { x.IsFollowUpRequired, x.SubmittedAt });
        b.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientID).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Lead).WithMany().HasForeignKey(x => x.LeadID).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Campaign).WithMany().HasForeignKey(x => x.CampaignID).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Visit).WithMany(x => x.Feedbacks).HasForeignKey(x => x.VisitID).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.SubmittedByEmployee).WithMany().HasForeignKey(x => x.SubmittedByEmployeeID).OnDelete(DeleteBehavior.Restrict);
    }
}
