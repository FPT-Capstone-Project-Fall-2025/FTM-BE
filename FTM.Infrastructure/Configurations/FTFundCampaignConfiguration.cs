using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTM.Infrastructure.Configurations
{
    public class FTFundCampaignConfiguration : IEntityTypeConfiguration<FTFundCampaign>
    {
        public void Configure(EntityTypeBuilder<FTFundCampaign> builder)
        {
            builder.ToTable("FTFundCampaigns")
                .HasKey(c => c.Id);

            builder.Property(c => c.FTId)
                .IsRequired();

            builder.Property(c => c.CampaignName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.CampaignDescription)
                .HasMaxLength(1000);

            builder.Property(c => c.CampaignManagerId)
                .IsRequired();

            builder.Property(c => c.StartDate)
                .IsRequired();

            builder.Property(c => c.EndDate)
                .IsRequired();

            builder.Property(c => c.FundGoal)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.CurrentBalance)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(c => c.Status)
                .IsRequired()
                .HasDefaultValue(CampaignStatus.Upcoming)
                .HasConversion<int>();

            builder.Property(c => c.IsPublic)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.ImageUrl)
                .HasMaxLength(500);

            builder.Property(c => c.Notes)
                .HasMaxLength(2000);

            builder.Property(c => c.CreatedBy)
                .IsRequired();

            builder.Property(c => c.CreatedOn)
                .IsRequired();

            builder.Property(c => c.LastModifiedOn);
            
            builder.Property(c => c.LastModifiedBy)
                .HasMaxLength(200);

            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Relationships
            builder.HasOne(c => c.FamilyTree)
                .WithMany()
                .HasForeignKey(c => c.FTId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.CampaignManager)
                .WithMany()
                .HasForeignKey(c => c.CampaignManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Donations)
                .WithOne(d => d.Campaign)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Expenses)
                .WithOne(e => e.Campaign)
                .HasForeignKey(e => e.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(c => c.FTId);
            builder.HasIndex(c => c.CampaignManagerId);
            builder.HasIndex(c => c.Status);
            builder.HasIndex(c => c.StartDate);
            builder.HasIndex(c => c.EndDate);
            builder.HasIndex(c => c.CreatedOn);
            builder.HasIndex(c => c.IsDeleted);
        }
    }
}
