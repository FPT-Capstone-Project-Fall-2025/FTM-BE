using System;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class FTRelationshipDto
    {
        public Guid Id { get; set; }
        public bool? IsActive { get; set; }
        public Guid FromFTMemberId { get; set; }
        public Guid FromFTMemberPartnerId { get; set; }
        public Guid ToFTMemberId { get; set; }
        public int CategoryCode { get; set; }

        // Navigation properties for display
        public string FromMemberName { get; set; }
        public string FromPartnerName { get; set; }
        public string ToMemberName { get; set; }
    }
}