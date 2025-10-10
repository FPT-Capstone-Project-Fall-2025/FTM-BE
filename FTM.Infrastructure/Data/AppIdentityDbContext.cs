using FTM.Domain.Entities.Identity;
using FTM.Domain.Entities.Applications;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Data
{
    public class AppIdentityDbContext :IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public AppIdentityDbContext()
        {
        }

        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<Mprovince> Mprovinces { get; set; }
        public DbSet<MWard> MWards { get; set; }
        public DbSet<Biography> Biographies { get; set; }

            public DbSet<WorkExperience> WorkExperiences { get; set; }
    public DbSet<WorkPosition> WorkPositions { get; set; }
    public DbSet<Education> Educations { get; set; }    
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed Roles
            var adminRoleId = Guid.NewGuid();
            var userRoleId = Guid.NewGuid();

            builder.Entity<ApplicationRole>().HasData(
                new ApplicationRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new ApplicationRole
                {
                    Id = userRoleId,
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            );


            // Configure Mprovince
            builder.Entity<Mprovince>()
                .HasKey(p => p.Id);

            builder.Entity<Mprovince>()
                .HasIndex(p => p.Code)
                .IsUnique();

            // Configure MWard
            builder.Entity<MWard>()
                .HasKey(w => w.Id);

            builder.Entity<MWard>()
                .HasIndex(w => w.Code)
                .IsUnique();

            // Configure Biography
            builder.Entity<Biography>()
                .HasKey(b => b.Id);

            builder.Entity<Biography>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Biography>()
                .HasIndex(b => new { b.UserId, b.Type });

                      // Configure WorkExperience
            builder.Entity<WorkExperience>()
                .HasKey(w => w.Id);

            builder.Entity<WorkExperience>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WorkExperience>()
                .HasIndex(w => new { w.UserId, w.CompanyName });

            // Configure WorkPosition
            builder.Entity<WorkPosition>()
                .HasKey(p => p.Id);

            builder.Entity<WorkPosition>()
                .HasOne(p => p.WorkExperience)
                .WithMany(w => w.Positions)
                .HasForeignKey(p => p.WorkExperienceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Education
            builder.Entity<Education>()
                .HasKey(e => e.Id);

            builder.Entity<Education>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Education>()
                .HasIndex(e => new { e.UserId, e.InstitutionName });

            // Configure ApplicationUser relationships - will add these after seeding data
            // builder.Entity<ApplicationUser>()
            //     .HasOne(u => u.MProvince)
            //     .WithMany()
            //     .HasForeignKey(u => u.ProvinceId)
            //     .OnDelete(DeleteBehavior.SetNull);

            // builder.Entity<ApplicationUser>()
            //     .HasOne(u => u.MWard)
            //     .WithMany()
            //     .HasForeignKey(u => u.WardId)
            //     .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
