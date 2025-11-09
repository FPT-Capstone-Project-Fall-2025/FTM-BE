using FTM.Domain.Entities.Posts;
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
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        public PostRepository(FTMDbContext context, ICurrentUserResolver currentUserResolver) 
            : base(context, currentUserResolver)
        {
        }

        public async Task<IEnumerable<Post>> GetPostsAsync(PostSpecParams specParams)
        {
            var query = Context.Posts
                .Include(p => p.FTMember)
                .Include(p => p.PostAttachments)
                .Include(p => p.PostComments)
                .Include(p => p.PostReactions)
                .Where(p => p.IsDeleted == false);

            // Filter by FamilyTreeId if provided
            if (specParams.FamilyTreeId.HasValue)
            {
                query = query.Where(p => p.FTMemberId == specParams.FamilyTreeId.Value);
            }

            // Filter by MemberId if provided
            if (specParams.MemberId.HasValue)
            {
                query = query.Where(p => p.FTMemberId == specParams.MemberId.Value);
            }

            // Apply ordering
            query = query.OrderByDescending(p => p.CreatedOn);

            // Apply pagination
            return await query
                .Skip(specParams.Skip)
                .Take(specParams.Take)
                .ToListAsync();
        }

        public async Task<int> CountPostsAsync(PostSpecParams specParams)
        {
            var query = Context.Posts.Where(p => p.IsDeleted == false);

            // Filter by FamilyTreeId if provided
            if (specParams.FamilyTreeId.HasValue)
            {
                query = query.Where(p => p.FTMemberId == specParams.FamilyTreeId.Value);
            }

            // Filter by MemberId if provided
            if (specParams.MemberId.HasValue)
            {
                query = query.Where(p => p.FTMemberId == specParams.MemberId.Value);
            }

            return await query.CountAsync();
        }

        public async Task<Post> GetPostWithDetailsAsync(Guid postId)
        {
            return await Context.Posts
                .Include(p => p.FTMember)
                .Include(p => p.PostAttachments)
                .Include(p => p.PostComments)
                    .ThenInclude(c => c.FTMember)
                .Include(p => p.PostComments)
                    .ThenInclude(c => c.ChildComments)
                        .ThenInclude(cc => cc.FTMember)
                .Include(p => p.PostReactions)
                    .ThenInclude(r => r.FTMember)
                .FirstOrDefaultAsync(p => p.Id == postId && p.IsDeleted == false);
        }

        public async Task<Post> GetPostWithAttachmentsAsync(Guid postId)
        {
            return await Context.Posts
                .Include(p => p.PostAttachments)
                .FirstOrDefaultAsync(p => p.Id == postId && p.IsDeleted == false);
        }
    }
}



