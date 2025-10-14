using FTM.Domain.Entities.Posts;
using FTM.Domain.Enums;
using FTM.Infrastructure.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories.IRepositories
{
    public interface IPostReactionRepository : IGenericRepository<PostReaction>
    {
        Task<IEnumerable<PostReaction>> GetReactionsByPostAsync(Guid postId);
        Task<PostReaction> GetReactionByMemberAsync(Guid memberId, Guid postId);
        Task<Dictionary<ReactionType, int>> GetReactionsSummaryForPostAsync(Guid postId);
    }
}
