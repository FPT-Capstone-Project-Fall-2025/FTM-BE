using FTM.Domain.DTOs.Authen;
using FTM.Domain.Entities.Applications;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Entities.Identity;
using FTM.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Data
{
    public class FTMDbContext : DbContext
    {
        public FTMDbContext()
        {
        }

        public FTMDbContext(DbContextOptions<FTMDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Mprovince> Mprovinces { get; set; }
        public virtual DbSet<MWard> MWards { get; set; }
        public virtual DbSet<MEthnic> MEthnics { get; set; }
        public virtual DbSet<MReligion> MReligions { get; set; }
        public virtual DbSet<FTMember> FTMembers { get; set; }
        public virtual DbSet<FTMemberFile> FTMemberFiles { get; set; }
        public virtual DbSet<FamilyTree> FamilyTrees { get; set; }
        public virtual DbSet<FTRelationship> FTRelationships { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new MEthicConfiguration());
            builder.ApplyConfiguration(new MReligionConfiguration());
            builder.ApplyConfiguration(new MProvinceConfiguration());
            builder.ApplyConfiguration(new MWardConfiguration());
            builder.ApplyConfiguration(new FTMemberConfiguration());
            builder.ApplyConfiguration(new FTMemberFileConfiguration());
            builder.ApplyConfiguration(new FamilyTreeConfiguration());
            builder.ApplyConfiguration(new FTRelationshipConfiguration());
            base.OnModelCreating(builder);
        }


    }
}
