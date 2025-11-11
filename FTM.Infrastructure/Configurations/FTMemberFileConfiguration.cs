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
    public class FTMemberFileConfiguration : IEntityTypeConfiguration<FTMemberFile>
    {
        public void Configure(EntityTypeBuilder<FTMemberFile> builder)
        {
            builder.ToTable("FTMemberFiles")
                .HasKey(x => x.Id);

            builder.HasOne(f => f.FTMember)
                .WithMany(m => m.FTMemberFiles)
                .HasForeignKey(f => f.FTMemberId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
