using FTM.Domain.Entities.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTM.Infrastructure.Configurations
{
    public class FTFamilyEventMemberConfiguration : IEntityTypeConfiguration<FTFamilyEventMember>
    {
        public void Configure(EntityTypeBuilder<FTFamilyEventMember> builder)
        {
            builder.ToTable("FTFamilyEventMembers")
                  .HasKey(em => em.Id);

            builder.Property(em => em.FTFamilyEventId)
                .IsRequired();

            builder.Property(em => em.FTMemberId)
                .IsRequired();

            builder.HasOne(em => em.FTFamilyEvent)
                .WithMany(e => e.EventMembers)
                .HasForeignKey(em => em.FTFamilyEventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(em => em.FTMember)
                .WithMany()
                .HasForeignKey(em => em.FTMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint to prevent duplicate member assignments
            builder.HasIndex(em => new { em.FTFamilyEventId, em.FTMemberId })
                .IsUnique();
        }
    }
}
