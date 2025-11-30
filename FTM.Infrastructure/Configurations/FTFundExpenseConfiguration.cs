using FTM.Domain.Entities.Funds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTM.Infrastructure.Configurations
{
    public class FTFundExpenseConfiguration : IEntityTypeConfiguration<FTFundExpense>
    {
        public void Configure(EntityTypeBuilder<FTFundExpense> builder)
        {
            // Table configuration
            builder.ToTable("FTFundExpenses");
            builder.HasKey(e => e.Id);

            // Property configurations
            builder.Property(e => e.ExpenseAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(e => e.ExpenseDescription)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(e => e.ExpenseEvent)
                .HasMaxLength(500);

            builder.Property(e => e.Recipient)
                .HasMaxLength(500);

            builder.Property(e => e.ApprovalFeedback)
                .HasMaxLength(1000);

            // Relationships
            builder.HasOne(e => e.Fund)
                .WithMany(f => f.Expenses)
                .HasForeignKey(e => e.FTFundId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Approver)
                .WithMany()
                .HasForeignKey(e => e.ApprovedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(e => e.FTFundId);
            builder.HasIndex(e => e.ApprovedBy);
            builder.HasIndex(e => e.CreatedOn);
            builder.HasIndex(e => e.Status);
            builder.HasIndex(e => e.PlannedDate);
        }
    }
}