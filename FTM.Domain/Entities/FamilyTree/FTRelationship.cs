using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Entities.FamilyTree
{
    public class FTRelationship : BaseEntity
    {
        public bool? IsActive { get; set; } = true;
        public Guid FromFTMemberId { get; set; }
        public Guid? FromFTMemberPartnerId { get; set; }
        public Guid ToFTMemberId { get; set; }
        public int CategoryCode { get; set; }

        public virtual FTMember FromFTMember { get; set; }
        public virtual FTMember FromFTMemberPartner { get; set; }
        public virtual FTMember ToFTMember { get; set; }
    }
}
