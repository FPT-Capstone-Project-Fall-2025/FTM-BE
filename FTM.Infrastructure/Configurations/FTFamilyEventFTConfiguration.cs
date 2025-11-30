using FTM.Domain.Entities.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTM.Infrastructure.Configurations
{
    public class FTFamilyEventFTConfiguration : IEntityTypeConfiguration<FTFamilyEventFT>
    {
        public void Configure(EntityTypeBuilder<FTFamilyEventFT> builder)
        {
            builder.ToTable("FTFamilyEventFTs")
                  .HasKey(eg => eg.Id);

            builder.Property(eg => eg.FTFamilyEventId)
                .IsRequired();

            builder.Property(eg => eg.FTId)
                .IsRequired();

            builder.HasOne(eg => eg.FTFamilyEvent)
                .WithMany(e => e.EventFTs)
                .HasForeignKey(eg => eg.FTFamilyEventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(eg => eg.FT)
                .WithMany()
                .HasForeignKey(eg => eg.FTId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint to prevent duplicate FT assignments
            builder.HasIndex(eg => new { eg.FTFamilyEventId, eg.FTId })
                .IsUnique();
        }
    }
}
