using FTM.Domain.Entities.FamilyTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Entities.Applications
{
    public partial class Mprovince : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Slug { get; set; }
        public string NameWithType { get; set; }

        public virtual ICollection<FTMember> BurialFTMembers { get; set; } = new List<FTMember>();
        public virtual ICollection<FTMember> FTMembers { get; set; } = new List<FTMember>();
    }
}
