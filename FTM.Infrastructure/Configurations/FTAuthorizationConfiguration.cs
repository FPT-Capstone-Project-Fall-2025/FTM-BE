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
    public class FTAuthorizationConfiguration : IEntityTypeConfiguration<FTAuthorization>
    {
        public void Configure(EntityTypeBuilder<FTAuthorization> builder)
        {
            builder.ToTable("FTAuthorizations")
                   .HasKey(a => a.Id);

            builder.Property(a => a.MethodCode)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(a => a.FeatureCode)
                   .HasConversion<string>()
                   .IsRequired(); ;

            builder.HasOne(a => a.AuthorizedMember)
                .WithMany(m => m.FTAuthorizations)
                .HasForeignKey(a => a.FTMemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.FamilyTree)
                .WithMany(ft => ft.FTAuthorizations)
                .HasForeignKey(a => a.FTId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
