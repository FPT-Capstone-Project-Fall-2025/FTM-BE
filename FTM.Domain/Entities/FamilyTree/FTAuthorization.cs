using FTM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FTM.Domain.Entities.FamilyTree
{
    public class FTAuthorization : BaseEntity
    {
        public Guid FTId { get; set; }
        public Guid FTMemberId { get; set; }
        public MethodType MethodCode { get; set; }
        public FeatureType FeatureCode { get; set; }
        public virtual FamilyTree FamilyTree { get; set; }
        public virtual FTMember AuthorizedMember { get; set; }
    }
}
 