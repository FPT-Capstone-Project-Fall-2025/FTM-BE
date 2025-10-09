using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Entities.Identity
{
    public class ApplicationUserRole : IdentityUserRole<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual ApplicationRole Role { get; set; }
    }
}
