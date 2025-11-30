using System;
using System.Collections.Generic;

namespace FTM.Domain.DTOs.Posts
{
    public class CreatePostRequest
    {
        public Guid GPId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid GPMemberId { get; set; }
        public int Status { get; set; } = 0; // Default: Draft
        public List<CreatePostAttachmentRequest> Attachments { get; set; } = new List<CreatePostAttachmentRequest>();
    }

    public class CreatePostAttachmentRequest
    {
        public string FileUrl { get; set; }
        public int FileType { get; set; } // 1: Image, 2: Video, 3: Icon, 4: Document
        public string Caption { get; set; }
    }
}
