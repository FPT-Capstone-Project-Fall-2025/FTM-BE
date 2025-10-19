using System;

namespace FTM.Domain.Specification.Posts
{
    public class ReactionSpecParams : BaseSpecParams
    {
        public Guid? PostId { get; set; }
    }
}
