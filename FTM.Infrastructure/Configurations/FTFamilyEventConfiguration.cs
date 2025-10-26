using FTM.Domain.Entities.Events;
using FTM.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTM.Infrastructure.Configurations
{
    public class FTFamilyEventConfiguration : IEntityTypeConfiguration<FTFamilyEvent>
    {
        public void Configure(EntityTypeBuilder<FTFamilyEvent> builder)
        {
            builder.ToTable("FTFamilyEvents")
                  .HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.EventType)
                .IsRequired()
                .HasMaxLength(100)
                .HasConversion<string>();

            builder.Property(e => e.StartTime)
                .IsRequired();

            builder.Property(e => e.Location)
                .HasMaxLength(1000);

            builder.Property(e => e.RecurrenceType)
                .IsRequired()
                .HasDefaultValue(RecurrenceType.None)
                .HasConversion<int>();

            builder.Property(e => e.Description)
                .HasMaxLength(2000);

            builder.Property(e => e.ImageUrl)
                .HasMaxLength(500);

            builder.Property(e => e.Address)
                .HasMaxLength(1000);

            builder.Property(e => e.LocationName)
                .HasMaxLength(500);

            builder.Property(e => e.IsAllDay)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.IsLunar)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.IsPublic)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasOne(e => e.FT)
                .WithMany()
                .HasForeignKey(e => e.FTId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.TargetMember)
                .WithMany()
                .HasForeignKey(e => e.TargetMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.EventMembers)
                .WithOne(em => em.FTFamilyEvent)
                .HasForeignKey(em => em.FTFamilyEventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.EventFTs)
                .WithOne(eg => eg.FTFamilyEvent)
                .HasForeignKey(eg => eg.FTFamilyEventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
