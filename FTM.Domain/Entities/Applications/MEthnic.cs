using FTM.Domain.Entities.FamilyTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Entities.Applications
{
    public class MEthnic : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public bool IsActive { get; set; } = true;
        public virtual ICollection<FTMember> FTMembers { get; set; } = new List<FTMember>();
    }
}
