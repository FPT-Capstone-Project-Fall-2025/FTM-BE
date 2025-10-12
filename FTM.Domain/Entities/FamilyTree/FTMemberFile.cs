﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Entities.FamilyTree
{
    public class FTMemberFile : BaseEntity
    {
        public bool? IsActive { get; set; } = true;

        public Guid FTMemberId { get; set; }
        public virtual FTMember FTMember { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public string Description { get; set; }
        public string UploadedBy { get; set; }
        public DateTimeOffset? UploadedOn { get; set; }
       // public int? CategoryCode { get; set; }
        public string Thumbnail { get; set; }
    }
}
