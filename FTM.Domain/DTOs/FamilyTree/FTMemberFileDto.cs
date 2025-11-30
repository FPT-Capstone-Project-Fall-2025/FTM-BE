using FTM.Domain.Entities.FamilyTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class FTMemberFileDto
    {
        public Guid FTMemberId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public bool? IsActive { get; set; } = true;
        public string CreatedBy { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTimeOffset? LastModifiedOn { get; set; }
    }
}
