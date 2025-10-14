using FTM.Domain.Enums;
using System;

namespace FTM.Domain.Entities.Posts
{
    public class PostAttachment : BaseEntity
    {
        public Guid PostId { get; set; }
        public string FileUrl { get; set; }
        public PostFileType FileType { get; set; }
        public string Caption { get; set; }

        // Navigation properties
        public virtual Post Post { get; set; }
    }
}
