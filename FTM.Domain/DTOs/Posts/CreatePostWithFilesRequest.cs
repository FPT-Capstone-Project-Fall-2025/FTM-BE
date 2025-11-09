using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace FTM.Domain.DTOs.Posts
{
    public class CreatePostWithFilesRequest
    {
        public Guid FTId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid FTMemberId { get; set; }
        public int Status { get; set; } = 0; // Default: Draft
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
        public List<string> Captions { get; set; } = new List<string>(); // Captions tương ứng với Files
        public List<int> FileTypes { get; set; } = new List<int>(); // FileTypes tương ứng với Files
    }

    public class UpdatePostWithFilesRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
        public List<string> Captions { get; set; } = new List<string>();
        public List<int> FileTypes { get; set; } = new List<int>();
        public List<string> ExistingFileUrls { get; set; } = new List<string>(); // URLs của file cũ muốn giữ lại
    }
}
