using FTM.Domain.Entities.Applications;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public string? Address { get; set; }
        public Guid? ProvinceId { get; set; }

        [ForeignKey(nameof(ProvinceId))]
        public virtual Mprovince MProvince { get; set; }
        public Guid? WardId { get; set; }
        [ForeignKey(nameof(WardId))]
        public virtual MWard MWard { get; set; }
        public string? Nickname { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Job { get; set; }
        public int? Gender { get; set; }
        public string? Picture { get; set; }
        public bool IsActive { get; set; }

        public bool IsGoogleLogin { get; set; } = false;

        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }

        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
