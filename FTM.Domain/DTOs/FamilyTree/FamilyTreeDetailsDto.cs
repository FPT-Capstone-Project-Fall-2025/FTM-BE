using System;
using System.Collections.Generic;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class FamilyTreeDetailsDto
    {
        public Guid Id { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTimeOffset CreatedOn { get; set; }
        public string LastModifiedBy { get; set; } = string.Empty;
        public DateTimeOffset LastModifiedOn { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }
        public string Owner { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? FilePath { get; set; }
        public bool IsActive { get; set; }
        public int? GPModeCode { get; set; }
        public int NumberOfMember { get; set; }
        //public List<string> Roles { get; set; } = new List<string>();
        public bool IsNeedConfirmAcceptInvited { get; set; }
    }
}