using System;

namespace FTM.Domain.DTOs.Posts
{
    public class UpdateCommentRequest
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
    }
}
