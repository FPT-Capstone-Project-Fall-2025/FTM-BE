using System;

namespace FTM.Domain.Entities.Events
{
    public class FTFamilyEventMember : BaseEntity
    {
        public Guid FTFamilyEventId { get; set; }
        public Guid FTMemberId { get; set; }
        public Guid? UserId { get; set; }

        // Navigation properties
        public virtual FTFamilyEvent FTFamilyEvent { get; set; }
        public virtual FamilyTree.FTMember FTMember { get; set; }
    }
}
