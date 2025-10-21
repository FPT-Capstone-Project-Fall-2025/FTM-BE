using FTM.Domain.Entities.FamilyTree;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTM.Infrastructure.Configurations
{
    public class CareerHonorConfiguration : IEntityTypeConfiguration<CareerHonor>
    {
        public void Configure(EntityTypeBuilder<CareerHonor> builder)
        {
            builder.ToTable("CareerHonors");
            
            builder.HasKey(c => c.Id);

            builder.Property(c => c.AchievementTitle)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(c => c.OrganizationName)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(c => c.Position)
                .HasMaxLength(200);

            builder.Property(c => c.YearOfAchievement)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(2000);

            builder.Property(c => c.PhotoUrl)
                .HasMaxLength(500);

            builder.Property(c => c.IsDisplayed)
                .IsRequired()
                .HasDefaultValue(true);

            // Relationships
            builder.HasOne(c => c.GPMember)
                .WithMany()
                .HasForeignKey(c => c.GPMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.FamilyTree)
                .WithMany()
                .HasForeignKey(c => c.FamilyTreeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(c => c.FamilyTreeId);
            builder.HasIndex(c => c.GPMemberId);
            builder.HasIndex(c => c.YearOfAchievement);
            builder.HasIndex(c => c.IsDisplayed);
        }
    }
}
