using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Entities.Identity;
using FTM.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Configurations
{
    public class FTNotificationConfiguration : IEntityTypeConfiguration<FTNotification>
    {
        public void Configure(EntityTypeBuilder<FTNotification> builder)
        {
            builder.ToTable("FTNotifications")
                   .HasKey(e => e.Id);

            builder.Property(e => e.UserId)
                   .IsRequired();

            builder.Property(e => e.Title)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(e => e.Message)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(e => e.Type)
                   .HasConversion<string>()  
                   .IsRequired();

            builder.Property(e => e.IsRead)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(e => e.IsActionable)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(e => e.Link)
                   .HasMaxLength(500);

        }
    }
}
