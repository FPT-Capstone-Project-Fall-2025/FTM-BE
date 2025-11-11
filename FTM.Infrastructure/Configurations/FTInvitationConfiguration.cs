using FTM.Domain.Entities.FamilyTree;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Configurations
{
    public class FTInvitationConfiguration : IEntityTypeConfiguration<FTInvitation>
    {
        public void Configure(EntityTypeBuilder<FTInvitation> builder)
        {
            builder.ToTable("FTInvitations")
            .HasKey(e => e.Id);

            builder.Property(e => e.InviterUserId)
                .IsRequired();

            builder.Property(e => e.FTMemberName)
                .IsRequired(false);

            builder.Property(e => e.InvitedUserId)
                .IsRequired();

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Status)
                .HasConversion<string>() 
                .IsRequired();

            builder.Property(e => e.ExpirationDate)
                .IsRequired();

            builder.HasOne(e => e.FT)
                .WithMany(ft => ft.FTInvitations)
                .HasForeignKey(e => e.FTId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.FTMember)
                .WithMany(m => m.FTInvitations)
                .HasForeignKey(e => e.FTMemberId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
