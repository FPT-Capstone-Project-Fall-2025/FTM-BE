using FTM.Domain.Entities.Applications;
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
    public class FTUserConfiguration : IEntityTypeConfiguration<FTUser>
    {
        public void Configure(EntityTypeBuilder<FTUser> builder)
        {
            builder.ToTable("FTUsers")
                .HasKey(e => e.Id);

            builder.Property(m => m.FTRole)
                .HasConversion<string>()
                .IsRequired();

            builder.HasOne(u => u.FT)
               .WithMany(ft => ft.FTUsers)
               .HasForeignKey(u => u.FTId)
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
