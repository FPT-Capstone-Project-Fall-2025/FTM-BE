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

        public string Owner { get; set; }

        public string Description { get; set; }

        public string Picture { get; set; }

        public bool? IsActive { get; set; } = true;
        public int? GPModeCode { get; set; }

        public virtual ICollection<FTMember> FTMembers { get; set; } = new List<FTMember>();
    }
}
