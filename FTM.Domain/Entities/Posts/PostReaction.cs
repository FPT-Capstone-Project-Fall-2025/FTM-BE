using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Enums;
using System;

namespace FTM.Domain.Entities.Posts
{
    public class PostReaction : BaseEntity
    {
        public Guid PostId { get; set; }
        public Guid FTMemberId { get; set; }
        public ReactionType ReactionType { get; set; }

        // Navigation properties
        public virtual Post Post { get; set; }
        public virtual FTMember FTMember { get; set; }
    }
}
