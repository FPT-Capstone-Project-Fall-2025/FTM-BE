using FTM.Domain.Entities.Funds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTM.Infrastructure.Configurations
{
    public class FTCampaignDonationConfiguration : IEntityTypeConfiguration<FTCampaignDonation>
    {
        public void Configure(EntityTypeBuilder<FTCampaignDonation> builder)
        {
            builder.ToTable("FTCampaignDonations");

            builder.HasKey(d => d.Id);

            // Configure relationship with Campaign
            builder.HasOne(d => d.Campaign)
                .WithMany(c => c.Donations)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Member (donor) - EXPLICIT foreign key
            builder.HasOne(d => d.Member)
                .WithMany()
                .HasForeignKey(d => d.FTMemberId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Configure relationship with Confirmer - EXPLICIT foreign key
            builder.HasOne(d => d.Confirmer)
                .WithMany()
                .HasForeignKey(d => d.ConfirmedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Configure decimal precision
            builder.Property(d => d.DonationAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Configure other properties
            builder.Property(d => d.DonorName)
                .HasMaxLength(200);

            builder.Property(d => d.PaymentMethod)
                .IsRequired();

            builder.Property(d => d.Status)
                .IsRequired();

            builder.Property(d => d.IsAnonymous)
                .HasDefaultValue(false);

            builder.Property(d => d.DonorNotes)
                .HasMaxLength(1000);

            builder.Property(d => d.PaymentTransactionId)
                .HasMaxLength(200);

            builder.Property(d => d.ConfirmationNotes)
                .HasMaxLength(1000);
        }
    }
}
