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
    public class MReligionConfiguration : IEntityTypeConfiguration<MReligion>
    {
        public void Configure(EntityTypeBuilder<MReligion> builder)
        {
            builder.ToTable("MReligions")
                .HasKey(r => r.Id);
            builder.Property(r => r.Code).IsRequired();
            builder.Property(r => r.Name).IsRequired();
        }
    }
}
