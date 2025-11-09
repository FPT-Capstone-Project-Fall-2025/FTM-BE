using System;

namespace FTM.Domain.DTOs.Posts
{
    public class CreateCommentRequest
    {
        public Guid PostId { get; set; }
        public Guid FTMemberId { get; set; }
        public string Content { get; set; }
        public Guid? ParentCommentId { get; set; } // null = root comment, else reply to comment
    }
}
