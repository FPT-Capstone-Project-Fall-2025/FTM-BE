using System;

namespace FTM.Domain.Specification.Posts
{
    public class CommentSpecParams : BaseSpecParams
    {
        public Guid? PostId { get; set; }
    }
}
