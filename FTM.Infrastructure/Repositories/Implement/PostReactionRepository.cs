using FTM.Domain.Entities.Posts;
using FTM.Domain.Enums;
using FTM.Domain.Specification.Posts;
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

        public async Task<IEnumerable<PostReaction>> GetReactionsAsync(ReactionSpecParams specParams)
        {
            var query = Context.PostReactions
                .Include(r => r.FTMember)
                .Where(r => r.IsDeleted == false);

            // Filter by PostId if provided
            if (specParams.PostId.HasValue)
            {
                query = query.Where(r => r.PostId == specParams.PostId.Value);
            }

            // Apply ordering
            query = query.OrderByDescending(r => r.CreatedOn);

            // Apply pagination
            return await query
                .Skip(specParams.Skip)
                .Take(specParams.Take)
                .ToListAsync();
        }

        public async Task<int> CountReactionsAsync(ReactionSpecParams specParams)
        {
            var query = Context.PostReactions.Where(r => r.IsDeleted == false);

            // Filter by PostId if provided
            if (specParams.PostId.HasValue)
            {
                query = query.Where(r => r.PostId == specParams.PostId.Value);
            }

            return await query.CountAsync();
        }

        public async Task<PostReaction> GetReactionByMemberAsync(Guid memberId, Guid postId)
        {
            return await Context.PostReactions
                .FirstOrDefaultAsync(r => 
                    r.FTMemberId == memberId && 
                    r.PostId == postId);
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



