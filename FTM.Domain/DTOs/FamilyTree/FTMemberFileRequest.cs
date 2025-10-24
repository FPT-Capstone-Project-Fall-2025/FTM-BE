using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class FTMemberFileRequest
    {
        public IFormFile File { get; set; }
        public string Title { get; set; }
        public string FileType { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        //public int? CategoryCode { get; set; }
        public string Thumbnail { get; set; }
    }
}
