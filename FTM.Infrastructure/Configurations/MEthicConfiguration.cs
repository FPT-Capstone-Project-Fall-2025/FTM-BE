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
    public class MEthicConfiguration : IEntityTypeConfiguration<MEthnic>
    {
        public void Configure(EntityTypeBuilder<MEthnic> builder)
        {
            builder.ToTable("MEthnics")
                .HasKey(e => e.Id);

            builder.Property(e => e.Code).IsRequired();
            builder.Property(e => e.Name).IsRequired();
        }

    }
}
