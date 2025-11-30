using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class UpsertFTRelationshipRequest
    {
        public Guid? Id { get; set; }
        public Guid? FromGPMemberId { get; set; }
        public Guid? FromGPMemberPartnerId { get; set; }
        public Guid? ToGPMemberId { get; set; }
        public int? CategoryCode { get; set; }
    }
}
