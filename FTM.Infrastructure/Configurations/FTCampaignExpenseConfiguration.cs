using FTM.Domain.Entities.Funds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTM.Infrastructure.Configurations
{
    public class FTCampaignExpenseConfiguration : IEntityTypeConfiguration<FTCampaignExpense>
    {
        public void Configure(EntityTypeBuilder<FTCampaignExpense> builder)
        {
            builder.ToTable("FTCampaignExpenses");

            builder.HasKey(e => e.Id);

            // Configure relationship with Campaign
            builder.HasOne(e => e.Campaign)
                .WithMany(c => c.Expenses)
                .HasForeignKey(e => e.CampaignId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Authorizer (who created the expense) - EXPLICIT foreign key
            builder.HasOne(e => e.Authorizer)
                .WithMany()
                .HasForeignKey(e => e.AuthorizedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Configure relationship with Approver (who approved the expense) - EXPLICIT foreign key
            builder.HasOne(e => e.Approver)
                .WithMany()
                .HasForeignKey(e => e.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Configure decimal precision
            builder.Property(e => e.ExpenseAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Configure required properties
            builder.Property(e => e.ExpenseTitle)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.ExpenseDescription)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(e => e.Category)
                .IsRequired();

            builder.Property(e => e.ExpenseDate)
                .IsRequired();

            builder.Property(e => e.PaymentMethod)
                .IsRequired();

            builder.Property(e => e.ApprovalStatus)
                .IsRequired();

            // Configure optional properties
            builder.Property(e => e.Recipient)
                .HasMaxLength(200);

            builder.Property(e => e.ReceiptImages)
                .HasMaxLength(2000);

            builder.Property(e => e.Notes)
                .HasMaxLength(1000);

            builder.Property(e => e.ApprovalNotes)
                .HasMaxLength(1000);
        }
    }
}
