using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.DTOs.FamilyTree
{
    /// <summary>
    /// Represents a summary of family relationships for a member.
    /// </summary>
    public class MemberRelationshipDto
    {
        public bool HasFather { get; set; }
        public bool HasMother { get; set; }
        public bool HasSiblings { get; set; }
        public bool HasPartner { get; set; }
        public bool HasChildren { get; set; }
    }
}
