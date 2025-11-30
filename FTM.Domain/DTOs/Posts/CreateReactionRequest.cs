using System;
using FTM.Domain.Enums;

namespace FTM.Domain.DTOs.Posts
{
    public class CreateReactionRequest
    {
        public Guid PostId { get; set; }
        public Guid FTMemberId { get; set; }
        public ReactionType ReactionType { get; set; } // 1: Like, 2: Love, 3: Haha, 4: Wow, 5: Sad, 6: Angry, 7: Care
    }
}
