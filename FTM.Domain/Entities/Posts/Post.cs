using FTM.Domain.Entities.FamilyTree;
using System;
using System.Collections.Generic;

namespace FTM.Domain.Entities.Posts
{
    public class Post : BaseEntity
    {
        public Guid GPId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid GPMemberId { get; set; }
        public int Status { get; set; } // 0: Draft, 1: Published, 2: Archived
        public DateTime? ApprovedAt { get; set; }
        public Guid? ApprovedBy { get; set; }

        // Navigation properties
        public virtual FTMember GPMember { get; set; }
        public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();
        public virtual ICollection<PostReaction> PostReactions { get; set; } = new List<PostReaction>();
        public virtual ICollection<PostAttachment> PostAttachments { get; set; } = new List<PostAttachment>();
    }
}
