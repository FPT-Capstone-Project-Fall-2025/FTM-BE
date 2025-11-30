using System;
using System.Collections.Generic;

namespace FTM.Domain.DTOs.Posts
{
    public class UpdatePostRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
        public List<CreatePostAttachmentRequest> Attachments { get; set; } = new List<CreatePostAttachmentRequest>();
    }
}
