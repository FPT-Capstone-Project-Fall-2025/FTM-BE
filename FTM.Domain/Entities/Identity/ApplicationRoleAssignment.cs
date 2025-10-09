using FTM.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Entities.Identity
{
    public class ApplicationRoleAssignment : BaseEntity
    {
        public Guid GPId { get; set; }
        public string GPName { get; set; } = string.Empty;
        
        // Use composite foreign key to match IdentityUserRole primary key
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        
        public virtual ApplicationUserRole UserRole { get; set; } = null!;
    }
}