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
    public class FTRelationshipConfiguration : IEntityTypeConfiguration<FTRelationship>
    {
        public void Configure(EntityTypeBuilder<FTRelationship> builder)
        {
            builder.ToTable("FTRelationships")
                .HasKey(r => r.Id); 

            builder.HasOne(r => r.FromFTMember)
                .WithMany(m => m.FTRelationshipFrom)
                .HasForeignKey(r => r.FromFTMemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.FromFTMemberPartner)
                .WithMany(m => m.FTRelationshipFromPartner)
                .HasForeignKey(r => r.FromFTMemberPartnerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.ToFTMember)
               .WithMany(m => m.FTRelationshipTo)
               .HasForeignKey(r => r.ToFTMemberId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
