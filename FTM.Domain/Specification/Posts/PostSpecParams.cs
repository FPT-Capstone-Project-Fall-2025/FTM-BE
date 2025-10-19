using System;

namespace FTM.Domain.Specification.Posts
{
    public class PostSpecParams : BaseSpecParams
    {
        public Guid? FamilyTreeId { get; set; }
        public Guid? MemberId { get; set; }
    }
}
