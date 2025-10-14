using FTM.Domain.Entities.Posts;
using FTM.Domain.Enums;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Implement;
using FTM.Infrastructure.Repositories.Interface;
using FTM.Infrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories
{
    public class PostReactionRepository : GenericRepository<PostReaction>, IPostReactionRepository
    {
        public PostReactionRepository(FTMDbContext context, ICurrentUserResolver currentUserResolver) 
            : base(context, currentUserResolver)
        {
        }

        public async Task<IEnumerable<PostReaction>> GetReactionsByPostAsync(Guid postId)
        {
            return await Context.PostReactions
                .Include(r => r.GPMember)
                .Where(r => r.PostId == postId && r.IsDeleted == false)
                .ToListAsync();
        }

        public async Task<PostReaction> GetReactionByMemberAsync(Guid memberId, Guid postId)
        {
            return await Context.PostReactions
                .FirstOrDefaultAsync(r => 
                    r.GPMemberId == memberId && 
                    r.PostId == postId && 
                    r.IsDeleted == false);
        }

        public async Task<Dictionary<ReactionType, int>> GetReactionsSummaryForPostAsync(Guid postId)
        {
            var reactions = await Context.PostReactions
                .Where(r => r.PostId == postId && r.IsDeleted == false)
                .GroupBy(r => r.ReactionType)
                .Select(g => new { ReactionType = g.Key, Count = g.Count() })
                .ToListAsync();

            return reactions.ToDictionary(r => r.ReactionType, r => r.Count);
        }
    }
}
