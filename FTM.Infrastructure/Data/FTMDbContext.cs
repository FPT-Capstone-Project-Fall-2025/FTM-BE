using FTM.Domain.Models.Authen;
using FTM.Domain.Entities.Applications;
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

        public virtual DbSet<SendOTPTracking> SendOTPTrackings { get; set; }
        public virtual DbSet<Mprovince> Mprovinces { get; set; }
        public virtual DbSet<MWard> MWards { get; set; }
    }
}
