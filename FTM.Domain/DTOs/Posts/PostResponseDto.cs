using System;
using System.Collections.Generic;

namespace FTM.Domain.DTOs.Posts
{
    public class PostResponseDto
    {
        public Guid Id { get; set; }
        public Guid FTId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid FTMemberId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorPicture { get; set; }
        public int Status { get; set; }
        public DateTimeOffset? ApprovedAt { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? LastModifiedOn { get; set; }
        
        public int TotalComments { get; set; }
        public int TotalReactions { get; set; }
        public Dictionary<string, int> ReactionsSummary { get; set; } = new Dictionary<string, int>();
        public PostReactionDto? CurrentUserReaction { get; set; }
        
        public List<PostAttachmentDto> Attachments { get; set; } = new List<PostAttachmentDto>();
        public List<PostCommentDto> Comments { get; set; } = new List<PostCommentDto>();
    }

    public class PostAttachmentDto
    {
        public Guid Id { get; set; }
        public string FileUrl { get; set; }
        public int FileType { get; set; }
        public string Caption { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
