using System;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class FamilyTreeDataTableDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid OwnerId { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }
        public string? FilePath { get; set; }
        public bool IsActive { get; set; }
        public int? GPModeCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public int MemberCount { get; set; }
    }
}