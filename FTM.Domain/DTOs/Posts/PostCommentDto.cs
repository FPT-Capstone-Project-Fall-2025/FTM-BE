using System;
using System.Collections.Generic;

namespace FTM.Domain.DTOs.Posts
{
    public class PostCommentDto
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid FTMemberId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorPicture { get; set; }
        public string Content { get; set; }
        public Guid? ParentCommentId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? LastModifiedOn { get; set; }
        
        // For nested comments
        public List<PostCommentDto> ChildComments { get; set; } = new List<PostCommentDto>();
    }
}
