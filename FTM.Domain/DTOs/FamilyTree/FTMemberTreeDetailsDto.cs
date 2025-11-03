using FTM.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class FTMemberTreeDetailsDto
    {
        public Guid Id { get; set; }
        public bool? IsActive { get; set; }
        public string Name { get; set; }
        public int? Gender { get; set; }
        public string? Avatar { get; set; }
        public DateTime? Birthday { get; set; }
        public int StatusCode { get; set; }
        public bool? IsRoot { get; set; }
        public bool? IsDivorced { get; set; }
        public List<Guid> Partners { get; set; }
        public List<KeyValueModel> Children { get; set; }
        public bool IsCurrentMember { get; set; }
        public bool IsPartner { get; set; }
    }
}
