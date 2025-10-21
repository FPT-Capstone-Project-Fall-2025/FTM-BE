using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class FTMemberSimpleDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid FTId { get; set; }
        public string Fullname { get; set; }
        public int Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string? FilePath { get; set; }
        public bool IsRoot { get; set; }
    }
}
