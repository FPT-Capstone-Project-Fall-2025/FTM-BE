using FTM.Domain.Entities.Funds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTM.Infrastructure.Configurations
{
    public class FTFundDonationConfiguration : IEntityTypeConfiguration<FTFundDonation>
    {
        public void Configure(EntityTypeBuilder<FTFundDonation> builder)
        {
            // Table configuration
            builder.ToTable("FTFundDonations");
            builder.HasKey(d => d.Id);

            // Property configurations
            builder.Property(d => d.DonationMoney)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(d => d.DonorName)
                .HasMaxLength(500);

            builder.Property(d => d.PaymentNotes)
                .HasMaxLength(500);

            builder.Property(d => d.PaymentTransactionId)
                .HasMaxLength(100);

            builder.Property(d => d.ConfirmationNotes)
                .HasMaxLength(1000);

            // Relationships
            builder.HasOne(d => d.Fund)
                .WithMany(f => f.Donations)
                .HasForeignKey(d => d.FTFundId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Member)
                .WithMany()
                .HasForeignKey(d => d.FTMemberId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(d => d.Confirmer)
                .WithMany()
                .HasForeignKey(d => d.ConfirmedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(d => d.FTFundId);
            builder.HasIndex(d => d.FTMemberId);
            builder.HasIndex(d => d.ConfirmedBy);
            builder.HasIndex(d => d.CreatedOn);
            builder.HasIndex(d => d.Status);
            builder.HasIndex(d => d.PaymentMethod);
        }
    }
}