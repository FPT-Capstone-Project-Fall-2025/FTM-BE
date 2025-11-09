using System;
using FTM.Domain.Enums;

namespace FTM.Domain.DTOs.Posts
{
    public class PostReactionDto
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid FTMemberId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorPicture { get; set; }
        public ReactionType ReactionType { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public bool HasReacted { get; set; }
    }
}
