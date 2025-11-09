using FTM.Domain.Entities.Funds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTM.Infrastructure.Configurations
{
    public class FTPayOSTransactionConfiguration : IEntityTypeConfiguration<FTPayOSTransaction>
    {
        public void Configure(EntityTypeBuilder<FTPayOSTransaction> builder)
        {
            // Table configuration
            builder.ToTable("FTPayOSTransactions");
            builder.HasKey(p => p.Id);

            // Property configurations
            builder.Property(p => p.OrderCode)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.PayOSTransactionId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(p => p.PayOSStatus)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.WebhookData)
                .HasColumnType("text");

            // Relationships
            builder.HasOne(p => p.Donation)
                .WithMany()
                .HasForeignKey(p => p.DonationId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(p => p.OrderCode)
                .IsUnique();
            builder.HasIndex(p => p.PayOSTransactionId);
            builder.HasIndex(p => p.DonationId);
            builder.HasIndex(p => p.PayOSStatus);
            builder.HasIndex(p => p.CreatedOn);
        }
    }
}