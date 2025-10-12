using FTM.Domain.Entities.Applications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Configurations
{
    public class MProvinceConfiguration : IEntityTypeConfiguration<Mprovince>
    {
        public void Configure(EntityTypeBuilder<Mprovince> builder)
        {
            builder.ToTable("MProvinces")
                .HasKey(x => x.Id);

            builder.Property(e => e.Code).IsRequired();
            builder.Property(e => e.Name).IsRequired();
        }
    }
}
