using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Entities.FamilyTree
{
    public class FamilyTree : BaseEntity
    {
        public string Name { get; set; }
        public Guid OwnerId { get; set; }
        public string Owner { get; set; }

        public string Description { get; set; }

        public string? FilePath {  get; set; }
        public int FileType { get; set; } = 1;

        public bool? IsActive { get; set; } = true;
        public int? GPModeCode { get; set; }

        public virtual ICollection<FTInvitation> FTInvitations { get; set; } = new List<FTInvitation>();
        public virtual ICollection<FTUser> FTUsers { get; set; } = new List<FTUser>();
        public virtual ICollection<FTMember> FTMembers { get; set; } = new List<FTMember>();
        public virtual ICollection<FTAuthorization> FTAuthorizations { get; set; } = new List<FTAuthorization>();
    }
}
