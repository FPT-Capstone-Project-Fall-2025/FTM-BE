using FTM.Domain.Entities.FamilyTree;
using System;
using System.Collections.Generic;

namespace FTM.Domain.Entities.Posts
{
    public class PostComment : BaseEntity
    {
        public Guid PostId { get; set; }
        public Guid GPMemberId { get; set; }
        public string Content { get; set; }
        
        // For nested comments (multi-level threading)
        public Guid? ParentCommentId { get; set; }

        // Navigation properties
        public virtual Post Post { get; set; }
        public virtual FTMember GPMember { get; set; }
        
        // Self-referencing relationship for nested comments
        public virtual PostComment ParentComment { get; set; }
        public virtual ICollection<PostComment> ChildComments { get; set; } = new List<PostComment>();
    }
}
