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
    public class FTMemberConfiguration : IEntityTypeConfiguration<FTMember>
    {
        public void Configure(EntityTypeBuilder<FTMember> builder)
        {
            builder.ToTable("FTMembers")
                  .HasKey(m => m.Id);

            builder.Property(m => m.FTRole)
                .HasConversion<string>()
                .IsRequired();

            builder.HasOne(m => m.Ethnic)
                .WithMany(e => e.FTMembers)
                .HasForeignKey(m => m.EthnicId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Religion)
                .WithMany(r => r.FTMembers)
                .HasForeignKey(m => m.ReligionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Province)
               .WithMany(p => p.FTMembers)
               .HasForeignKey(m => m.ProvinceId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.BurialProvince)
                .WithMany(p => p.BurialFTMembers)
                .HasForeignKey(m => m.BurialProvinceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Ward)
               .WithMany(w => w.FTMembers)
               .HasForeignKey(m => m.WardId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.BurialWard)
                .WithMany(w => w.BurialFTMembers)
                .HasForeignKey(m => m.BurialWardId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.FT)
                .WithMany(ft => ft.FTMembers)
                .HasForeignKey(m => m.FTId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
