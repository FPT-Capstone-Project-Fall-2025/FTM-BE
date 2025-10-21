using FTM.Domain.Entities.FamilyTree;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTM.Infrastructure.Configurations
{
    public class AcademicHonorConfiguration : IEntityTypeConfiguration<AcademicHonor>
    {
        public void Configure(EntityTypeBuilder<AcademicHonor> builder)
        {
            builder.ToTable("AcademicHonors");
            
            builder.HasKey(a => a.Id);

            builder.Property(a => a.AchievementTitle)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(a => a.InstitutionName)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(a => a.DegreeOrCertificate)
                .HasMaxLength(200);

            builder.Property(a => a.YearOfAchievement)
                .IsRequired();

            builder.Property(a => a.Description)
                .HasMaxLength(2000);

            builder.Property(a => a.PhotoUrl)
                .HasMaxLength(500);

            builder.Property(a => a.IsDisplayed)
                .IsRequired()
                .HasDefaultValue(true);

            // Relationships
            builder.HasOne(a => a.GPMember)
                .WithMany()
                .HasForeignKey(a => a.GPMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.FamilyTree)
                .WithMany()
                .HasForeignKey(a => a.FamilyTreeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(a => a.FamilyTreeId);
            builder.HasIndex(a => a.GPMemberId);
            builder.HasIndex(a => a.YearOfAchievement);
            builder.HasIndex(a => a.IsDisplayed);
        }
    }
}
