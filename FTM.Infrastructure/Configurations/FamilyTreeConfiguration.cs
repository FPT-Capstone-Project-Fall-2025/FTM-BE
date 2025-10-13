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
    public class FamilyTreeConfiguration : IEntityTypeConfiguration<FamilyTree>
    {
        public void Configure(EntityTypeBuilder<FamilyTree> builder)
        {
            builder.ToTable("FamilyTrees")
                .HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Owner).IsRequired();
            builder.Property(x => x.OwnerId).IsRequired();
        }
    }
}
