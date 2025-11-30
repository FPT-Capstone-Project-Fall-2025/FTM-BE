using System;

namespace FTM.Domain.Entities.Events
{
    public class FTFamilyEventFT : BaseEntity
    {
        public Guid FTFamilyEventId { get; set; }
        public Guid FTId { get; set; }

        // Navigation properties
        public virtual FTFamilyEvent FTFamilyEvent { get; set; }
        public virtual FamilyTree.FamilyTree FT { get; set; }
    }
}
